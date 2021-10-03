using CommandLine;
using CommandLine.Text;

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Brimborium.CodeBlocks.Tool;
using Brimborium.Registrator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;

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

    public class Program {
        public static async Task<int> Main(string[] args) {
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
                appConfiguration = await GetConfigurationAsync(opts.ConfigurationFullPath);
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
                    Console.Error.WriteLine("Project is not specified or empty.");
                } else {
                    var psi = new ProcessStartInfo();
                    psi.FileName = "dotnet";
                    psi.ArgumentList.Add("build");
                    psi.ArgumentList.Add(appConfiguration.Project);
                    var process = System.Diagnostics.Process.Start(psi);
                    if (process is null) {
                        // TODO
                        Console.Error.WriteLine("TODO process");
                        return 1;
                    } else {
                        await process.WaitForExitAsync();
                        var exitCode = process.ExitCode;
                        if (exitCode == 0) {
                            // OK 
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

                var dependencyContext = Microsoft.Extensions.DependencyModel.DependencyContext.Load(assembly);
                //var loader = new Microsoft.Extensions.DependencyModel.DependencyContextLoader();
                //loader.Load(assembly);
                var classNameStartup = $"{assembly.GetName().Name}.Startup";

                var builder = new ConfigurationBuilder();
                var appsettingsJson = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(appConfiguration.Assembly!)!, "appsettings.json");
                builder.AddJsonFile(appsettingsJson, optional: true, reloadOnChange: false);
                var dictConfiguration = new Dictionary<string, string>();
                dictConfiguration["Logging:LogLevel:Default"] = opts.Verbose ? nameof(LogLevel.Debug) : nameof(LogLevel.Information);
                builder.AddInMemoryCollection(dictConfiguration);

                var configuration = builder.Build();

                var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
                var typeStartup = assembly.GetTypes().FirstOrDefault(type => string.Equals(type.FullName, classNameStartup, StringComparison.OrdinalIgnoreCase));

                serviceCollection.AddLogging((loggingBuider) => {
                    loggingBuider.AddConfiguration(configuration.GetSection("Logging"));
                    loggingBuider.AddConsole();
                });

                if (string.IsNullOrEmpty(appConfiguration.BaseFolder)) {
                    Console.Error.WriteLine($"configuration BaseFolder is not set.");
                    return 1;
                }
                if (string.IsNullOrEmpty(appConfiguration.TempFolder)) {
                    Console.Error.WriteLine($"configuration TempFolder is not set.");
                    return 1;
                }
                var baseFileSystem = new FileSystemService(appConfiguration.BaseFolder!);
                var tempFileSystem = new FileSystemService(appConfiguration.TempFolder!);
                tempFileSystem.CreateDirectory("");
                var toolService = new ToolService(baseFileSystem, tempFileSystem);
                if (typeStartup is not null) {
                    serviceCollection.AddSingleton(typeStartup);
                }
                serviceCollection.AddSingleton<AppConfiguration>(appConfiguration);
                serviceCollection.AddSingleton<ToolService>(toolService);
                serviceCollection.AddServicesWithRegistrator(a => {
                    a.FromAssemblyDependencies(dependencyContext, assembly)
                        .AddClasses()
                        .UsingAttributes();
                    a.FromAssemblyDependencies(dependencyContext, assembly)
                        .AddClasses(a => a.Where(c => typeof(ICodeGenTask).IsAssignableFrom(c)))
                        .AsSelfWithInterfaces()
                        .WithTransientLifetime();
                });

                if (typeStartup is not null) {
                    using var serviceProvider = serviceCollection.BuildServiceProvider();

                    var startup = (IStartup)serviceProvider.GetRequiredService(typeStartup);
                    startup.ConfigureServices(serviceCollection);
                }

                {
                    using var serviceProvider = serviceCollection.BuildServiceProvider();
                    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                    var codeGenTasks = serviceProvider.GetServices<ICodeGenTask>()
                        .OrderBy(t => t.GetStep()).ToList();

                    if (codeGenTasks.Count == 0) {
                        Console.Error.WriteLine("Cannot find any ICodeGenTask s");
                        return 1;
                    } else {
                        foreach (var codeGenTask in codeGenTasks) {
                            logger.LogInformation("Step {step}",codeGenTask.GetType().Name);
                            try {
                                codeGenTask.Execute();
                            } catch (System.Exception otherError) {
                                logger.LogError(otherError, "Step {step} failed", codeGenTask.GetType().Name);
                            }
                        }
                    }

                    if (opts.GenerateOnly) {
                        logger.LogInformation("GenerateOnly -- skip CopyReplace");
                    } else {
                        logger.LogInformation("CopyReplace");
                        tempFileSystem.CopyReplace();
                    }
                }
                return 0;
            } catch (System.Exception error) {
                Console.Error.WriteLine($"Failed");
                Console.Error.WriteLine(error.ToString());
                return 1;
            }
        }

        private static async Task<AppConfiguration> GetConfigurationAsync(string? configurationFullPath) {
            System.IO.FileInfo? fiConfiguration = null;
            if (!string.IsNullOrEmpty(configurationFullPath)) {
                fiConfiguration = new System.IO.FileInfo(configurationFullPath);
                if (!fiConfiguration.Exists) {
                    throw new System.Exception($"Configuration file: {configurationFullPath} not found.");
                }
            }
            if (fiConfiguration is null) {
                fiConfiguration = new System.IO.FileInfo("codegen.json");
            }
            AppConfiguration configuration = new AppConfiguration();
            if (fiConfiguration is not null && fiConfiguration.Exists) {
                using var stream = fiConfiguration.OpenRead();

                configuration = await System.Text.Json.JsonSerializer.DeserializeAsync<Brimborium.CodeBlocks.AppConfiguration>(
                    stream,
                    new System.Text.Json.JsonSerializerOptions() {
                        AllowTrailingCommas = true,
                        ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip
                    }) ?? new AppConfiguration();
            }
            var configurationFile = fiConfiguration!.FullName;
            configuration.ConfigurationFile = configurationFile;
            var configurationDirectory = System.IO.Path.GetDirectoryName(configurationFile)!;
            if (string.IsNullOrEmpty(configuration.BaseFolder)) {
                configuration.BaseFolder = configurationDirectory;
            } else {
                if (System.IO.Path.IsPathFullyQualified(configuration.BaseFolder)) {
                } else {
                    configuration.BaseFolder = System.IO.Path.GetFullPath(System.IO.Path.Combine(configurationDirectory, configuration.BaseFolder));
                }
            }
            if (!string.IsNullOrEmpty(configuration.Assembly)) {
                configuration.Assembly = System.IO.Path.GetFullPath(System.IO.Path.Combine(configuration.BaseFolder, configuration.Assembly));
            }
            if (!string.IsNullOrEmpty(configuration.Project)) {
                configuration.Project = System.IO.Path.GetFullPath(System.IO.Path.Combine(configuration.BaseFolder, configuration.Project));
            }
            if (!string.IsNullOrEmpty(configuration.TempFolder)) {
                configuration.TempFolder = System.IO.Path.GetFullPath(System.IO.Path.Combine(configuration.BaseFolder, configuration.TempFolder));
            } else {
                configuration.TempFolder = System.IO.Path.GetFullPath(System.IO.Path.Combine(configuration.BaseFolder, "output\\codegen"));
            }
            return configuration;
        }

        private static async Task<int> DiffAsync(DiffOptions opts) {
            await Task.CompletedTask;
            return 0;
        }
    }
}
