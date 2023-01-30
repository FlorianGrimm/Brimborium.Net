namespace Brimborium.Extensions.Logging.LocalFile;

public interface ILocalFileAccess {
    void CreateDirectory(string path);
    FileInformation GetFileInfo(string fullName);
    IEnumerable<FileInformation> GetFiles(string path, string searchPattern);
    Task WriteFileAsync(string fullName, IEnumerable<LogMessage> items);
    void WriteErrorLine(Exception error);
    void DeleteFile(string fullName);
}

public record struct FileInformation(
    string Name,
    string FullName,
    bool Exists,
    long Length,
    DateTime LastWriteTimeUtc
) {
    public static FileInformation FromFileInfo(FileInfo fileInfo)
        => new FileInformation(
            fileInfo.Name,
            fileInfo.FullName,
            fileInfo.Exists,
            fileInfo.Length,
            fileInfo.LastWriteTimeUtc);
}

public class LocalFileAccess : ILocalFileAccess {
    public LocalFileAccess() {
    }

    public void CreateDirectory(string path) {
        Directory.CreateDirectory(path);
    }

    public FileInformation GetFileInfo(string fullName)
        => FileInformation.FromFileInfo(new FileInfo(fullName));

    public IEnumerable<FileInformation> GetFiles(string path, string searchPattern) {
        return (new System.IO.DirectoryInfo(path))
            .GetFiles(searchPattern)
            .Select(FileInformation.FromFileInfo);
    }

    public async Task WriteFileAsync(string fullName, IEnumerable<LogMessage> items) {
        using (var streamWriter = File.AppendText(fullName)) {
            foreach (var item in items) {
                await streamWriter.WriteAsync(item.Message).ConfigureAwait(false);
            }
            await streamWriter.FlushAsync().ConfigureAwait(false);
            streamWriter.Close();
        }
    }

    public void DeleteFile(string fullName) {
        File.Delete(fullName);
    }

    public void WriteErrorLine(Exception error) {
        System.Console.Error.WriteLine(error.ToString());
    }
}
