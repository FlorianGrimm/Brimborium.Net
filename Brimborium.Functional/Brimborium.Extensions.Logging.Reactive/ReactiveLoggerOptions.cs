namespace Brimborium.Extensions.Logging.Reactive;

public class ReactiveLoggerOptions {
    public bool IsEnabled { get; set; }

    public bool IncludeScopes { get; set; }
    
    public ReactiveLoggerOptions() { 
        this.IsEnabled = true;
    }

}
