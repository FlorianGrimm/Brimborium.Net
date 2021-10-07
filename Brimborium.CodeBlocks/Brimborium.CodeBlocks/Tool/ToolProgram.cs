#pragma warning disable IDE0060 // Remove unused parameter

using Brimborium.CodeBlocks.Library;
using Brimborium.CodeBlocks.Tool;

using CommandLine;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Brimborium.CodeBlocks.Tool {
    [Verb("install", Hidden = true, HelpText = "install the tool.")]
    public class InstallOptions {
    }
    [Verb("uninstall", Hidden = true, HelpText = "uninstall the tool.")]
    public class UninstallOptions {
    }

    [Verb("run", isDefault: true, HelpText = "run ")]
    public class RunOptions : IRunOptions {
        //normal options here
        [Option('c', "configuration", Required = false, HelpText = "the configuration")]
        public string? ConfigurationFullPath { get; set; }

        /*
        [Option('w', "watch", Required = false, HelpText = "watch")]
        public bool Watch { get; set; }
        */

        [Option('b', "build", Required = false, HelpText = "build")]
        public bool Build { get; set; }

        [Option('g', "generateonly ", Required = false, HelpText = "generate only do not copy")]
        public bool GenerateOnly { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }

    [Verb("diff", HelpText = "dry run")]
    public class DiffOptions {
        //commit options here
    }

    public class Options {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }

    public class ToolProgram {
        public static async Task<int> Run(string[] args) {
            var parserResult = CommandLine.Parser.Default.ParseArguments<InstallOptions, UninstallOptions, RunOptions, DiffOptions>(args);

            if (parserResult is Parsed<object> parsed) {
                Console.WriteLine($"Brimborium.CodeBlocks {ThisAssembly.AssemblyInformationalVersion}");
                return (parsed.Value) switch {
                    InstallOptions installOptions => await InstallAsync(installOptions),
                    UninstallOptions uninstallOptions => await UninstallAsync(uninstallOptions),
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
            if (p is null) {
                Console.Error.WriteLine("Cannot install.");
                return 1;
            } else {
                await p.WaitForExitAsync();
                return 0;
            }
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
            if (p is null) {
                Console.Error.WriteLine("Cannot install.");
                return 1;
            } else {
                await p.WaitForExitAsync();
                return 0;
            }
        }

        private static async Task<int> RunAsync(RunOptions opts) {
            AppConfiguration appConfiguration;
            try {
                appConfiguration = await SharedToolProgram.GetConfigurationAsync(opts.ConfigurationFullPath, null);
            } catch (System.Exception error) {
                Console.Error.WriteLine(error.Message);
                Console.Error.WriteLine(error.ToString());
                return 1;
            }

            Console.Out.WriteLine($"configurationFile : {appConfiguration.ConfigurationFile}");
            Console.Out.WriteLine($"BaseFolder        : {appConfiguration.BaseFolder}");
            Console.Out.WriteLine($"Project           : {appConfiguration.Project}");
            Console.Out.WriteLine($"Assembly          : {appConfiguration.Assembly}");

            if (opts.Build) {
                if (string.IsNullOrEmpty(appConfiguration.Project)) {
                    Console.Error.WriteLine("");
                    Console.Error.WriteLine("Project is not specified or empty.");
                    var di = new System.IO.DirectoryInfo(appConfiguration.BaseFolder!);
                    var lstcsproj = di.GetFiles("*.csproj", System.IO.SearchOption.AllDirectories);
                    foreach (var fi in lstcsproj) {
                        var relativePath = System.IO.Path.GetRelativePath(appConfiguration.BaseFolder!, fi.FullName);
                        Console.Error.WriteLine($"\"Project\": \"{relativePath}\",");
                    }
                    return 1;
                } else {
                    var psi = new ProcessStartInfo();
                    psi.FileName = "dotnet";
                    psi.ArgumentList.Add("build");
                    psi.ArgumentList.Add(appConfiguration.Project);
                    var process = System.Diagnostics.Process.Start(psi);
                    if (process is null) {
                        Console.Error.WriteLine("Cannot start dotnet build");
                        return 1;
                    } else {
                        await process.WaitForExitAsync();
                        var exitCode = process.ExitCode;
                        if (exitCode == 0) {
                            // OK 
                            Console.Out.WriteLine("dotnet build ends successfully.");
                        } else {
                            // TODO
                            Console.Error.WriteLine($"dotnet build returns {exitCode}");
                            return 1;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(appConfiguration.Assembly)) {
                Console.Error.WriteLine("Assembly is not specified or empty.");
                return 1;
            }
            var fiAssembly = new System.IO.FileInfo(appConfiguration.Assembly);
            if (!fiAssembly.Exists) {
                Console.Error.WriteLine($"Assembly file {appConfiguration.Assembly} does not exists.");
                return 1;
            }

            try {

                System.Reflection.Assembly assembly;
                try {
                    assembly = System.Reflection.Assembly.LoadFrom(appConfiguration.Assembly);
                } catch (System.Exception error) {
                    Console.Error.WriteLine(error.Message);
                    return 1;
                }
                Console.Out.WriteLine($"Assembly {appConfiguration.Assembly} loaded.");


                var dependencyContext = Microsoft.Extensions.DependencyModel.DependencyContext.Load(assembly);

                return await SharedToolProgram.RunAsync(assembly, dependencyContext, appConfiguration, opts);
            } catch (System.Exception error) {
                Console.Error.WriteLine($"Failed");
                Console.Error.WriteLine(error.ToString());
                return 1;
            }
        }
       
        private static async Task<int> DiffAsync(DiffOptions opts) {
            await Task.CompletedTask;
            return 0;
        }
    }
}
