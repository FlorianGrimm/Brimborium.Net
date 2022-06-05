namespace Brimborium.GenerateStoredProcedure {
    public partial class GenerateForStoredProcedure {
        public static bool GenerateSql(
            DatabaseInfo databaseInfo,
            string outputFolder,
            GenerateConfigurationBase cfg,
            Dictionary<string, string> templateVariables,
            bool isVerbose,
            bool isForce) {
            var codeGeneratorBindings = cfg.Build(databaseInfo, isVerbose);
            templateVariables["ProjectRoot"] = outputFolder;
            templateVariables["Schema"] = "";
            templateVariables["Name"] = "";
            
            return CodeGenerator.Generate(
                outputFolder:outputFolder,
                pattern: "*.sql",
                templateVariables:templateVariables, 
                codeGeneratorBindings: codeGeneratorBindings,
                log:null,
                isVerbose:isVerbose
                );
        }
    }
}
