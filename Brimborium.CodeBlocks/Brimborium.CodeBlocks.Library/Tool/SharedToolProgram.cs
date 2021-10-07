#pragma warning disable IDE0060 // Remove unused parameter

using Brimborium.CodeBlocks.Library;
using Brimborium.CodeBlocks.Tool;

using CommandLine;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Brimborium.CodeBlocks.Tool {

    public interface IRunOptions {
        bool GenerateOnly { get; set; }

        bool Verbose { get; set; }
    }

    public static class SharedToolProgram {
        public static async Task<AppConfiguration> GetConfigurationAsync(
            string? configurationFullPath,
            string? optionalConfigurationFolder
            ) {
            System.IO.FileInfo? fiConfiguration = null;
            if (!string.IsNullOrEmpty(configurationFullPath)) {
                fiConfiguration = new System.IO.FileInfo(configurationFullPath);
                if (!fiConfiguration.Exists) {
                    throw new System.Exception($"Configuration file: {configurationFullPath} not found.");
                }
            }
            if (fiConfiguration is null) {
                if (string.IsNullOrEmpty(optionalConfigurationFolder)) {
                    // skip
                } else {
                    var codegenJson = System.IO.Path.Combine(
                        optionalConfigurationFolder,
                        "codegen.json");
                    fiConfiguration = new System.IO.FileInfo(codegenJson);
                    if (fiConfiguration.Exists) {
                        // OK go with that
                    } else {
                        fiConfiguration = null;
                    }
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

        public static async Task<int> RunAsync(
            Assembly assembly,
            DependencyContext dependencyContext,
            AppConfiguration appConfiguration,
            IRunOptions opts) {

            var classNameStartup = $"{assembly.GetName().Name}.Startup";

            var builder = new ConfigurationBuilder();
            var appsettingsJson = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(appConfiguration.Assembly!)!, "appsettings.json");
            builder.AddJsonFile(appsettingsJson, optional: true, reloadOnChange: false);
            var dictConfiguration = new Dictionary<string, string>();
            dictConfiguration["Logging:LogLevel:Default"] = opts.Verbose ? nameof(LogLevel.Debug) : nameof(LogLevel.Information);
            builder.AddInMemoryCollection(dictConfiguration);

            var configuration = builder.Build();

            var templateProvider = new CBTemplateProvider();
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

            if (typeStartup is not null) {
                serviceCollection.AddSingleton(typeStartup);
            }
            serviceCollection.AddSingleton<CBTemplateProvider>(templateProvider);
            serviceCollection.AddSingleton<AppConfiguration>(appConfiguration);
            serviceCollection.AddSingleton<ToolService>((sp) => {
                var baseFileSystem = new FileSystemService(appConfiguration.BaseFolder!, sp.GetRequiredService<ILoggerFactory>().CreateLogger("BaseFolder"));
                var tempFileSystem = new FileSystemService(appConfiguration.TempFolder!, sp.GetRequiredService<ILoggerFactory>().CreateLogger("TempFolder"));
                tempFileSystem.CreateDirectory("");
                var toolService = new ToolService(baseFileSystem, tempFileSystem, sp.GetRequiredService<ILoggerFactory>().CreateLogger("ToolService"));
                return toolService;
            });
            serviceCollection.AddServicesWithRegistrator(a => {
                a.FromAssemblyDependencies(dependencyContext, assembly)
                    .AddClasses()
                    .UsingAttributes();
                a.FromAssemblyDependencies(dependencyContext, assembly)
                    .AddClasses(a => a.Where(c => typeof(ICodeGenTask).IsAssignableFrom(c)))
                    .AsSelfWithInterfaces()
                    .WithTransientLifetime();
                a.FromAssemblyDependencies(dependencyContext, assembly)
                    .AddClasses(a => a.Where(c => typeof(ICBNamedTemplate).IsAssignableFrom(c)))
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
                var lstNamedTemplate = serviceProvider.GetServices<ICBNamedTemplate>();
                foreach (var namedTemplate in lstNamedTemplate) {
                    if (namedTemplate is CBTemplate template) {
                        templateProvider.AddTemplate(template, namedTemplate.Language, namedTemplate.Name);
                    }
                }

                var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Program");
                var codeGenTasks = serviceProvider.GetServices<ICodeGenTask>()
                    .OrderBy(t => t.GetStep()).ToList();

                if (codeGenTasks.Count == 0) {
                    Console.Error.WriteLine("Cannot find any ICodeGenTask s");
                    return 1;
                } else {
                    foreach (var codeGenTask in codeGenTasks) {
                        logger.LogInformation("Step {step}", codeGenTask.GetType().Name);
                        try {
                            codeGenTask.Execute();
                        } catch (System.Exception otherError) {
                            logger.LogError(otherError, "Step {step} failed", codeGenTask.GetType().Name);
                        }
                    }
                }

                serviceProvider.GetRequiredService<ToolService>().CopyReplace(opts.GenerateOnly);
                logger.LogInformation("Done");
            }
            return 0;
        }
    }
}
