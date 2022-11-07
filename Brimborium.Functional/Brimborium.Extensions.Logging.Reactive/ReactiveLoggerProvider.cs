namespace Brimborium.Extensions.Logging.Reactive;

[ProviderAlias("ReactiveLogger")]
public class ReactiveLoggerProvider : ILoggerProvider, ISupportExternalScope {
    private IExternalScopeProvider? _ScopeProvider;
    private readonly IDisposable _optionsChangeToken;
    private readonly IReactiveLoggerSource _ReactiveLoggerSource;
    private bool _IncludeScopes;
    
    internal IExternalScopeProvider? ScopeProvider => this._IncludeScopes ? this._ScopeProvider : null;

    public bool IsEnabled { get; set; }

    public ReactiveLoggerProvider(
        IOptionsMonitor<ReactiveLoggerOptions> options,
        IReactiveLoggerSource reactiveLoggerSource
        ) {
        this._optionsChangeToken = options.OnChange(this.UpdateOptions);
        this.UpdateOptions(options.CurrentValue);
        this._ReactiveLoggerSource = reactiveLoggerSource;
    }

    private void UpdateOptions(ReactiveLoggerOptions options) {
        //var oldIsEnabled = this.IsEnabled;
        this.IsEnabled = options.IsEnabled;

        this._IncludeScopes = options.IncludeScopes;
        
        //if (oldIsEnabled != this.IsEnabled) {
        //    if (this.IsEnabled) {
        //        this.Start();
        //    } else {
        //        this.Stop();
        //    }
        //}
    }

    //private void Start() {
    //}
    //private void Stop() {
    //}

    public ILogger CreateLogger(string categoryName) {
        return new ReactiveLogger(this, categoryName);
    }

    public void Dispose() {
        this._optionsChangeToken.Dispose();
    }

    public void SetScopeProvider(IExternalScopeProvider scopeProvider) {
        this._ScopeProvider = scopeProvider;
    }

    internal void Log<TState>(LogEntry<TState> logEntry) {
        if (this.IsEnabled) {
            this._ReactiveLoggerSource.Log(logEntry);
            //this._ReactiveLoggerSource.SourceLogEntry.OnNext(logEntry);
        }
    }
}
