namespace Brimborium.Configuration;

public interface ISqlConfigurationAccess {
    IChangeToken Watch(ISqlConfigurationProviderSink sink);

    bool Load(string connectionString, Dictionary<string, string?> data);
}
