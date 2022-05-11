namespace Brimborium.Functional;

public static class MD5Extension {
    private static MD5? _MD5;
    private static MD5 GetMD5() {
        return _MD5 ??= MD5.Create();
    }

    public static string GetMD5HashFromString(string content) {
        var b = Encoding.UTF8.GetBytes(content);
        return Convert.ToBase64String(GetMD5().ComputeHash(b));
    }
    public static string GetMD5HashFromByteArray(byte[] content) {
        return Convert.ToBase64String(GetMD5().ComputeHash(content));
    }
}
