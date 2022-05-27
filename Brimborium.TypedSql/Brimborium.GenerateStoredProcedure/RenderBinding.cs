namespace Brimborium.GenerateStoredProcedure {
    public static class RenderBindingExtension {
        public static string GetFilenameFromTableInfo(
            TableInfo data,
            Dictionary<string, string> boundVariables,
            RenderTemplate template
            ) {
            boundVariables["Schema"] = data.Table.Schema;
            boundVariables["Name"] = data.Table.Name;
            return template.GetFilename(data, boundVariables);
        }
    }
}
