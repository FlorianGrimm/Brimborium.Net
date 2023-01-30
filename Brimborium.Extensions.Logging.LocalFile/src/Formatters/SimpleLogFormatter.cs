namespace Brimborium.Extensions.Logging.LocalFile.Formatters;

/// <summary>
/// A simple formatter for log messages
/// </summary>
public class SimpleLogFormatter : ILogFormatterB {
    public string Name => "simple";

    /// <inheritdoc />
    public void Write<TState>(
        in LogEntryB<TState> logEntry,
        BatchingLoggerState batchingLoggerState,
        Action<DateTimeOffset, string> addMessage) {
        var scopeProvider = batchingLoggerState.ScopeProvider;

        var builder = new StringBuilder();
        builder.Append(logEntry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"));
        builder.Append(" [");
        builder.Append(logEntry.LogLevel.ToString());
        builder.Append("] ");
        builder.Append(logEntry.Category);

        if (scopeProvider is not null) {
            scopeProvider.ForEachScope((scope, stringBuilder) => {
                stringBuilder.Append(" => ").Append(scope);
            }, builder);

            builder.Append(':').AppendLine();
        } else {
            builder.Append(": ");
        }

        builder.AppendLine(logEntry.Formatter(logEntry.State, logEntry.Exception));

        if (logEntry.Exception != null) {
            builder.AppendLine(logEntry.Exception.ToString());
        }
    }
}