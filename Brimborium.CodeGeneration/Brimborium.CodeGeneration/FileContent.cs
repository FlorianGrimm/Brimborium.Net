﻿namespace Brimborium.CodeGeneration {
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
        private static readonly Regex regexStart = new Regex("^#if[]+false[ \t]*[\\r\\n]+");
        private static readonly Regex regexStop = new Regex("[\\r\\n]+#endif[ \t\\r\\n]*$");

        public bool HasChanged(string fileContent) {
            if (string.IsNullOrEmpty(this.Content)) {
                return true;
            } else {
                if (string.Equals(this.Content, fileContent, StringComparison.OrdinalIgnoreCase)) {
                    return false;
                } else {
                    return true;
                }
            }
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
