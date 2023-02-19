namespace Brimborium.Details;

public static class Program {
    public static async Task Main(string[] args) {
        System.Console.Out.WriteLine("Brimborium.Details");
        System.Console.Out.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        var (configuration, appSettings) = CreateConfiguration(args);

        /*
        configuration.GetReloadToken().RegisterChangeCallback((state) => {
            System.Console.Out.WriteLine("Configuration changed");
        }, null);
        */
        
        SolutionInfo solutionInfo;

        {
            System.Console.Out.WriteLine($"appSettings.DetailsRoot: {appSettings.DetailsRoot}");
            System.Console.Out.WriteLine($"appSettings.DetailsConfiguration: {appSettings.DetailsConfiguration}");
            if (string.IsNullOrEmpty(appSettings.DetailsRoot)) {
                System.Console.Error.WriteLine("no DetailsRoot");
                return;
            }

            var loadedSolutionInfo = SolutionInfoUtility.LoadSolutionInfo(
                configuration);

            if (string.IsNullOrEmpty(loadedSolutionInfo.SolutionFile)){
                System.Console.Error.WriteLine("empty SolutionFile - please specify");
                return;
            }

            if (string.IsNullOrEmpty(loadedSolutionInfo.DetailsFolder)){
                System.Console.Error.WriteLine("empty DetailsFolder - please specify");
                return;
            }

            solutionInfo = loadedSolutionInfo.PostLoad(appSettings.DetailsRoot);
            System.Console.Out.WriteLine($"Final Values:");
            System.Console.Out.WriteLine($"DetailsRoot: {solutionInfo.DetailsRoot}");
            System.Console.Out.WriteLine($"SolutionFile: {solutionInfo.SolutionFile}");
            System.Console.Out.WriteLine($"DetailsFolder: {solutionInfo.DetailsFolder}");
        }

        var markdownUtility = new MarkdownUtility(solutionInfo);
        await markdownUtility.ParseDetail();

        // var csharpUtility=new CSharpUtility(solutionInfo);
        // await csharpUtility.ParseCSharp();

        // var typescriptUtility=new TypeScriptUtility(solutionInfo);
        // await typescriptUtility.ParseTypeScript();

        // await markdownUtility.WriteDetail();

        //solution.GetProject()
#if false
        var extension = System.IO.Path.GetExtension(detailJsonPath);
        var outputDetailJsonPath = detailJsonPath.Substring(0, detailJsonPath.Length - extension.Length) + ".stage" + extension;
        await SolutionInfoUtility.WriteSolutionInfo(outputDetailJsonPath, solutionInfo);
#endif
    } // main

    public static (IConfigurationRoot configuration, AppSettings appSettings) CreateConfiguration(string[] args) {
        ConfigurationBuilder builder = new ConfigurationBuilder();
        builder.AddUserSecrets("Brimborium.Details");
        builder.AddEnvironmentVariables();
        builder.AddCommandLine(args);
        var configuration = builder.Build();

        // is there a configuration file?
        var appSettings = new AppSettings();
        configuration.Bind(appSettings);
        {
            if (string.IsNullOrEmpty(appSettings.DetailsConfiguration)) {
                var lstDetailsJsonFileName = System.IO.Directory.EnumerateFiles(
                    System.Environment.CurrentDirectory,
                    "details.json",
                    new EnumerationOptions() {
                        RecurseSubdirectories = false
                    })
                    .ToList();
                if (lstDetailsJsonFileName.Count == 1) {
                    appSettings.DetailsConfiguration = lstDetailsJsonFileName[0];
                    appSettings.DetailsRoot = System.Environment.CurrentDirectory;
                }
            }
            if (string.IsNullOrEmpty(appSettings.DetailsConfiguration)) {

                var lstDetailsJsonFileName = System.IO.Directory.EnumerateFiles(
                 System.Environment.CurrentDirectory,
                 "details.json",
                 new EnumerationOptions() {
                     RecurseSubdirectories = true,
                     MaxRecursionDepth = 2
                 })
                 .ToList();
                if (lstDetailsJsonFileName.Count == 1) {
                    appSettings.DetailsConfiguration = lstDetailsJsonFileName[0];
                    appSettings.DetailsRoot = System.Environment.CurrentDirectory;
                }
            }
            if (string.IsNullOrEmpty(appSettings.DetailsConfiguration)) {
                var parentDirectory = System.IO.Path.GetDirectoryName(System.Environment.CurrentDirectory);
                if (parentDirectory is not null) {
                    var lstDetailsJsonFileName = System.IO.Directory.EnumerateFiles(
                        parentDirectory,
                        "details.json",
                        new EnumerationOptions() {
                            RecurseSubdirectories = false
                        })
                        .ToList();
                    if (lstDetailsJsonFileName.Count == 1) {
                        appSettings.DetailsConfiguration = lstDetailsJsonFileName[0];
                        appSettings.DetailsRoot = parentDirectory;
                    }
                }
            }
            if (string.IsNullOrEmpty(appSettings.DetailsConfiguration)) {
                if (string.IsNullOrEmpty(appSettings.DetailsRoot)) {
                    appSettings.DetailsRoot = System.Environment.CurrentDirectory;
                }
                return (configuration, appSettings);
            }

            if (string.IsNullOrEmpty(appSettings.DetailsRoot)) {
                appSettings.DetailsConfiguration = System.IO.Path.GetFullPath(appSettings.DetailsConfiguration);
                appSettings.DetailsRoot = System.IO.Path.GetDirectoryName(appSettings.DetailsConfiguration) ?? throw new InvalidOperationException();
            } else {
                appSettings.DetailsRoot = System.IO.Path.GetFullPath(appSettings.DetailsRoot);
                appSettings.DetailsConfiguration = System.IO.Path.GetFullPath(
                    System.IO.Path.Combine(appSettings.DetailsRoot, appSettings.DetailsConfiguration));
            }
        }
        {
            builder.AddJsonFile(appSettings.DetailsConfiguration, optional: false, reloadOnChange: true);
            configuration = builder.Build();
            return (configuration, appSettings);
        }
    } // CreateConfiguration
}

public class AppSettings {
    public string DetailsConfiguration { get; set; } = string.Empty;
    public string DetailsRoot { get; set; } = string.Empty;
}

