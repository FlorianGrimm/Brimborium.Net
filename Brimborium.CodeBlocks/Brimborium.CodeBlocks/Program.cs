using CommandLine;
using CommandLine.Text;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brimborium.CodeBlocks {
    [Verb("install", Hidden = true, HelpText = "install the tool.")]
    public class InstallOptions {
    }
    [Verb("uninstall", Hidden = true, HelpText = "uninstall the tool.")]
    public class UninstallOptions {
    }

    [Verb("run", isDefault: true, HelpText = "run ")]
    public class RunOptions {
        //normal options here
    }

    [Verb("diff", HelpText = "dry run")]
    public class DiffOptions {
        //commit options here
    }

    public class Options {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }

    public class Program {
        public static async Task<int> Main(string[] args) {
            if (args.Length == 1 && args[0] == "install") {
              
            }
            if (args.Length == 1 && args[0] == "uninstall") {
             
            }

            var parserResult = CommandLine.Parser.Default.ParseArguments<InstallOptions, UninstallOptions, RunOptions, DiffOptions>(args);

            if (parserResult is Parsed<object> parsed) {
                Console.WriteLine($"Brimborium.CodeBlocks {ThisAssembly.AssemblyInformationalVersion}");
                return (parsed.Value) switch {
                    InstallOptions installOptions=> await InstallAsync(installOptions),
                    UninstallOptions uninstallOptions=> await  UninstallAsync(uninstallOptions),
                    RunOptions addOptions => await RunAsync(addOptions),
                    DiffOptions commitOptions => await DiffAsync(commitOptions),
                    _ => 1
                };
            }
            return 1;
        }


        private static async Task<int> InstallAsync(InstallOptions opts) {
            Console.WriteLine($"Brimborium.CodeBlocks {ThisAssembly.AssemblyInformationalVersion}");
            Console.WriteLine($"tool install");

            var p = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
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
            await p.WaitForExitAsync();
            return 0;
        }
        private static async Task<int> UninstallAsync(UninstallOptions opts) {
            Console.WriteLine($"Brimborium.CodeBlocks {ThisAssembly.AssemblyInformationalVersion}");
            Console.WriteLine($"tool uninstall");

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
            await p.WaitForExitAsync();
            return 0;
        }
        private static async Task<int> RunAsync(RunOptions opts) {
            await Task.CompletedTask;
            return 0;
        }

        private static async Task<int> DiffAsync(DiffOptions opts) {
            await Task.CompletedTask;
            return 0;
        }
    }
}
