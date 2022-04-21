namespace Brimborium.TestSample.Record;

internal static class PrimaryKeyLocation {
    internal static (string outputPath, string outputNamespace) GetPrimaryKeyOutputInfo()
        => (
        outputPath: System.IO.Path.Combine(GetDirectoryPath(), "PrimaryKey.cs"),
        outputNamespace: typeof(PrimaryKeyLocation).Namespace!
        );

    private static string GetDirectoryPath() {
        return System.IO.Path.GetDirectoryName(getFilePathGenerated() ?? string.Empty)!;
        static string? getFilePathGenerated([System.Runtime.CompilerServices.CallerFilePath] string? fp = default) => fp;
    }
}
