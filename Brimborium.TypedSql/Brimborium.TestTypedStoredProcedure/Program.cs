
using Microsoft.Extensions.Configuration;

using System.Collections.Generic;

namespace Brimborium.TestTypedStoredProcedure;

public static class Program {
    public static string ConnectionString = "Data Source=.;Initial Catalog=TestDB;Trusted_Connection=True;";

    public static void Main(string[] args) {
        //var configuration = GetConfiguration(args);
        //MainGenerate(configuration);
    }
    //public static IConfigurationRoot GetConfiguration(string[] args) {
    //    var configurationBuilder = new ConfigurationBuilder();
    //    // configurationBuilder.AddJsonFile(@"c:\secure\appsettings.TestGenerateStoredProcedure.json", true);
    //    configurationBuilder.AddCommandLine(args).AddUserSecrets(assembly: typeof(Program).Assembly, optional: true);
    //    var configuration = configurationBuilder.Build();
    //    return configuration;
    //}

    //public static void MainGenerate(IConfigurationRoot configuration) {
    //    var connectionString = configuration.GetValue<string>("ConnectionString")
    //        ?? configuration.GetValue<string>("App:SQLConnectionString");

    //    var filePathGenerated = GetFilePathGenerated()
    //    System.Console.Out.WriteLine(filePathGenerated);

    //    //var outputFolder = GetFilePathGenerated().Replace(
    //    //    @"src\GenerateStoredProcedure\Program.cs",
    //    //    @"src\Solvin.OneTS.Database");
    //    //var cfg = Configuration.GetDefintion();
    //    //var templateVariables = new Dictionary<string, string>();
    //    //Generator.GenerateSql(connectionString, outputFolder, cfg, templateVariables);
    //}

    //public static string GetFilePathGenerated() {
    //    return getFilePathGenerated() ?? string.Empty;
    //    static string? getFilePathGenerated([System.Runtime.CompilerServices.CallerFilePath] string? fp = default) => fp;
    //}
}
