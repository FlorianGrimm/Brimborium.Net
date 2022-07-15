namespace Brimborium.Configuration.Test;

public class HugoOptions {
    public string? Gna { get; set; }
}

public class HugoService {
    private readonly IOptionsMonitor<HugoOptions> _OptionsMonitor;
    private HugoOptions _Options;

    public HugoService(IOptionsMonitor<HugoOptions> options) {
        this._OptionsMonitor = options;
        this._OptionsMonitor.OnChange(this.Refresh);
        this._Options = this._OptionsMonitor.CurrentValue;
    }

    public HugoOptions Options => this._Options;

    private void Refresh(HugoOptions nextOptions, string name) {
        this._Options = nextOptions;
    }
}
