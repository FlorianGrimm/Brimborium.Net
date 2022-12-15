namespace Brimborium.LocalObservability;

public class ReactiveLoggerSinkMatcher
    : IReactiveLoggerSink
    , IObserver<ILogEntry> {
    private readonly IMatchingEngine _MatchingEngine;

    public ReactiveLoggerSinkMatcher(
        IMatchingEngine matchingEngine
        ) {
        this._MatchingEngine = matchingEngine;
    }

    public IDisposable Subscribe(IReactiveLoggerSource reactiveLoggerSource) {
        return reactiveLoggerSource.SourceLogEntry.Subscribe(this);
    }

    public void OnNext(ILogEntry logEntry) {
        // HACK for now
        //System.Console.WriteLine(logEntry.ToString());
        //if (logEntry.Scopes is not null) {
        //    foreach (var scope in logEntry.Scopes) {
        //        System.Console.WriteLine(scope);
        //    }
        //}
        try {
            this._MatchingEngine.Match(new LogEntryData(logEntry, LogEntryDataAccessor.GetInstance()));
        } catch (Exception error) {
#warning error
        }
    }

    public void OnCompleted() {
    }

    public void OnError(Exception error) {
    }

}
