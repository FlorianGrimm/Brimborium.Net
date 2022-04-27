// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Brimborium.Extensions.Logging.LocalFile;

/// <summary>
/// A provider of <see cref="BatchingLogger"/> instances.
/// </summary>
public abstract class BatchingLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly List<LogMessage> _currentBatch = new List<LogMessage>();
    private readonly TimeSpan _interval;
    private readonly int? _queueSize;
    private readonly int? _batchSize;
    private readonly IDisposable _optionsChangeToken;

    private int _messagesDropped;

    private BlockingCollection<LogMessage> _messageQueue;
    private Task _outputTask;
    private CancellationTokenSource _cancellationTokenSource;

    private bool _includeScopes;
    private IExternalScopeProvider _scopeProvider;

    internal protected IExternalScopeProvider ScopeProvider => this._includeScopes ? this._scopeProvider : null;

    internal protected BatchingLoggerProvider(IOptionsMonitor<BatchingLoggerOptions> options)
    {
        // NOTE: Only IsEnabled is monitored

        var loggerOptions = options.CurrentValue;
        if (loggerOptions.BatchSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(loggerOptions.BatchSize), $"{nameof(loggerOptions.BatchSize)} must be a positive number.");
        }
        if (loggerOptions.FlushPeriod <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(loggerOptions.FlushPeriod), $"{nameof(loggerOptions.FlushPeriod)} must be longer than zero.");
        }

        this._interval = loggerOptions.FlushPeriod;
        this._batchSize = loggerOptions.BatchSize;
        this._queueSize = loggerOptions.BackgroundQueueSize;

        this._optionsChangeToken = options.OnChange(this.UpdateOptions);
        this.UpdateOptions(options.CurrentValue);
    }

    /// <summary>
    /// Checks if the queue is enabled.
    /// </summary>
    public bool IsEnabled { get; private set; }

    private void UpdateOptions(BatchingLoggerOptions options)
    {
        var oldIsEnabled = this.IsEnabled;
        this.IsEnabled = options.IsEnabled;
        this._includeScopes = options.IncludeScopes;

        if (oldIsEnabled != this.IsEnabled)
        {
            if (this.IsEnabled)
            {
                this.Start();
            }
            else
            {
                this.Stop();
            }
        }

    }

    internal protected abstract Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken token);

    private async Task ProcessLogQueue()
    {
        while (!this._cancellationTokenSource.IsCancellationRequested)
        {
            var limit = this._batchSize ?? int.MaxValue;

            while (limit > 0 && this._messageQueue.TryTake(out var message))
            {
                this._currentBatch.Add(message);
                limit--;
            }

            var messagesDropped = Interlocked.Exchange(ref this._messagesDropped, 0);
            if (messagesDropped != 0)
            {
                this._currentBatch.Add(new LogMessage(DateTimeOffset.Now, $"{messagesDropped} message(s) dropped because of queue size limit. Increase the queue size or decrease logging verbosity to avoid this.{Environment.NewLine}"));
            }

            if (this._currentBatch.Count > 0)
            {
                try
                {
                    await this.WriteMessagesAsync(this._currentBatch, this._cancellationTokenSource.Token).ConfigureAwait(false);
                }
                catch
                {
                    // ignored
                }

                this._currentBatch.Clear();
            }
            else
            {
                await this.IntervalAsync(this._interval, this._cancellationTokenSource.Token).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Wait for the given <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="interval">The amount of time to wait.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the delay.</param>
    /// <returns>A <see cref="Task"/> which completes when the <paramref name="interval"/> has passed or the <paramref name="cancellationToken"/> has been canceled.</returns>
    protected virtual Task IntervalAsync(TimeSpan interval, CancellationToken cancellationToken)
    {
        return Task.Delay(interval, cancellationToken);
    }

    internal protected void AddMessage(DateTimeOffset timestamp, string message)
    {
        if (!this._messageQueue.IsAddingCompleted)
        {
            try
            {
                if (!this._messageQueue.TryAdd(new LogMessage(timestamp, message), millisecondsTimeout: 0, cancellationToken: this._cancellationTokenSource.Token))
                {
                    Interlocked.Increment(ref this._messagesDropped);
                }
            }
            catch
            {
                //cancellation token canceled or CompleteAdding called
            }
        }
    }

    private void Start()
    {
        this._messageQueue = this._queueSize == null ?
            new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>()) :
            new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>(), this._queueSize.Value);

        this._cancellationTokenSource = new CancellationTokenSource();
        this._outputTask = Task.Run(this.ProcessLogQueue);
    }

    private void Stop()
    {
        this._cancellationTokenSource.Cancel();
        this._messageQueue.CompleteAdding();

        try
        {
            this._outputTask.Wait(this._interval);
        }
        catch (TaskCanceledException)
        {
        }
        catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 && ex.InnerExceptions[0] is TaskCanceledException)
        {
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this._optionsChangeToken?.Dispose();
        if (this.IsEnabled)
        {
            this.Stop();
        }
    }

    /// <summary>
    /// Creates a <see cref="BatchingLogger"/> with the given <paramref name="categoryName"/>.
    /// </summary>
    /// <param name="categoryName">The name of the category to create this logger with.</param>
    /// <returns>The <see cref="BatchingLogger"/> that was created.</returns>
    public ILogger CreateLogger(string categoryName)
    {
        return new BatchingLogger(this, categoryName);
    }

    /// <summary>
    /// Sets the scope on this provider.
    /// </summary>
    /// <param name="scopeProvider">Provides the scope.</param>
    void ISupportExternalScope.SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        this._scopeProvider = scopeProvider;
    }
}
