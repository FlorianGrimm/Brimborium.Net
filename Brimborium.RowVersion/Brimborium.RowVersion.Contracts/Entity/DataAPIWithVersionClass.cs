namespace Brimborium.RowVersion.Entity;

public class DataAPIWithVersionClass
    : IEntityWithDataEntityVersion {
    private DataEntityVersion _EntryVersion;

    public DataAPIWithVersionClass() {
    }

    public long EntityVersion {
        get => this._EntryVersion.GetEntityVersion(ref this._EntryVersion);
        set => this._EntryVersion = new DataEntityVersion(value);
    }


    public string DataVersion {
        get => this.DataEntityVersion.GetDataVersion(ref this._EntryVersion);
        set => this._EntryVersion = new DataEntityVersion(value);
    }

    [JsonIgnore]
    public DataEntityVersion DataEntityVersion { get => this._EntryVersion; set => this._EntryVersion = value; }
}
