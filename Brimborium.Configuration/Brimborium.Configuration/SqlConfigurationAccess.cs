namespace Brimborium.Configuration;

public class SqlConfigurationAccess 
    : ISqlConfigurationAccess
    {
    ConfigurationReloadToken _reloadToken;
    private ISqlConfigurationProviderSink _Sink;

    public SqlConfigurationAccess() {
        this._Sink = new SqlConfigurationProviderNullSink();
        this._reloadToken = new ConfigurationReloadToken();
    }

    public IChangeToken Watch(ISqlConfigurationProviderSink sink) {
        this._Sink = sink;
        return this._reloadToken;
    }

    public virtual bool Load(string connectionString, Dictionary<string, string?> data) => false;

    
    public void OnReload() {
        ConfigurationReloadToken previousToken = System.Threading.Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken());
        previousToken.OnReload();
    }
}
