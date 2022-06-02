namespace Brimborium.CodeGeneration {
    public interface IFileContentService {
        FileContent Create(string fileName);
        FileContent Write(FileContent fileContent, string newContent);
    }
}
