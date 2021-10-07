#pragma warning disable IDE0060 // Remove unused parameter

using Brimborium.CodeBlocks.Tool;

using CommandLine;

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Brimborium.CodeBlocks.Inplace {

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

    public static class InplaceProgram {
        public static async Task<int> Run(
            string[] args, 
            Assembly assembly,
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = ""
            ) {
            var parserResult = Parser.Default.ParseArguments<RunOptions, DiffOptions>(args);

            if (parserResult is Parsed<object> parsed) {
                Console.WriteLine($"Brimborium.CodeBlocks {ThisAssembly.AssemblyInformationalVersion}");
                return parsed.Value switch {
                    RunOptions addOptions => await RunAsync(addOptions, assembly, sourceFilePath),
                    DiffOptions commitOptions => await DiffAsync(commitOptions),
                    _ => 1
                };
            }
            return 1;
        }
        private static async Task<int> RunAsync(RunOptions opts, Assembly assembly, string sourceFilePath) {
            AppConfiguration appConfiguration;
            try {
                
                appConfiguration = await SharedToolProgram.GetConfigurationAsync(
                    opts.ConfigurationFullPath,
                    System.IO.Path.GetDirectoryName(sourceFilePath)
                    );
            } catch (Exception error) {
                Console.Error.WriteLine(error.Message);
                Console.Error.WriteLine(error.ToString());
                return 1;
            }

            Console.Out.WriteLine($"configurationFile : {appConfiguration.ConfigurationFile}");
            Console.Out.WriteLine($"BaseFolder        : {appConfiguration.BaseFolder}");
            //Console.Out.WriteLine($"Project           : {appConfiguration.Project}");
            //Console.Out.WriteLine($"Assembly          : {appConfiguration.Assembly}");
            try {
                var dependencyContext = Microsoft.Extensions.DependencyModel.DependencyContext.Default;
                return await SharedToolProgram.RunAsync(assembly, dependencyContext, appConfiguration, opts);
            } catch (Exception error) {
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
