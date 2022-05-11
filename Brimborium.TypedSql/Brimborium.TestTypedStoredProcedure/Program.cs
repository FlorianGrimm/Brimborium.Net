using Brimborium.TypedStoredProcedure;

using Microsoft.Extensions.Configuration;

using System.Collections.Generic;
using System.Linq;

namespace Brimborium.TestTypedStoredProcedure;

public static partial class Program {
    public static int Main(string[] args) {
        try {
            var configuration = GetConfiguration(args);
            var connectionString = configuration.GetValue<string>("ConnectionString");

            if (string.IsNullOrEmpty(connectionString)) {
                System.Console.Error.WriteLine("ConnectionString is empty");
                return 1;
            }
            //
            System.Console.Out.WriteLine(connectionString);

            AddNativeTypeConverter();
            
            { 
                //MainEnsureProcedure();
            }

#if false
            {
                var (outputFilePrimaryKey, outputNamespacePrimaryKey) = Brimborium.TestSample.Record.PrimaryKeyLocation.GetPrimaryKeyOutputInfo();
                MainGeneratePrimaryKey(connectionString, outputFilePrimaryKey, outputNamespacePrimaryKey);
            }
#endif

#if true
            { 
                var defintions = GetDefintion();

                var(outputPath, outputNamespace, outputClassName) = Brimborium.TestSample.Service.SqlAccessLocation.GetPrimaryKeyOutputInfo();
                //var outputFolder = System.IO.Path.GetDirectoryName(outputPath);

                MainGenerateSqlAccess(connectionString, defintions, outputPath, outputNamespace, outputClassName);
            }
#endif

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

    private static void MainGeneratePrimaryKey(
        string connectionString,
        string outputFilePrimaryKey,
        string outputNamespacePrimaryKey
        ) {
        var printClass = new PrintClass(outputNamespacePrimaryKey, string.Empty);
        Generator.GenerateModel(connectionString, outputFilePrimaryKey, printClass);
    }

    private static void MainGenerateSqlAccess(
        string connectionString, 
        DatabaseDefintion dbDefs,
        string outputPath,
        string outputNamespace, 
        string outputClassName
        ) {
        var types=typeof(Brimborium.TestSample.Record.PrimaryKeyLocation).Assembly.GetTypes()
            .Where(t => t.Namespace == "Brimborium.TestSample.Record")
            .ToArray()
            ;
        var printClass = new PrintClass(
            outputNamespace,
            outputClassName
            );
        Generator.GenerateSqlAccessWrapper(types, connectionString, outputPath, dbDefs, printClass, false);
    }

    public static IConfigurationRoot GetConfiguration(string[] args) {
        var configurationBuilder = new ConfigurationBuilder();
        // configurationBuilder.AddJsonFile(@"c:\secure\appsettings.TestTypedStoredProcedure.json", true);
        configurationBuilder.AddCommandLine(args).AddUserSecrets(assembly: typeof(Program).Assembly, optional: true);
        var configuration = configurationBuilder.Build();
        return configuration;
    }

    //public static bool MainGenerate(string connectionString, string outputFolder) {
    //    var templateVariables = new Dictionary<string, string>();
    //    var cfg = new GenerateConfiguration();
    //    return Brimborium.GenerateStoredProcedure.Generator.GenerateSql(
    //        connectionString,
    //        outputFolder,
    //        cfg,
    //        templateVariables);
    //    return true;
    //}

    //public static string GetUpperDirectoryPath() {
    //    return System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(getFilePathGenerated() ?? string.Empty)!)!;
    //    static string? getFilePathGenerated([System.Runtime.CompilerServices.CallerFilePath] string? fp = default) => fp;
    //}
}