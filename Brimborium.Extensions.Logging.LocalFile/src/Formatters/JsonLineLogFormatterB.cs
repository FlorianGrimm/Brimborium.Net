namespace Brimborium.Extensions.Logging.LocalFile.Formatters;

public class JsonLineLogFormatterB : BaseJsonLogFormatterB {
    private const string MessageTemplateKey = "{OriginalFormat}";

    public JsonLineLogFormatterB() {

    }
    
    public override string Name => "jsonline";
    
    public override void Write<TState>(
        in LogEntryB<TState> logEntry,
        BatchingLoggerState batchingLoggerState,
        Action<DateTimeOffset, string> addMessage) {
        string message;
        if (logEntry.Exception is not null) {
            message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        } else {
            message = "";
        }
        int DefaultBufferSize = 1024 + message.Length;
        var jsonWriterOptions = batchingLoggerState.JsonWriterOptions;
        var timestampFormat = batchingLoggerState.TimestampFormat;
        string result;
        using (var output = new PooledByteBufferWriter(DefaultBufferSize)) {
            using (var writer = new Utf8JsonWriter(output, jsonWriterOptions)) {
                writer.WriteStartObject();
                var timestampFormatText = timestampFormat ?? "u"; //"yyyy-MM-dd HH:mm:ss.fff zzz";
                writer.WriteString("Timestamp", logEntry.Timestamp.ToString(timestampFormatText));
                writer.WriteNumber("EventId", logEntry.EventId.Id);
                writer.WriteString("LogLevel", GetLogLevelString(logEntry.LogLevel));
                writer.WriteString("Category", logEntry.Category);
                if (!string.IsNullOrEmpty(message)) {
                    writer.WriteString("Message", message);
                }

                if (logEntry.Exception != null) {
                    var exceptionMessage = logEntry.Exception.ToString();
                    if (!jsonWriterOptions.Indented) {
                        exceptionMessage = exceptionMessage.Replace(Environment.NewLine, " ");
                    }
                    writer.WriteString(nameof(Exception), exceptionMessage);
                }

                if (logEntry.State != null) {
                    writer.WriteStartObject(nameof(logEntry.State));
                    if (logEntry.State is IReadOnlyCollection<KeyValuePair<string, object>> stateProperties) {
                        foreach (KeyValuePair<string, object> item in stateProperties) {
                            if (item.Key == "{OriginalFormat}") {
                                WriteItem(writer, item);
                                break;
                            } else {
                            }
                        }
                        foreach (KeyValuePair<string, object> item in stateProperties) {
                            if (item.Key == "{OriginalFormat}") {
                                //
                            } else {
                                WriteItem(writer, item);
                            }
                        }
                    }
                    writer.WriteEndObject();
                }
                WriteScopeInformation(writer, batchingLoggerState.IncludeScopes, batchingLoggerState.ScopeProvider);
                writer.WriteEndObject();
                writer.Flush();
                //if ((output.WrittenCount + 2) < output.Capacity) { }
                output.Write(new ReadOnlySpan<byte>(crlf));
            }
            result = Encoding.UTF8.GetString(output.WrittenMemory.Span);
        }
        addMessage(logEntry.Timestamp, result);
    }

    private static byte[] crlf = new byte[] { 13, 10 };
}