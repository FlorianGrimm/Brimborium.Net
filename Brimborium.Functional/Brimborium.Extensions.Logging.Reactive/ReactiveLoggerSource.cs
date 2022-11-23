namespace Brimborium.Extensions.Logging.Reactive;

public interface IReactiveLoggerSource {
    Subject<ILogEntry> SourceLogEntry { get; }

    void Initialize();

    void Log<TState>(LogEntry<TState> logEntry);
}

public interface IReactiveLoggerSink {
    IDisposable Subscribe(IReactiveLoggerSource reactiveLoggerSource);
}

public static class ReactiveLoggerSourceExtensions {
    public static void UseServiceReactiveLoggerSource(this IServiceProvider serviceProvider) {
        serviceProvider.GetRequiredService<IReactiveLoggerSource>().Initialize();
    }
}

public class ReactiveLoggerSource : IReactiveLoggerSource {

    private readonly IServiceProvider _ServiceProvider;

    public Subject<ILogEntry> SourceLogEntry { get; }

    public ReactiveLoggerSource(IServiceProvider serviceProvider) {
        this.SourceLogEntry = new System.Reactive.Subjects.Subject<ILogEntry>();
        this._ServiceProvider = serviceProvider;
    }

    public void Initialize() {
        var lstReactiveLoggerSink = this._ServiceProvider.GetServices<IReactiveLoggerSink>();
        foreach (var reactiveLoggerSink in lstReactiveLoggerSink) {
            reactiveLoggerSink.Subscribe(this);
        }
    }

    public void Log<TState>(LogEntry<TState> logEntry) {
        this.SourceLogEntry.OnNext(logEntry);
    }
}