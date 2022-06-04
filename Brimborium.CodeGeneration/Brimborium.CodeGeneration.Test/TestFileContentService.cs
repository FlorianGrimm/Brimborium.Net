namespace Brimborium.CodeGeneration {
    public class TestFileContentService : IFileContentService {
        public readonly Dictionary<string, FileContent> DictFileContent;

        public TestFileContentService() {
            this.DictFileContent = new Dictionary<string, FileContent>();
        }

        public FileContent Add(string fileName, string content) {
            var result = new FileContent(fileName, content, this);
            this.DictFileContent[result.FileName] = result;
            return result;
        }

        public FileContent Create(string fileName) {
            if (this.DictFileContent.TryGetValue(fileName, out var result)) {
                return result;
            } else { 
                result = new FileContent(fileName, string.Empty, this);
                return result;
            }
        }

        public FileContent Write(FileContent fileContent, string newContent) {
            var result = new FileContent(fileContent.FileName, newContent);
            this.DictFileContent[result.FileName] = result;
            return result;
        }
    }
}
