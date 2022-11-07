using Microsoft.Extensions.Logging.Abstractions;

namespace Brimborium.Extensions.Logging.Reactive;
public class ReactiveLogger : ILogger {
    private readonly ReactiveLoggerProvider _Provider;
    private readonly string _CategoryName;

    public ReactiveLogger(
        ReactiveLoggerProvider reactiveLoggerProvider,
        string categoryName) {
        this._Provider = reactiveLoggerProvider;
        this._CategoryName = categoryName;
    }

    public IDisposable BeginScope<TState>(TState state) 
        // where TState : notnull 
        => this._Provider.ScopeProvider?.Push(state) ?? NullScope.Instance;

    public bool IsEnabled(LogLevel logLevel) {
        return (this._Provider.IsEnabled) && (logLevel != LogLevel.None);
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter) {
        var line = formatter(state, exception);
        var utcNow = System.DateTimeOffset.UtcNow;
        var scopeProvider = this._Provider.ScopeProvider;
        List<object>? scopes;
        if (scopeProvider is null) {
            scopes = null;
        } else { 
            scopes=new List<object>();
            this._Provider.ScopeProvider?.ForEachScope((scope, lst) => {
                if (scope is not null) { 
                    lst.Add(scope);
                }
            }, scopes);
        }
        var logEntry = new LogEntry<TState>(
            this._CategoryName,
            logLevel,
            eventId,
            state,
            exception,
            utcNow,
            scopes,
            line);
        this._Provider.Log(logEntry);
    }
}
