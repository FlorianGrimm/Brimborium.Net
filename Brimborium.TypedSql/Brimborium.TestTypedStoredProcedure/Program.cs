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

            { 
                var defintions = GetDefintion();

                var(outputPath, outputNamespace, outputClassName) = Brimborium.TestSample.Service.SqlAccessLocation.GetPrimaryKeyOutputInfo();
                //var outputFolder = System.IO.Path.GetDirectoryName(outputPath);

                MainGenerateSqlAccess(connectionString, defintions, outputPath, outputNamespace, outputClassName);
            }
           
            return 0;
        } catch (System.Exception error) {
            System.Console.Error.WriteLine(error.ToString());
            return 1;
        }
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
        GeneratorForTypedSqlAccess.GenerateSqlAccessWrapper(types, connectionString, outputPath, dbDefs, printClass, false);
    }

    public static IConfigurationRoot GetConfiguration(string[] args) {
        var configurationBuilder = new ConfigurationBuilder();
        // configurationBuilder.AddJsonFile(@"c:\secure\appsettings.TestTypedStoredProcedure.json", true);
        configurationBuilder.AddCommandLine(args).AddUserSecrets(assembly: typeof(Program).Assembly, optional: true);
        var configuration = configurationBuilder.Build();
        return configuration;
    }
}