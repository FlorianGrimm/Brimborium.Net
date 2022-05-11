namespace Brimborium.RowVersion.Extensions;

public static class DataVersionExtensions {
    static Regex? _Regex0;
    public static bool DataVersionIsEmptyOrZero(string? dataversion) {
        if (string.IsNullOrEmpty(dataversion)) {
            return true;
        } else {
            var regex0 = (_Regex0 ??= new Regex("^0+$", RegexOptions.None));
            return regex0.IsMatch(dataversion);
        }
    }

    public static string ToDataVersion(long entityVersion) {
        return entityVersion.ToString("x16");
    }

    public static long ToEntityVersion(string? dataVersion) {
        if (string.IsNullOrEmpty(dataVersion)) {
            return 0;
        } else if (long.TryParse(dataVersion, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var entityVersion)) {
            return entityVersion;
        }

        return -1;
    }

    public static bool EntityVersionDoesMatch(this long currentEntityVersion, long newEntityVersion) {
        return (0 == newEntityVersion)
            || (currentEntityVersion == newEntityVersion);
    }
}