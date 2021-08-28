using System;

namespace Brimborium.CodeBlocks
{
    class Program
    {
        static int Main(string[] args) {
            if (args.Length == 1 && args[0]=="install") {
                Console.WriteLine($"Brimborium.CodeBlocks {ThisAssembly.AssemblyInformationalVersion}");
                Console.WriteLine($"tool install");

                var p =System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
                    "dotnet.exe"
                    ) {
                    ArgumentList = { 
                        "tool",
                        "install",
                        "--tool-path",
                        "dotnetcodeblocks",
                        "--add-source",
                        "./nupkg",
                        "--version",
                        ThisAssembly.AssemblyInformationalVersion, //"1.0.5-beta-ga769bb6dcc",
                        "dotnetcodeblocks"
                    }
                });
                p.WaitForExit();
                return 0;
            }
            if (args.Length == 1 && args[0] == "uninstall") {
                Console.WriteLine($"Brimborium.CodeBlocks {ThisAssembly.AssemblyInformationalVersion}");
                Console.WriteLine($"tool install");

                var p = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
                    "dotnet.exe"
                    ) {
                    ArgumentList = {
                        "tool",
                        "uninstall",
                        "--tool-path",
                        "dotnetcodeblocks",
                        "dotnetcodeblocks"
                    }
                });
                p.WaitForExit();
                return 0;
            }
            Console.WriteLine($"Brimborium.CodeBlocks {ThisAssembly.AssemblyInformationalVersion}");
            
            Console.WriteLine("Hello World!");
            return 0;
        }
    }
}
