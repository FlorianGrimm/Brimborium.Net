namespace Brimborium.Configuration;

public class SqlConfigurationSource : IConfigurationSource {
    private readonly string _ConnectionString;
    private readonly ISqlConfigurationAccess _SqlConfigurationAccess;

    public SqlConfigurationSource(
        string connectionString,
        ISqlConfigurationAccess sqlConfigurationAccess
        ) {
        this._ConnectionString = connectionString;
        this._SqlConfigurationAccess = sqlConfigurationAccess;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder) 
        => new SqlConfigurationProvider(
            this._ConnectionString,
            this._SqlConfigurationAccess);
}
