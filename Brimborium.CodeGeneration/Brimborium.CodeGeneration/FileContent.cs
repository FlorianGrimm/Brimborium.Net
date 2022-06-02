namespace Brimborium.CodeGeneration {
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed record FileContent(
        string FileName,
        string Content,
        IFileContentService? FileContentService=default) {
        public static FileContent Create(
            string fileName,
            IFileContentService? fileContentService = default) {
            return (fileContentService ?? DefaultFileContentService.GetInstance()).Create(fileName);
            
        }
        private static Regex regexStart = new Regex("^#if[]+false[ \t]*[\\r\\n]+");
        private static Regex regexStop = new Regex("[\\r\\n]+#endif[ \t\\r\\n]*$");

        public bool HasChanged(string fileContent) {
            if (string.IsNullOrEmpty(this.Content)) {
            } else {
                if (regexStart.IsMatch(this.Content) && regexStop.IsMatch(this.Content, System.Math.Max(0, this.Content.Length - 100))) {
                    return false;
                }
                if (string.CompareOrdinal(this.Content, fileContent) == 0) {
                    return false;
                }
            }
            return true;
        }

        public (bool changed, FileContent result) Write(string fileContent) {
            if (this.HasChanged(fileContent)) {
                var result = (this.FileContentService ?? DefaultFileContentService.GetInstance()).Write(this, fileContent);
                return (true, result);
            } else {
                return (false, this);
            }
        }
    }
}
