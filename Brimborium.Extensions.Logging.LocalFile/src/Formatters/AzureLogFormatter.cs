namespace Brimborium.Extensions.Logging.LocalFile.Formatters;

public class AzureLogFormatter : ILogFormatterB {
    private StringBuilder? _Builder;

    public string Name => "AzureLog";

    public void Write<TState>(
        in LogEntryB<TState> logEntry,
        BatchingLoggerState batchingLoggerState,
        Action<DateTimeOffset, string> addMessage) {

        StringBuilder builder;
        builder = System.Threading.Interlocked.Exchange(ref _Builder, null)
                ?? new StringBuilder(1024);
        var timestampFormat = batchingLoggerState.TimestampFormat ?? "yyyy-MM-dd HH:mm:ss.fff zzz";
        builder.Append(logEntry.Timestamp.ToString(timestampFormat /*"yyyy-MM-dd HH:mm:ss.fff zzz"*/, CultureInfo.InvariantCulture));
        builder.Append(" [");

        builder.Append(GetLogLevelString(logEntry.LogLevel));
        builder.Append("] ");
        builder.Append(logEntry.Category);

        var scopeProvider = batchingLoggerState.ScopeProvider;
        if (scopeProvider is not null) {
            scopeProvider.ForEachScope((scope, stringBuilder) => {
                stringBuilder.Append(" => ").Append(scope);
            }, builder);

            builder.AppendLine(":");
        } else {
            builder.Append(": ");
        }

        if (batchingLoggerState.IncludeEventId) {
            builder.Append(logEntry.EventId.Id.ToString("d6"));
            builder.Append(": ");
        }

        builder.AppendLine(logEntry.Formatter(logEntry.State, logEntry.Exception));

        if (logEntry.Exception is not null) {
            builder.AppendLine(logEntry.Exception.ToString()).Replace(Environment.NewLine, "; ");
        }

        addMessage(logEntry.Timestamp, builder.ToString());
        builder.Length = 0;
        System.Threading.Interlocked.Exchange(ref _Builder, builder);
    }

    private static string GetLogLevelString(LogLevel logLevel) {
        return logLevel switch {
            LogLevel.Trace => "Trace",
            LogLevel.Debug => "Debug",
            LogLevel.Information => "Information",
            LogLevel.Warning => "Warning",
            LogLevel.Error => "Error",
            LogLevel.Critical => "Critical",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }
}
