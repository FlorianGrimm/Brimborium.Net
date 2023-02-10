namespace Brimborium.SourceGenerator.BaseMethods.Test;

public class AttributeSourceHelper {
    private static string getLocation([CallerFilePath] string path = "") => path;
    private static string[]? _BaseMethodsSources;
    public static string[] getBaseMethodsSources() {
        if (_BaseMethodsSources is null) {
            var folderPath = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(
                    getLocation(),
                    @"..\..\Brimborium.BaseMethods"
                )
                );
            _BaseMethodsSources =
                System.IO.Directory.GetFiles(folderPath, "*.cs", System.IO.SearchOption.TopDirectoryOnly)
                    .Select(System.IO.File.ReadAllText)
                    .ToArray();
        }
        return _BaseMethodsSources;
    }
    private static string? _AttributeSource;
    public static string getAttributeSource() {
        if (_AttributeSource is null) {
            var attributesPath = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(
                    getLocation(),
                    @"..\..\Brimborium.BaseMethods\Attributes.cs"
                )
                );
            _AttributeSource = System.IO.File.ReadAllText(attributesPath);
        }
        return _AttributeSource;
    }
    //
    private static string? _GlobalUsingsSource;
    public static string getGlobalUsingsSource() {
        if (_GlobalUsingsSource is null) {
            var GlobalUsingssPath = Path.GetFullPath(
                Path.Combine(
                    getLocation(),
                    @"..\..\Brimborium.BaseMethods\GlobalUsings.cs"
                )
                );
            _GlobalUsingsSource = File.ReadAllText(GlobalUsingssPath);
        }
        return _GlobalUsingsSource;
    }
}
