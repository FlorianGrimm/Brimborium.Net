namespace Brimborium.LocalObservability;

public class ReactiveLoggerSinkMatcher
    : IReactiveLoggerSink
    , IObserver<ILogEntry> {
    private readonly IMatchingEngine _MatchingEngine;

    public ReactiveLoggerSinkMatcher(
        IMatchingEngine matchingEngine
        ) {
        this._MatchingEngine = matchingEngine;
        //this.SourceMatches = new Subject<object>();
    }

    public IDisposable Subscribe(IReactiveLoggerSource reactiveLoggerSource) {
        return reactiveLoggerSource.SourceLogEntry.Subscribe(this);
    }

    public void OnNext(ILogEntry logEntry) {
        // HACK for now
        System.Console.WriteLine(logEntry.ToString());
        if (logEntry.Scopes is not null) {
            foreach (var scope in logEntry.Scopes) {
                System.Console.WriteLine(scope);
            }
        }
        this._MatchingEngine.Match(new LogEntryData(logEntry, ProxyILogEntry2MatchingEntry.GetInstance()));
    }

    public void OnCompleted() {
    }

    public void OnError(Exception error) {
    }

}
