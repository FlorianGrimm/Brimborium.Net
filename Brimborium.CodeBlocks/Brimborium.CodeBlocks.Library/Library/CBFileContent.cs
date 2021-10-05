namespace Brimborium.CodeBlocks.Library {
    public sealed class CBFileContent {
        public CBFileContent(string fileName, string content) {
            this.FileName = fileName;
            this.Content = content;
        }

        public string FileName { get; set; }
        public string Content { get; set; }
        public bool TempModified { get; internal set; }
    }
}
