namespace Brimborium.RowVersion.Extensions;
public static class DataVersionExtensions {
    public static bool DataVersionIsEmptyOrZero(string? dataversion) {
        if (string.IsNullOrEmpty(dataversion)) {
            return true;
        } else {
            return string.CompareOrdinal("0", dataversion) == 0;
        }
    }

    public static string ToDataVersion(long entityVersion) {
        return entityVersion.ToString("x16");
    }

    public static long ToEntityVersion(string? dataVersion) {
        if (string.IsNullOrEmpty(dataVersion)) {
            return 0;
        }
        if (long.TryParse(dataVersion, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var entityVersion)) {
            return entityVersion;
        }

        return -1;
    }
}