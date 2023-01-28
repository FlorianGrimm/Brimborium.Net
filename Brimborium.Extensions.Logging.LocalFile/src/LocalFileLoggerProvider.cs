// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Brimborium.Extensions.Logging.LocalFile;

[ProviderAlias("LocalFile")]
public class LocalFileLoggerProvider : BatchingLoggerProvider {
    public static LocalFileLoggerProvider Create(
        IOptionsMonitor<LocalFileLoggerOptions> options,
        ILocalFileAccess? localFileAccess = null
    ) => new LocalFileLoggerProvider(options, localFileAccess);

    private string? _path;
    private string _fileName;
    private string? _extension;
    private int? _maxFileSize;
    private int? _maxRetainedFiles;
    private int _maxFileCountPerPeriodicity;
    private PeriodicityOptions _periodicity;
    private readonly ILocalFileAccess _LocalFileAccess;

    protected LocalFileLoggerProvider(
        IOptionsMonitor<LocalFileLoggerOptions> options,
        ILocalFileAccess? localFileAccess
        ) : base(options) {
        this._LocalFileAccess = localFileAccess ?? new LocalFileAccess();
        var loggerOptions = options.CurrentValue;
        this._path = loggerOptions.LogDirectory;
        this._fileName = loggerOptions.FileName;
        this._extension = loggerOptions.Extension;
        this._maxFileSize = loggerOptions.FileSizeLimit;
        this._maxRetainedFiles = loggerOptions.RetainedFileCountLimit;
        this._maxFileCountPerPeriodicity = loggerOptions.FilesPerPeriodicityLimit ?? 1;
        this._periodicity = loggerOptions.Periodicity;
    }

    /// <summary>
    /// Creates a new instance of <see cref="AzureAppServicesFileLoggerProvider"/>.
    /// </summary>
    /// <param name="options">The options to use when creating a provider.</param>
    [SuppressMessage("ApiDesign", "RS0022:Constructor make noninheritable base class inheritable", Justification = "Required for backwards compatibility")]
    public LocalFileLoggerProvider(
        IOptionsMonitor<LocalFileLoggerOptions> options
        ) : this(options, null) {
    }

    protected override void UpdateOptions(BatchingLoggerOptions options) {
        if (options is LocalFileLoggerOptions localFileLoggerOptions) {
            this._path = localFileLoggerOptions.LogDirectory;
            this._fileName = localFileLoggerOptions.FileName;
            this._maxFileSize = localFileLoggerOptions.FileSizeLimit;
            this._maxRetainedFiles = localFileLoggerOptions.RetainedFileCountLimit;
            this._maxFileCountPerPeriodicity = localFileLoggerOptions.FilesPerPeriodicityLimit ?? 1;
            this._periodicity = localFileLoggerOptions.Periodicity;
        }
        base.UpdateOptions(options);
    }

    internal protected override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken cancellationToken) {
        if (this._path is null) { throw new System.ArgumentException("_path is null"); }

        this._LocalFileAccess.CreateDirectory(this._path);

        foreach (var group in messages.GroupBy(this.GetGrouping)) {
            var baseName = this.GetBaseName(group.Key);
            var fullName = this.GetLogFilePath(baseName, group.Key);

            if (fullName == null) {
                return;
            }

            // var fileInfo = new FileInfo(fullName);
            var fileInfo = this._LocalFileAccess.GetFileInfo(fullName);
            if (this._maxFileSize > 0 && fileInfo.Exists && fileInfo.Length > this._maxFileSize) {
                return;
            }
            try {
                await this._LocalFileAccess.WriteFileAsync(fullName, group);
                using (var streamWriter = File.AppendText(fullName)) {
                    foreach (var item in group) {
                        await streamWriter.WriteAsync(item.Message).ConfigureAwait(false);
                    }
                    await streamWriter.FlushAsync().ConfigureAwait(false);
                    streamWriter.Close();
                }
            } catch (System.Exception error) {
                System.Console.Error.WriteLine(error.ToString());
            }
        }

        this.RollFiles();
    }

    // private string GetFullName((int Year, int Month, int Day) group) {
    //     if (this._path is null) { throw new System.ArgumentException("_path is null"); }

    //     return Path.Combine(this._path, $"{this._fileName}{group.Year:0000}{group.Month:00}{group.Day:00}.txt");
    // }

    private string? GetLogFilePath(string baseName, FileGrouping fileNameGrouping) {
        if (this._path is null) { return null; }

        if (_maxFileCountPerPeriodicity == 1) {
            var fullPath = Path.Combine(_path, $"{baseName}{_extension}");
            return IsAvailable(fullPath) ? fullPath : null;
        }

        var counter = GetCurrentCounter(baseName);

        while (counter < _maxFileCountPerPeriodicity) {
            var fullName = Path.Combine(_path, $"{baseName}.{counter}{_extension}");
            if (!IsAvailable(fullName)) {
                counter++;
                continue;
            }

            return fullName;
        }

        return null;

        bool IsAvailable(string filename) {
            var fileInfo = new FileInfo(filename);
            return !(_maxFileSize > 0 && fileInfo.Exists && fileInfo.Length > _maxFileSize);
        }
    }

    private int GetCurrentCounter(string baseName) {
        if (this._path is null) { return 0; }
        try {
            var files = Directory.GetFiles(_path, $"{baseName}.*{_extension}");
            if (files.Length == 0) {
                // No rolling file currently exists with the base name as pattern
                return 0;
            }

            // Get file with highest counter
            var latestFile = files.OrderByDescending(file => file).First();

            var baseNameLength = Path.Combine(_path, baseName).Length + 1;
            var fileWithoutPrefix = latestFile
                .AsSpan()
                .Slice(baseNameLength);
            var indexOfPeriod = fileWithoutPrefix.IndexOf('.');
            if (indexOfPeriod < 0) {
                // No additional dot could be found
                return 0;
            }

            var counterSpan = fileWithoutPrefix.Slice(0, indexOfPeriod);
            if (int.TryParse(counterSpan.ToString(), out var counter)) {
                return counter;
            }

            return 0;
        } catch (Exception) {
            return 0;
        }
    }

    private string GetBaseName(FileGrouping group) {
        switch (_periodicity) {
            case PeriodicityOptions.Minutely:
                return $"{_fileName}{group.Year:0000}{group.Month:00}{group.Day:00}{group.Hour:00}{group.Minute:00}";
            case PeriodicityOptions.Hourly:
                return $"{_fileName}{group.Year:0000}{group.Month:00}{group.Day:00}{group.Hour:00}";
            case PeriodicityOptions.Daily:
                return $"{_fileName}{group.Year:0000}{group.Month:00}{group.Day:00}";
            case PeriodicityOptions.Monthly:
                return $"{_fileName}{group.Year:0000}{group.Month:00}";
        }
        throw new InvalidDataException("Invalid periodicity");
    }

    private FileGrouping GetGrouping(LogMessage message)
        => new FileGrouping(message.Timestamp.Year, message.Timestamp.Month, message.Timestamp.Day, message.Timestamp.Hour, message.Timestamp.Minute);

    private void RollFiles() {
        if (this._path is null) { throw new System.ArgumentException("_path is null"); }

        try {
            if (this._maxRetainedFiles > 0) {
                var groupsToDelete = this._LocalFileAccess
                    .GetFiles(this._path, this._fileName + "*")
                    .Select(file => new KeyValuePair<string, FileInfo>(GetFilenameForGrouping(file.Name), file))
                    .Where(item => !string.IsNullOrEmpty(item.Key))
                    .GroupBy(item => item.Key, item => item.Value)
                    .OrderByDescending(f => f.Key)
                    .Skip(this._maxRetainedFiles.Value);
                // Exception? lastError = null;
                foreach (var groupToDelete in groupsToDelete) {
                    foreach (var fileToDelete in groupToDelete) {
                        try {
                            fileToDelete.Delete();
                        } catch (Exception error) {
                            System.Console.Error.WriteLine(error.ToString());
                            // lastError = error;
                        }
                    }
                }
                // if (lastError is not null) {
                //     System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(lastError).Throw();
                //     throw lastError;
                // }
            }
        } catch (System.Exception error) {
            System.Console.Error.WriteLine(error.ToString());
        }

        string GetFilenameForGrouping(string filename) {
            var hasExtension = !string.IsNullOrEmpty(_extension);
            var isMultiFile = _maxFileCountPerPeriodicity > 1;
            return (isMultiFile, hasExtension) switch {
                (false, false) => filename,
                (false, true) => Path.GetFileNameWithoutExtension(filename),
                (true, false) => Path.GetFileNameWithoutExtension(filename),
                (true, true) => Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(filename)),
            };
        }
    }
}

internal record FileGrouping(
    int Year,
    int Month,
    int Day,
    int Hour,
    int Minute
);

public interface ILocalFileAccess {
    void CreateDirectory(string path);
    FileInformation GetFileInfo(string fullName);
    IEnumerable<FileInformation> GetFiles(string path, string searchPattern);
    Task WriteFileAsync(string fullName, IEnumerable<LogMessage> items);
}

public record struct FileInformation(
    string FullName,
    bool Exists,
    long Length,
    DateTime LastWriteTimeUtc
);

public class LocalFileAccess : ILocalFileAccess {
    public LocalFileAccess() {
    }
    public void CreateDirectory(string path) {
        Directory.CreateDirectory(path);
    }
    public FileInformation GetFileInfo(string fullName) {
        var fileInfo = new FileInfo(fullName);
        return new FileInformation(
            fileInfo.FullName,
            fileInfo.Exists,
            fileInfo.Length,
            fileInfo.LastWriteTimeUtc);
    }
    public IEnumerable<FileInformation> GetFiles(string path, string searchPattern) {
        return (new System.IO.DirectoryInfo(path))
            .GetFiles(searchPattern)
            .Select(fileInfo => new FileInformation(
                fileInfo.FullName,
                fileInfo.Exists,
                fileInfo.Length,
                fileInfo.LastWriteTimeUtc));
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
}
