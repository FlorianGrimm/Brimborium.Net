using Brimborium.TypedStoredProcedure;

using Microsoft.Extensions.Configuration;

using System.Collections.Generic;

namespace Brimborium.TestTypedStoredProcedure;

public static partial class Program {
    //public static string DefaultConnectionString = "";
    //public static string DefaultConnectionString = "Data Source=parado.dev.solvin.local;Initial Catalog=TodoDB;Trusted_Connection=True;";
    public static string DefaultConnectionString = "Data Source=.;Initial Catalog=TestDB;Trusted_Connection=True;";

    public static int Main(string[] args) {
        try {
            var configuration = GetConfiguration(args);

            AddNativeTypeConverter();
            //MainEnsureProcedure();
            //MainGeneratePrimaryKey(configuration);
            //MainGenerateOneTSSqlAccess(configuration);

            //var connectionString = configuration.GetValue<string>("ConnectionString");
            //var outputFolder = configuration.GetValue<string>("OutputFolder")
            //    ?? configuration.GetValue<string>("App:OutputFolder");
            //var sqlProjectName = configuration.GetValue<string>("SqlProject");

            //if (string.IsNullOrEmpty(connectionString)) {
            //    connectionString = DefaultConnectionString;
            //}

            //var upperDirectoryPath = GetUpperDirectoryPath();

            //if (string.IsNullOrEmpty(outputFolder)) {
            //    outputFolder = "Brimborium.TestSqlDatabase"; // change this
            //}
            //if (string.IsNullOrEmpty(sqlProjectName)) {
            //    sqlProjectName = "Brimborium.TestSqlDatabaseDeploy"; // change this
            //}

            //if (string.IsNullOrEmpty(outputFolder)) {
            //    System.Console.Error.WriteLine("outputFolder is empty");
            //    return 1;
            //} else {
            //    if (!System.IO.Path.IsPathFullyQualified(outputFolder)) {
            //        outputFolder = System.IO.Path.Combine(upperDirectoryPath, outputFolder);
            //    }
            //    var changes = MainGenerate(connectionString, outputFolder);
            //    if (changes) {
            //        System.Console.Out.WriteLine($"changes try to update.");

            //        var dotnet = Brimborium.GenerateStoredProcedure.Utility.TryGetDotNetPath();
            //        if (dotnet is null) {
            //            System.Console.Error.WriteLine("dotnet not found");
            //            return 1;
            //        }
            //        var sqlProject_csproj = System.IO.Path.Combine(
            //            upperDirectoryPath,
            //            sqlProjectName,
            //            $"{sqlProjectName}.csproj" //sqlproj
            //            );
            //        var sqlProjectDirectory = System.IO.Path.Combine(
            //                upperDirectoryPath,
            //                sqlProjectName
            //                );
            //        {
            //            var psi = new System.Diagnostics.ProcessStartInfo(
            //                dotnet, $"build \"{sqlProject_csproj}\"");
            //            psi.WorkingDirectory = sqlProjectDirectory;
            //            var process = System.Diagnostics.Process.Start(psi);
            //            if (process is not null) {
            //                process.WaitForExit(30_000);
            //                if (process.ExitCode == 0) {
            //                    System.Console.Out.WriteLine($"dotnet build {sqlProject_csproj} OK");
            //                } else {
            //                    System.Console.Error.WriteLine($"dotnet build {sqlProject_csproj} Failed");
            //                    return 1;
            //                }
            //            }
            //        }
            //        {
            //            var csb = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
            //            var p1 = (string.IsNullOrEmpty(csb.DataSource) || string.IsNullOrEmpty(csb.InitialCatalog)) ? string.Empty :
            //                $"/p:TargetServerName=\"{csb.DataSource}\" /p:TargetDatabaseName=\"{csb.InitialCatalog}\"";
            //            var p2 = (string.IsNullOrEmpty(csb.UserID) || string.IsNullOrEmpty(csb.Password)) ? string.Empty :
            //                $"/p:TargetUser=\"{csb.UserID} /p:TargetPassword=\"{csb.Password} ";
            //            var psi = new System.Diagnostics.ProcessStartInfo(
            //                dotnet,
            //                $"publish \"{sqlProject_csproj}\" {p1} {p2}");
            //            psi.WorkingDirectory = sqlProjectDirectory;
            //            var process = System.Diagnostics.Process.Start(psi);
            //            if (process is not null) {
            //                process.WaitForExit(30_000);
            //                if (process.ExitCode == 0) {
            //                    System.Console.Out.WriteLine($"dotnet build {sqlProject_csproj} OK");
            //                } else {
            //                    System.Console.Out.WriteLine($"dotnet build {sqlProject_csproj} Failed");
            //                    return 1;
            //                }
            //            }
            //        }
            //        return 0;
            //    } else {
            //        System.Console.Out.WriteLine($"no changes.");
            //        return 0;
            //    }
            //}
            return 0;
        } catch (System.Exception error) {
            System.Console.Error.WriteLine(error.ToString());
            return 1;
        }
    }

    public static IConfigurationRoot GetConfiguration(string[] args) {
        var configurationBuilder = new ConfigurationBuilder();
        // configurationBuilder.AddJsonFile(@"c:\secure\appsettings.TestTypedStoredProcedure.json", true);
        configurationBuilder.AddCommandLine(args).AddUserSecrets(assembly: typeof(Program).Assembly, optional: true);
        var configuration = configurationBuilder.Build();
        return configuration;
    }


    private static void AddNativeTypeConverter() {
        SQLUtility.AddDefaultTypeConverter();
        //SQLUtility.AddTypeConverter(
        //    typeof(int),
        //    typeof(bool),
        //    typeof(Solvin.OneTS.Services.SqlHelpers.Int32Converter),
        //    nameof(Solvin.OneTS.Services.SqlHelpers.Int32Converter.ToBool));
    }

    //public static bool MainGenerate(string connectionString, string outputFolder) {
    //    var templateVariables = new Dictionary<string, string>();
    //    //var cfg = new GenerateConfiguration();
    //    //return Brimborium.GenerateStoredProcedure.Generator.GenerateSql(
    //    //    connectionString,
    //    //    outputFolder,
    //    //    cfg,
    //    //    templateVariables);
    //    return true;
    //}

    //public static string GetUpperDirectoryPath() {
    //    return System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(getFilePathGenerated() ?? string.Empty)!)!;
    //    static string? getFilePathGenerated([System.Runtime.CompilerServices.CallerFilePath] string? fp = default) => fp;
    //}
}