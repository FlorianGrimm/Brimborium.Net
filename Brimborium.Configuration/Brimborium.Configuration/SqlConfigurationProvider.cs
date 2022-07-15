namespace Brimborium.Configuration;

public class SqlConfigurationProvider
    : ConfigurationProvider
    , ISqlConfigurationProviderSink
    , IDisposable
    {
    private readonly IDisposable _changeTokenRegistration;

    private string _ConnectionString;
    private readonly ISqlConfigurationAccess _SqlConfigurationAccess;

    public SqlConfigurationProvider(
        string connectionString,
        ISqlConfigurationAccess sqlConfigurationAccess
        ) {
        this._ConnectionString = connectionString;
        this._SqlConfigurationAccess = sqlConfigurationAccess;
        this._changeTokenRegistration = ChangeToken.OnChange(
            () => this._SqlConfigurationAccess.Watch(this),
            () => {
                //Thread.Sleep(Source.ReloadDelay);
                Reload();
            });
        // IOptionsChangeTokenSource
    }

    public override void Load() {
        this.LoadOrReload(false);
    }
    public void Reload() {
        this.LoadOrReload(true);
    }

    private void LoadOrReload(bool reload) {
        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        if (this._SqlConfigurationAccess.Load(this._ConnectionString, data)) {
            this.Data = data;
            if (reload) {
                //var a = this.GetReloadToken();
                base.OnReload();
                //var b = this.GetReloadToken();
            }
        }
    }

    public void Dispose() {
        this._changeTokenRegistration.Dispose();
    }
}
