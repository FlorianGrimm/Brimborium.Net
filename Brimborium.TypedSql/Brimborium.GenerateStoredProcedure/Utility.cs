using System.IO;
using System.Runtime.InteropServices;

namespace Brimborium.GenerateStoredProcedure {
    public static class Utility {
        // https://github.com/dotnet/roslyn/blob/main/src/Tools/Source/RunTests/Options.cs
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
