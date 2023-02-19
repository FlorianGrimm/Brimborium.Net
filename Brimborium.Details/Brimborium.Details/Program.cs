namespace Brimborium.Details;

public static class Program {
    public static async Task Main(string[] args) {
        System.Console.Out.WriteLine("Brimborium.Details");
        System.Console.Out.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        var (configuration, appSettings) = CreateConfiguration(args);
        var existingSolutionInfo = SolutionInfoUtility.LoadSolutionInfo(
            configuration, 
            appSettings.RootPath);
#if false
        var solutionInfoConfiguration = new SolutionInfoConfiguration();
        configuration.Bind(solutionInfoConfiguration);

        var solutionInfo = new SolutionInfo(
            solutionInfoConfiguration.RootPath ?? "",
            solutionInfoConfiguration.SolutionFilePath ?? "",
            solutionInfoConfiguration.DetailPath ?? ""
        );
        solutionInfo.ListMainProjectName.AddRange(solutionInfoConfiguration.ListMainProjectName);
        solutionInfo.ListMainProjectInfo.AddRange(solutionInfoConfiguration.ListMainProjectInfo);
        solutionInfo.ListProject.AddRange(solutionInfoConfiguration.ListProject);

        //var detailJsonPath = args[0];
        //var detailJsonPath = @"C:\visualstudio\solvindev\SEW\AVTV2\SEW.TestPlanning\detail.json";
        var detailJsonPath = @"C:\visualstudio\trans\SEW\AVTV2\SEW.TestPlanning\details.json";
        if (!System.IO.File.Exists(detailJsonPath)) {
            System.Console.Out.WriteLine($"detailJsonPath not found: {detailJsonPath}");
            return;
        }
        System.Console.Out.WriteLine($"detailJsonPath: {detailJsonPath}");
        var existingSolutionInfo = await SolutionInfoUtility.ReadSolutionInfo(detailJsonPath);
#endif
#if false
        System.Console.Out.WriteLine(
            System.Text.Json.JsonSerializer.Serialize(
                existingSolutionInfo,
                new System.Text.Json.JsonSerializerOptions() { WriteIndented = true }));
#endif
        // if (detailJsonPath.Length > 0) {
        //     return;
        // }

        //var solutionInfo = existingSolutionInfo with { RootPath = rootPath };
        // solutionInfo = solutionInfo with {
        //     SolutionFilePath = solutionInfo.GetFullPath(solutionInfo.SolutionFilePath),
        //     DetailPath = solutionInfo.GetFullPath(solutionInfo.DetailPath),
        // };

        var solutionInfo = existingSolutionInfo;

        //var lstProjectName = new string[] { "SEW.TestPlanning.WebApp" };
        //var detailPath = @"C:\visualstudio\solvindev\SEW\AVTV2\SEW.TestPlanning\detail";
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
        builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        builder.AddUserSecrets("Brimborium.Details");
        builder.AddEnvironmentVariables();
        builder.AddCommandLine(args);
        var configuration = builder.Build();

        // is there a configuration file?
        var appSettings = new AppSettings();
        configuration.Bind(appSettings);
        {
            if (string.IsNullOrEmpty(appSettings.Configuration)) {
                return (configuration, appSettings);
            }

            if (string.IsNullOrEmpty(appSettings.RootPath)) {
                appSettings.Configuration=System.IO.Path.GetFullPath(appSettings.Configuration);
                appSettings.RootPath = System.IO.Path.GetDirectoryName(appSettings.Configuration) ?? throw new InvalidOperationException();
            } else {
                appSettings.RootPath=System.IO.Path.GetFullPath(appSettings.RootPath);
                appSettings.Configuration=System.IO.Path.GetFullPath(
                    System.IO.Path.Combine(appSettings.RootPath, appSettings.Configuration));
            }
        }
        {
            builder.AddJsonFile(appSettings.Configuration, optional: false, reloadOnChange: false);
            configuration = builder.Build();
            return (configuration, appSettings);
        }
    } // CreateConfiguration
}

public class AppSettings {
    public string Configuration { get; set; } = string.Empty;
    public string RootPath { get; set; } = string.Empty;
}

