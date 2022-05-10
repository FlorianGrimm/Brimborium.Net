namespace Brimborium.RowVersion.API;

public interface IDataAPIWithVersion {
    string DataVersion { get; }
}

public record class DataAPIWithVersionRecord(
    string DataVersion
    ): IDataAPIWithVersion;

public class DataAPIWithVersionClass : IDataAPIWithVersion {
    public DataAPIWithVersionClass() {
        this.DataVersion = string.Empty;
    }

    public string DataVersion { get; set; }
}
