namespace Brimborium.CodeGeneration {
    public class DefaultFileContentService : IFileContentService {
        private static IFileContentService? _Instance;
        public static IFileContentService GetInstance() => (_Instance ??= new DefaultFileContentService());

        public DefaultFileContentService() {
        }

        public FileContent Create(string fileName) {
            try {
                var content = System.IO.File.ReadAllText(fileName);
                return new FileContent(fileName, content);
            } catch {
                return new FileContent(fileName, "");
            }
        }

        public FileContent Write(FileContent fileContent, string newContent) {
            var result = new FileContent(fileContent.FileName, newContent);
            System.IO.File.WriteAllText(fileContent.FileName, newContent);
            return result;
        }
    }
}
