namespace Brimborium.LocalObservability.TestWebApplication1 {
    public class ReactiveLoggerSinkMatcher : IReactiveLoggerSink {
        public void Initialize(IReactiveLoggerSource reactiveLoggerSource) {
            reactiveLoggerSource.SourceLogEntry.Subscribe((logEntry) => {
                System.Console.WriteLine(logEntry.ToString());
                if (logEntry.Scopes is not null) {
                    foreach (var scope in logEntry.Scopes) { 
                    System.Console.WriteLine(scope);
                    }
                }
            });
        }
    }
}