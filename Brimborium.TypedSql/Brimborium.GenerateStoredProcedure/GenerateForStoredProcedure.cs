namespace Brimborium.GenerateStoredProcedure {
    public partial class GenerateForStoredProcedure {
        public static bool GenerateSql(
            DatabaseInfo databaseInfo,
            string outputFolder,
            GenerateConfigurationBase cfg,
            Dictionary<string, string> templateVariables,
            bool isVerbose,
            bool isForce) {
            var config = cfg.Build(databaseInfo, isVerbose);
            templateVariables["ProjectRoot"] = outputFolder;
            templateVariables["Schema"] = "";
            templateVariables["Name"] = "";
            var di = new System.IO.DirectoryInfo(outputFolder);
            var arrFileInfo = di.GetFiles("*.sql", System.IO.SearchOption.AllDirectories);

            return CodeGenerator.Generate(
                arrFileInfo:arrFileInfo,
                templateVariables:templateVariables, 
                config:config, 
                log:null,
                isVerbose:isVerbose
                );
        }
    }
}
