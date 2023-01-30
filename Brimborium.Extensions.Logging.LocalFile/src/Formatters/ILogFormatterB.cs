using Microsoft.Extensions.DependencyInjection;

namespace Brimborium.Extensions.Logging.LocalFile.Formatters;

/// <summary>
/// Formats log messages that are written to the log file
/// </summary>
public interface ILogFormatterB {
    /// <summary>
    /// Gets the name of the formatter
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Writes the log message to the specified StringBuilder.
    /// </summary>
    /// <param name="logEntry">The log entry.</param>
    /// <param name="scopeProvider">The provider of scope data.</param>
    /// <typeparam name="TState">The type of the object to be written.</typeparam>
    void Write<TState>(
        in LogEntryB<TState> logEntry,
        BatchingLoggerState batchingLoggerState,
        Action<DateTimeOffset, string> addMessage);
}
public class LogFormatterBFactory {
    private static LogFormatterBFactory? _Instance;
    public static LogFormatterBFactory GetInstance() {
        if (_Instance is not null) {
            return _Instance;
        }
        var instance = new LogFormatterBFactory(new List<ILogFormatterB>() {
            new AzureLogFormatter(),
            new JsonLineLogFormatterB(),
            new JsonLogFormatterB(),
            new SimpleLogFormatter()
        });

        return _Instance = instance;
    }

    private List<ILogFormatterB> _LogFormatters;

    public LogFormatterBFactory(List<ILogFormatterB> logFormatters) {
        this._LogFormatters = logFormatters;
    }

    public ILogFormatterB? CreateILogFormatterB(string name) {
        var logFormatters = this._LogFormatters;
        return logFormatters.FirstOrDefault(
                logFormatter => string.Equals(logFormatter.Name, name, StringComparison.OrdinalIgnoreCase)
            );
    }
}