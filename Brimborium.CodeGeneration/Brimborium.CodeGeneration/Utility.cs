
namespace Brimborium.CodeGeneration {
    public static class Utility {
        // https://github.com/dotnet/roslyn/blob/main/src/Tools/Source/RunTests/Options.cs
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static string? TryGetDotNetPath() {
            var dir = RuntimeEnvironment.GetRuntimeDirectory();
            var programName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "dotnet.exe" : "dotnet";

            while (dir != null && !File.Exists(Path.Combine(dir, programName))) {
                dir = Path.GetDirectoryName(dir);
            }

            return dir == null ? null : Path.Combine(dir, programName);
        }
    }
}
