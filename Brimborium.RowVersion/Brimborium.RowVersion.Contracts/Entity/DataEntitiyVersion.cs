using Brimborium.RowVersion.Extensions;

namespace Brimborium.RowVersion.Entity;

public struct DataEntityVersion {
    private long _EntityVersion;

    private string? _DataVersion;

    public DataEntityVersion(long entityVersion) {
        this._EntityVersion = entityVersion;
        this._DataVersion = null;
    }

    public DataEntityVersion(string dataVersion) {
        if (dataVersion.Length == 16) {
            this._EntityVersion = 0;
            this._DataVersion = dataVersion;
        } else {
            if (dataVersion.Length == 0) {
                this._EntityVersion = 0;
                this._DataVersion = null;
            } else {
                var entityVersion = DataVersionExtensions.ToEntityVersion(dataVersion);
                this._EntityVersion = entityVersion;
                this._DataVersion = DataVersionExtensions.ToDataVersion(entityVersion);
            }
        }
    }

    private DataEntityVersion(long entityVersion, string dataVersion) {
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
            return DataVersionExtensions.ToEntityVersion(this._DataVersion);
            //if (long.TryParse(this._DataVersion, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var entityVersion)) {
            //    that = new DataEntityVersion(entityVersion, this._DataVersion);
            //    return entityVersion;
            //} else {
            //    return -1;
            //}
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
            return DataVersionExtensions.ToDataVersion(entryVersion._EntityVersion);
        } else {
            return entryVersion._DataVersion;
        }
    }

    public static implicit operator long(DataEntityVersion entryVersion) {
        if (entryVersion._EntityVersion == 0 && entryVersion._DataVersion is not null) {
            return DataVersionExtensions.ToEntityVersion(entryVersion._DataVersion);
        } else {
            return entryVersion._EntityVersion;
        }
    }
}
