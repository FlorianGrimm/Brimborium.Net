namespace Brimborium.RowVersion.Entity;

public struct DataEntityVersion {
    private long _EntityVersion;

    private string? _DataVersion;

    public DataEntityVersion(long entityVersion) {
        this._EntityVersion = entityVersion;
        this._DataVersion = null;
    }

    public DataEntityVersion(string dataVersion) {
        this._EntityVersion = 0;
        this._DataVersion = dataVersion;
    }

    public DataEntityVersion(long entityVersion, string dataVersion) {
        this._EntityVersion = entityVersion;
        this._DataVersion = dataVersion;
    }

    public string GetDataVersion(ref DataEntityVersion that) {
        if (this._DataVersion is null) {
            var result = this._EntityVersion.ToString("x16");
            that = new DataEntityVersion(this._EntityVersion, result);
            return result;
        } else {
            return this._DataVersion;
        }
    }

    public long GetEntityVersion(ref DataEntityVersion that) {
        if (this._EntityVersion == 0 && this._DataVersion is not null) {
            if (long.TryParse(this._DataVersion, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var entityVersion)) {
                that = new DataEntityVersion(entityVersion, this._DataVersion);
                return entityVersion;
            } else {
                return 0;
            }
        } else { 
            return this._EntityVersion;
        }
    }

    public static implicit operator DataEntityVersion(long entityVersion)
        => new DataEntityVersion(entityVersion);

    public static implicit operator DataEntityVersion(string dataVersion)
        => new DataEntityVersion(dataVersion);


    public static implicit operator string(DataEntityVersion entryVersion) {
        if (entryVersion._DataVersion is null) {
            return entryVersion._EntityVersion.ToString("x16");
        } else { 
            return entryVersion._DataVersion;
        }
    }

    public static implicit operator long(DataEntityVersion entryVersion) {
        if (entryVersion._EntityVersion == 0 && entryVersion._DataVersion is not null) {
            if (long.TryParse(entryVersion._DataVersion, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var dataVersion)) {
                return dataVersion;
            } else {
                return 0;
            }
        } else {
            return entryVersion._EntityVersion;
        }
    }
}
