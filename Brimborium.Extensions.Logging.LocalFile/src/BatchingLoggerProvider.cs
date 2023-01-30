// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Brimborium.Extensions.Logging.LocalFile.Formatters;

using Microsoft.Extensions.DependencyInjection;

namespace Brimborium.Extensions.Logging.LocalFile;

/// <summary>
/// A provider of <see cref="BatchingLogger"/> instances.
/// </summary>
public abstract class BatchingLoggerProvider : ILoggerProvider, ISupportExternalScope {
    private readonly List<LogMessage> _currentBatch = new List<LogMessage>();
    private readonly TimeSpan _Interval;
    private readonly int _QueueSize;
    private readonly long _BatchSize;
    private IDisposable? _OptionsChangeToken;

    private bool _Started;
    private BlockingCollection<LogMessage>? _MessageQueue;
    private Task? _OutputTask;
    private CancellationTokenSource? _CancellationTokenSource;
    private long _MessagesDropped;

    private BatchingLoggerState _BatchingLoggerState;
    protected ILogFormatterB? _LogFormatter;

    internal protected BatchingLoggerProvider(
        IOptionsMonitor<BatchingLoggerOptions> options
        ) {
        this._BatchingLoggerState = BatchingLoggerState.Empty();

        var loggerOptions = options.CurrentValue;
        if (loggerOptions.BatchSize <= 0) {
            throw new ArgumentOutOfRangeException(nameof(loggerOptions.BatchSize), $"{nameof(loggerOptions.BatchSize)} must be a positive number.");
        }
        if (loggerOptions.FlushPeriod <= TimeSpan.Zero) {
            throw new ArgumentOutOfRangeException(nameof(loggerOptions.FlushPeriod), $"{nameof(loggerOptions.FlushPeriod)} must be longer than zero.");
        }

        this._Interval = loggerOptions.FlushPeriod;
        this._BatchSize = loggerOptions.BatchSize ?? long.MaxValue;
        this._QueueSize = loggerOptions.BackgroundQueueSize ?? 0;


        this._OptionsChangeToken = options.OnChange(this.UpdateOptions);
        this.UpdateOptions(options.CurrentValue);
    }

    public BatchingLoggerState GetBatchingLoggerState()
        => this._BatchingLoggerState;

    internal protected virtual ILogFormatterB GetLogFormatter()
        => this._LogFormatter ??= this.CreateLogFormatter<AzureLogFormatter>();

    protected ILogFormatterB CreateLogFormatter<T>()
        where T : ILogFormatterB, new() {

        if (this._BatchingLoggerState.LogFormatter is not null) {
            return this._BatchingLoggerState.LogFormatter;
        }

        if (!string.IsNullOrEmpty(this._BatchingLoggerState.UseLogFormatter)) {
            var logFormatter = LogFormatterBFactory.GetInstance().CreateILogFormatterB(
                this._BatchingLoggerState.UseLogFormatter);
            if (logFormatter is not null) {
                return logFormatter;
            }
        }
        {
            return new T();
        }
    }

    protected virtual void UpdateOptions(BatchingLoggerOptions options) {
        var oldState = this._BatchingLoggerState;
        var currentState = new BatchingLoggerState(
            IncludeScopes: options.IncludeScopes,
            ScopeProvider: this._BatchingLoggerState.ScopeProvider,
            IsEnabled: options.IsEnabled,
            IncludeEventId: options.IncludeEventId,
            JsonWriterOptions: options.JsonWriterOptions,
            TimestampFormat: options.TimestampFormat,
            UseUtcTimestamp: options.UseUtcTimestamp,
            UseLogFormatter: options.UseLogFormatter,
            LogFormatter: options.LogFormatter
            );
        this._BatchingLoggerState = currentState;

        if (!(ReferenceEquals(oldState.LogFormatter, currentState.LogFormatter)
            && string.Equals(oldState.UseLogFormatter, currentState.UseLogFormatter, StringComparison.OrdinalIgnoreCase)
            )) {
            this._LogFormatter = null;
        }

        if (oldState.IsEnabled != currentState.IsEnabled) {
            if (currentState.IsEnabled) {
                this.Start();
            } else {
                this.Stop();
            }
        }
    }

    internal protected abstract Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken token);

    private async Task ProcessLogQueue() {
        if (this._CancellationTokenSource is null) { throw new ArgumentException("_cancellationTokenSource is null"); }
        if (this._MessageQueue is null) { throw new ArgumentException("_messageQueue is null"); }

        while (!this._CancellationTokenSource.IsCancellationRequested) {
            var limit = this._BatchSize;

            if (System.Threading.Interlocked.Read(ref this._MessagesDropped) == 0) {
                if (this._MessageQueue.TryTake(out var message, 60000, this._CancellationTokenSource.Token)) {
                    this._currentBatch.Add(message);
                    limit--;
                }
            }

            {
                while (limit > 0 && this._MessageQueue.TryTake(out var message)) {
                    this._currentBatch.Add(message);
                    limit--;
                }
            }

            var messagesDropped = Interlocked.Exchange(ref this._MessagesDropped, 0);
            if (messagesDropped != 0) {
                this._currentBatch.Add(new LogMessage(DateTimeOffset.Now, $"{messagesDropped} message(s) dropped because of queue size limit. Increase the queue size or decrease logging verbosity to avoid this.{Environment.NewLine}"));
            }

            if (this._currentBatch.Count > 0) {
                try {
                    await this.WriteMessagesAsync(this._currentBatch, this._CancellationTokenSource.Token).ConfigureAwait(false);
                } catch {
                    // ignored
                }

                this._currentBatch.Clear();
            } else {
                await this.IntervalAsync(this._Interval, this._CancellationTokenSource.Token).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Wait for the given <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="interval">The amount of time to wait.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the delay.</param>
    /// <returns>A <see cref="Task"/> which completes when the <paramref name="interval"/> has passed or the <paramref name="cancellationToken"/> has been canceled.</returns>
    protected virtual Task IntervalAsync(TimeSpan interval, CancellationToken cancellationToken) {
        return Task.Delay(interval, cancellationToken);
    }

    internal protected void AddMessage(DateTimeOffset timestamp, string message) {
        if (this._MessageQueue is null) { throw new ArgumentException("_messageQueue is null"); }

        if (!this._MessageQueue.IsAddingCompleted) {
            try {
                if (!this._MessageQueue.TryAdd(
                   item: new LogMessage(timestamp, message),
                    millisecondsTimeout: 0,
                    cancellationToken: (this._CancellationTokenSource is null)
                    ? CancellationToken.None
                    : this._CancellationTokenSource.Token)) {
                    Interlocked.Increment(ref this._MessagesDropped);
                }
            } catch {
                //cancellation token canceled or CompleteAdding called
            }
        }
    }

    private void Start() {
        if (this._Started) { return; }
        this._Started = true;

        this._MessageQueue = this._QueueSize <= 0 ?
            new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>()) :
            new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>(), this._QueueSize);

        this._CancellationTokenSource = new CancellationTokenSource();
        this._OutputTask = Task.Run(this.ProcessLogQueue);
    }

    private void Stop() {
        if (!this._Started) { return; }
        this._Started = false;

        this._CancellationTokenSource?.Cancel();
        this._MessageQueue?.CompleteAdding();

        try {
            this._OutputTask?.Wait(this._Interval);
        } catch (TaskCanceledException) {
        } catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 && ex.InnerExceptions[0] is TaskCanceledException) {
        }
    }

    /// <inheritdoc/>
    public void Dispose() {
        this._OptionsChangeToken?.Dispose();
        if (this._BatchingLoggerState.IsEnabled) {
            this.Stop();
        }
    }

    /// <summary>
    /// Creates a <see cref="BatchingLogger"/> with the given <paramref name="categoryName"/>.
    /// </summary>
    /// <param name="categoryName">The name of the category to create this logger with.</param>
    /// <returns>The <see cref="BatchingLogger"/> that was created.</returns>
    public ILogger CreateLogger(string categoryName) {
        return new BatchingLogger(this, categoryName);
    }

    /// <summary>
    /// Sets the scope on this provider.
    /// </summary>
    /// <param name="scopeProvider">Provides the scope.</param>
    void ISupportExternalScope.SetScopeProvider(IExternalScopeProvider scopeProvider) {
        this._BatchingLoggerState = this._BatchingLoggerState with { ScopeProvider = scopeProvider };
    }
}
