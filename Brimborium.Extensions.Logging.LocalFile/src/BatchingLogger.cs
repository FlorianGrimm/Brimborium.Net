// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Brimborium.Extensions.Logging.LocalFile;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class BatchingLogger : ILogger {
    private readonly BatchingLoggerProvider _provider;
    private readonly string _category;
    private readonly Action<DateTimeOffset, string> _AddMessage;

    public BatchingLogger(
        BatchingLoggerProvider loggerProvider,
        string categoryName) {
        this._provider = loggerProvider;
        this._category = categoryName;
        this._AddMessage = this._provider.AddMessage;
    }

    private BatchingLoggerState GetBatchingLoggerState()
        => this._provider.GetBatchingLoggerState();

    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull
        => this.GetBatchingLoggerState().ScopeProvider?.Push(state) ?? NullScope.Instance;

    public bool IsEnabled(LogLevel logLevel) {
        return (this.GetBatchingLoggerState().IsEnabled) && (logLevel != LogLevel.None);
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter) {
        if (!this.IsEnabled(logLevel)) {
            return;
        }
        var logFormatter = this._provider.GetLogFormatter();
        if (logFormatter is not null) {
            var batchingLoggerState = this.GetBatchingLoggerState();
            DateTimeOffset timestamp = batchingLoggerState.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
            var logEntry = new LogEntryB<TState>(timestamp, logLevel, _category, eventId, state, exception, formatter);
            logFormatter.Write(in logEntry, batchingLoggerState, this._AddMessage);
        }
    }

    public void Log<TState>(DateTimeOffset timestamp, LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
        if (!this.IsEnabled(logLevel)) {
            return;
        }

        var logFormatter = this._provider.GetLogFormatter();
        if (logFormatter is not null) {
            var batchingLoggerState = this.GetBatchingLoggerState();
            var logEntry = new LogEntryB<TState>(timestamp, logLevel, _category, eventId, state, exception, formatter);
            logFormatter.Write(in logEntry, batchingLoggerState, this._AddMessage);
        }
    }
}
