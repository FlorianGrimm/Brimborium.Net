using System.Reactive.Subjects;

namespace Brimborium.Extensions.Logging.Reactive;

public interface IReactiveLoggerSource {
    Subject<ILogEntry> SourceLogEntry { get; }

    void Initialize();

    void Log<TState>(LogEntry<TState> logEntry);
}

public interface IReactiveLoggerSink {
    void Initialize(IReactiveLoggerSource reactiveLoggerSource);
}

public class ReactiveLoggerSource : IReactiveLoggerSource {
    // private bool _IsInitialized;
    private readonly IServiceProvider _ServiceProvider;

    public Subject<ILogEntry> SourceLogEntry { get; }

    public ReactiveLoggerSource(IServiceProvider serviceProvider) {
        this.SourceLogEntry = new System.Reactive.Subjects.Subject<ILogEntry>();
        this._ServiceProvider = serviceProvider;
    }

    public void Log<TState>(LogEntry<TState> logEntry) {
        //if (!this._IsInitialized) {
        //    this._IsInitialized = true;
        //}
        this.SourceLogEntry.OnNext(logEntry);
    }

    public void Initialize() {
        var lstReactiveLoggerSink = this._ServiceProvider.GetServices<IReactiveLoggerSink>();
        foreach (var reactiveLoggerSink in lstReactiveLoggerSink) {
            reactiveLoggerSink.Initialize(this);
        }
    }
}