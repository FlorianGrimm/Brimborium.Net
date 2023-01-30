namespace Brimborium.Extensions.Logging.LocalFile.Formatters;
public abstract class BaseJsonLogFormatterB : ILogFormatterB {
    public BaseJsonLogFormatterB() {
    }

    public abstract string Name { get; }

    public abstract void Write<TState>(in LogEntryB<TState> logEntry, BatchingLoggerState batchingLoggerState, Action<DateTimeOffset, string> addMessage);

    protected static string GetLogLevelString(LogLevel logLevel) {
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

    protected static void WriteScopeInformation(
        Utf8JsonWriter writer, 
        bool includeScopes,
        IExternalScopeProvider? scopeProvider
        ) {
        if (includeScopes && scopeProvider is not null) {
            writer.WriteStartArray("Scopes");
            scopeProvider.ForEachScope((scope, state) => {
                if (scope is IEnumerable<KeyValuePair<string, object>> scopeItems) {
                    state.WriteStartObject();
                    state.WriteString("Message", scope.ToString());
                    foreach (KeyValuePair<string, object> item in scopeItems) {
                        WriteItem(state, item);
                    }
                    state.WriteEndObject();
                } else {
                    state.WriteStringValue(ToInvariantString(scope));
                }
            }, writer);
            writer.WriteEndArray();
        }
    }

    protected static void WriteItem(Utf8JsonWriter writer, KeyValuePair<string, object> item) {
        var key = item.Key;
        switch (item.Value) {
            case bool boolValue:
                writer.WriteBoolean(key, boolValue);
                break;
            case byte byteValue:
                writer.WriteNumber(key, byteValue);
                break;
            case sbyte sbyteValue:
                writer.WriteNumber(key, sbyteValue);
                break;
            case char charValue:
#if NETCOREAPP
                writer.WriteString(key, MemoryMarshal.CreateSpan(ref charValue, 1));
#else
                    writer.WriteString(key, charValue.ToString());
#endif
                break;
            case decimal decimalValue:
                writer.WriteNumber(key, decimalValue);
                break;
            case double doubleValue:
                writer.WriteNumber(key, doubleValue);
                break;
            case float floatValue:
                writer.WriteNumber(key, floatValue);
                break;
            case int intValue:
                writer.WriteNumber(key, intValue);
                break;
            case uint uintValue:
                writer.WriteNumber(key, uintValue);
                break;
            case long longValue:
                writer.WriteNumber(key, longValue);
                break;
            case ulong ulongValue:
                writer.WriteNumber(key, ulongValue);
                break;
            case short shortValue:
                writer.WriteNumber(key, shortValue);
                break;
            case ushort ushortValue:
                writer.WriteNumber(key, ushortValue);
                break;
            case null:
                writer.WriteNull(key);
                break;
            default:
                writer.WriteString(key, ToInvariantString(item.Value));
                break;
        }
    }

    protected static void WriteValue(Utf8JsonWriter writer, object item) {
        switch (item) {
            case bool boolValue:
                writer.WriteBooleanValue(boolValue);
                break;
            case byte byteValue:
                writer.WriteNumberValue(byteValue);
                break;
            case sbyte sbyteValue:
                writer.WriteNumberValue(sbyteValue);
                break;
            case char charValue:
#if NETCOREAPP
                writer.WriteStringValue(MemoryMarshal.CreateSpan(ref charValue, 1));
#else
                    writer.WriteStringValue(charValue.ToString());
#endif
                break;
            case decimal decimalValue:
                writer.WriteNumberValue(decimalValue);
                break;
            case double doubleValue:
                writer.WriteNumberValue(doubleValue);
                break;
            case float floatValue:
                writer.WriteNumberValue(floatValue);
                break;
            case int intValue:
                writer.WriteNumberValue(intValue);
                break;
            case uint uintValue:
                writer.WriteNumberValue(uintValue);
                break;
            case long longValue:
                writer.WriteNumberValue(longValue);
                break;
            case ulong ulongValue:
                writer.WriteNumberValue(ulongValue);
                break;
            case short shortValue:
                writer.WriteNumberValue(shortValue);
                break;
            case ushort ushortValue:
                writer.WriteNumberValue(ushortValue);
                break;
            case null:
                writer.WriteNullValue();
                break;
            default:
                writer.WriteStringValue(ToInvariantString(item));
                break;
        }
    }

    protected static string? ToInvariantString(object? obj)
        => Convert.ToString(obj, CultureInfo.InvariantCulture);

    protected readonly struct WriterWrapper {
        public readonly Utf8JsonWriter Writer;
        public readonly List<object> Values;

        public WriterWrapper(Utf8JsonWriter writer) {
            Writer = writer;
            Values = new List<object>();
        }
    }
}
