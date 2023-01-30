using System.Text.Json;

namespace Brimborium.Extensions.Logging.LocalFile.Formatters;

public class JsonLogFormatterB : BaseJsonLogFormatterB {
    private const string MessageTemplateKey = "{OriginalFormat}";

    private static readonly JsonWriterOptions Options = new() {
        Indented = false,
    };

    public override string Name => "json";

    public override void Write<TState>(
        in LogEntryB<TState> logEntry,
        BatchingLoggerState batchingLoggerState,
        Action<DateTimeOffset, string> addMessage) {
        var exception = logEntry.Exception;
        var message = logEntry.Formatter(logEntry.State, exception);
        string result;
        var jsonWriterOptions = batchingLoggerState.JsonWriterOptions;
        var timestampFormat = batchingLoggerState.TimestampFormat;
        using (var output = new PooledByteBufferWriter(4096)) {
            using (var writer = new Utf8JsonWriter(output, jsonWriterOptions)) {

                writer.WriteStartObject();
                writer.WriteString("Timestamp", logEntry.Timestamp);
                writer.WriteString("Level", GetLogLevelString(logEntry.LogLevel));
                writer.WriteString("Category", logEntry.Category);
                writer.WriteString("Message", message);
                if (exception != null) {
                    var exceptionMessage = exception.ToString()
                        .Replace(Environment.NewLine, " ");
                    writer.WriteString(nameof(Exception), exceptionMessage);
                }

                string? messageTemplate = null;
                if (logEntry.State != null) {
                    writer.WriteStartObject(nameof(logEntry.State));
                    if (logEntry.State is IEnumerable<KeyValuePair<string, object>> stateProperties) {
                        foreach (KeyValuePair<string, object> item in stateProperties) {
                            if (item.Key == MessageTemplateKey
                                && item.Value is string template) {
                                messageTemplate = template;
                            } else {
                                WriteItem(writer, item);
                            }
                        }
                    } else {
                        writer.WriteString("Message", logEntry.State.ToString());
                    }
                    writer.WriteEndObject();
                }

                if (!string.IsNullOrEmpty(messageTemplate)) {
                    writer.WriteString("MessageTemplate", messageTemplate);
                }
                WriteScopeInformation(writer, batchingLoggerState.IncludeScopes, batchingLoggerState.ScopeProvider);

                writer.WriteEndObject();
                writer.Flush();
            }
            result = Encoding.UTF8.GetString(output.WrittenMemory.Span);
        }
        addMessage(logEntry.Timestamp, result);
    }
}