namespace Brimborium.RowVersion.Extensions;

internal static class MD5Extension {
    private static MD5? _MD5;
    
    internal static string GetMD5HashFromByteArray(byte[] content) {
        var md5 = _MD5 ??= MD5.Create();
        return Convert.ToBase64String(md5.ComputeHash(content));
    }
}
