namespace Brimborium.TestSample.Service;
internal static class SqlAccessLocation {
    internal static (string outputPath, string outputNamespace,string outputClassName) GetPrimaryKeyOutputInfo()
    => (
        outputPath: System.IO.Path.Combine(GetDirectoryPath(), "SqlAccess.Generated.cs"),
        outputNamespace: "Brimborium.TestSample.Service",  //typeof(SqlAccess).Namespace!,
        outputClassName: "SqlAccess"
    );

    private static string GetDirectoryPath() {
        return System.IO.Path.GetDirectoryName(getFilePathGenerated() ?? string.Empty)!;
        static string? getFilePathGenerated([System.Runtime.CompilerServices.CallerFilePath] string? fp = default) => fp;
    }
}
