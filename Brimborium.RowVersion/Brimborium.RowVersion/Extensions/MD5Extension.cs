namespace Brimborium.RowVersion.Extensions;

public static class MD5Extension {
    private static MD5? _MD5;
    public static string GetMD5HashFromString(string content) {
        var md5 = _MD5 ??= MD5.Create();
        var b = Encoding.UTF8.GetBytes(content);
        return Convert.ToBase64String(md5.ComputeHash(b));
    }
    public static string GetMD5HashFromByteArray(byte[] content) {
        var md5 = _MD5 ??= MD5.Create();
        return Convert.ToBase64String(md5.ComputeHash(content));
    }
}
