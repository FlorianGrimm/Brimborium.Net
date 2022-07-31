namespace Brimborium.RowVersion.Extensions;

public static class DataVersionExtensions {
    static Regex? _Regex0;

    /// <summary>
    /// Determines if the string is 0+
    /// </summary>
    /// <param name="dataversion"></param>
    /// <returns></returns>
    public static bool DataVersionIsEmptyOrZero(string? dataversion) {
        if (string.IsNullOrEmpty(dataversion)) {
            return true;
        } else {
            var regex0 = (_Regex0 ??= new Regex("^0{1,16}$", RegexOptions.None));
            return regex0.IsMatch(dataversion);
        }
    }

    /// <summary>
    /// Convert long entityVersion to string dataVersion.
    /// </summary>
    /// <param name="entityVersion">the entityVersion (rowversion in sql)</param>
    /// <returns>the string dataVersion hex 16 chars.</returns>
    public static string ToDataVersion(long entityVersion) {
        return entityVersion.ToString("x16");
    }

    /// <summary>
    /// Convert string dataVersion to long entityVersion.
    /// </summary>
    /// <param name="dataVersion">the string dataVersion hex 16 chars or null or -1.</param>
    /// <returns>
    /// long entityVersion converted from the string dataVersion hex 16 chars 
    /// or 0 if null 
    /// otherwise -1.
    /// </returns>
    public static long ToEntityVersion(string? dataVersion) {
        if (string.IsNullOrEmpty(dataVersion)) {
            return 0;
        } else if (long.TryParse(dataVersion, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var entityVersion)) {
            return entityVersion;
        }

        return -1;
    }

    /// <summary>
    /// Matches the current EntityVersion with new EntityVersion.
    /// </summary>
    /// <param name="currentEntityVersion">the EntityVersion of the current entity.</param>
    /// <param name="newEntityVersion">the EntityVersion of the new entity.</param>
    /// <returns>true if they are the same or newEntityVersion == -1.</returns>
    public static bool EntityVersionDoesMatch(this long currentEntityVersion, long newEntityVersion) {
        return (-1L == newEntityVersion)
            || (currentEntityVersion == newEntityVersion);
    }
}