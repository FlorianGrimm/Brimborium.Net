// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;

using System.Runtime.CompilerServices;

namespace Brimborium.Extensions.Logging.LocalFile;

[ProviderAlias("LocalFile")]
public class LocalFileLoggerProvider : BatchingLoggerProvider {
    public static LocalFileLoggerProvider Create(
        IOptionsMonitor<LocalFileLoggerOptions> options,
        ILocalFileAccess? localFileAccess = null
    ) => new LocalFileLoggerProvider(
        options, localFileAccess);

    private record class State(
        string? path,
        string fileName,
        string? extension,
        int maxFileSize,
        int maxRetainedFiles,
        int maxFileCountPerPeriodicity,
        PeriodicityOptions periodicity
        );
    private State _State;

    private readonly ILocalFileAccess _LocalFileAccess;

    /// <summary>
    /// Creates a new instance of <see cref="AzureAppServicesFileLoggerProvider"/>.
    /// </summary>
    /// <param name="options">The options to use when creating a provider.</param>
    [SuppressMessage("ApiDesign", "RS0022:Constructor make noninheritable base class inheritable", Justification = "Required for backwards compatibility")]
    [ActivatorUtilitiesConstructor]
    public LocalFileLoggerProvider(
        IOptionsMonitor<LocalFileLoggerOptions> options
        ) : this(options, null) {
    }


    protected LocalFileLoggerProvider(
        IOptionsMonitor<LocalFileLoggerOptions> options,
        ILocalFileAccess? localFileAccess
        ) : base(options) {
        this._LocalFileAccess = localFileAccess ?? new LocalFileAccess();
        this._State = ConvertOptions(options.CurrentValue);
    }

    protected override void UpdateOptions(BatchingLoggerOptions options) {
        if (options is LocalFileLoggerOptions localFileLoggerOptions) {
            this._State = ConvertOptions(localFileLoggerOptions);
        }
        base.UpdateOptions(options);
    }

    private State ConvertOptions(LocalFileLoggerOptions options)
        => new State(
            path: options.LogDirectory,
            fileName: options.FileName,
            extension: options.Extension,
            maxFileSize: options.FileSizeLimit ?? 0,
            maxRetainedFiles: options.RetainedFileCountLimit ?? 0,
            maxFileCountPerPeriodicity: options.FilesPerPeriodicityLimit ?? 1,
            periodicity: options.Periodicity
            );

    protected internal override ILogFormatterB GetLogFormatter() 
        => this._LogFormatter ??= this.CreateLogFormatter<JsonLineLogFormatterB>();

    internal protected override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken cancellationToken) {
        // copy state to be immutable for this batch
        var state = this._State; 
        if (state.path is null) { throw new System.ArgumentException("path is null"); }

        this._LocalFileAccess.CreateDirectory(state.path);

        foreach (var group in messages.GroupBy(this.GetGrouping)) {
            var baseName = this.GetBaseName(group.Key, state);
            var fullName = this.GetLogFilePath(baseName, group.Key, state);

            if (fullName == null) { return; }
            if (state.maxFileSize > 0) {
                var fileInfo = this._LocalFileAccess.GetFileInfo(fullName);
                if (fileInfo.Exists
                    && fileInfo.Length > state.maxFileSize) {
                    return;
                }
            }
            try {
                await this._LocalFileAccess.WriteFileAsync(fullName, group);
            } catch (System.Exception error) {
                this._LocalFileAccess.WriteErrorLine(error);
            }
        }

        this.RollFiles(state);
    }

    private string? GetLogFilePath(
        string baseName,
        FileGroupingYMDHM fileNameGrouping,
        in State state) {
        if (state.path is null) { return null; }

        var maxFileSize = state.maxFileSize;

        if (state.maxFileCountPerPeriodicity == 1) {
            var fullPath = Path.Combine(state.path, $"{baseName}{state.extension}");
            return IsAvailable(fullPath) ? fullPath : null;
        }

        var counter = this.GetCurrentCounter(baseName, state);

        while (counter < state.maxFileCountPerPeriodicity) {
            var fullName = System.IO.Path.Combine(
                state.path,
                $"{baseName}.{counter}{state.extension}");
            if (IsAvailable(fullName)) {
                return fullName;
            } else {
                counter++;
                continue;
            }
        }

        return null;

        bool IsAvailable(string filename) {
            var fileInfo = this._LocalFileAccess.GetFileInfo(filename);
            return !(maxFileSize > 0
                    && fileInfo.Exists
                    && fileInfo.Length > maxFileSize);
        }
    }

    private int GetCurrentCounter(
        string baseName,
        in State state) {
        if (state.path is null) { return 0; }
        try {
            var extension = state.extension;
            var files = Directory.GetFiles(state.path, $"{baseName}.*{extension}");
            if (files.Length == 0) {
                // No rolling file currently exists with the base name as pattern
                return 0;
            }

            // Get file with highest counter
            var latestFile = files.OrderByDescending(file => file).First();

            var baseNameLength = Path.Combine(state.path, baseName).Length + 1;
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
        } catch {
            return 0;
        }
    }

    private string GetBaseName(
        FileGroupingYMDHM group,
        in State state) {
        switch (state.periodicity) {
            case PeriodicityOptions.Minutely:
                return $"{state.fileName}{group.Year:0000}{group.Month:00}{group.Day:00}{group.Hour:00}{group.Minute:00}";
            case PeriodicityOptions.Hourly:
                return $"{state.fileName}{group.Year:0000}{group.Month:00}{group.Day:00}{group.Hour:00}";
            case PeriodicityOptions.Daily:
                return $"{state.fileName}{group.Year:0000}{group.Month:00}{group.Day:00}";
            case PeriodicityOptions.Monthly:
                return $"{state.fileName}{group.Year:0000}{group.Month:00}";
        }
        throw new InvalidDataException("Invalid periodicity");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private FileGroupingYMDHM GetGrouping(LogMessage message)
        => FileGroupingYMDHM.FromDateTimeOffset(message.Timestamp);

    private void RollFiles(
        in State state) {
        if (state.path is null) { throw new System.ArgumentException("_path is null"); }
        var hasExtension = !string.IsNullOrEmpty(state.extension);
        var isMultiFile = state.maxFileCountPerPeriodicity > 1;

        try {
            if (state.maxRetainedFiles > 0) {
                var groupsToDelete = this._LocalFileAccess.GetFiles(state.path, state.fileName + "*")
                    .Select(file => new KeyValuePair<string, FileInformation>(GetFilenameForGrouping(file.Name), file))
                    .Where(item => !string.IsNullOrEmpty(item.Key))
                    .GroupBy(item => item.Key, item => item.Value)
                    .OrderByDescending(f => f.Key)
                    .Skip(state.maxRetainedFiles);
                // Exception? lastError = null;
                foreach (var groupToDelete in groupsToDelete) {
                    foreach (var fileToDelete in groupToDelete) {
                        try {
                            this._LocalFileAccess.DeleteFile(fileToDelete.FullName);
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
            return (isMultiFile, hasExtension) switch {
                (false, false) => filename,
                (false, true) => Path.GetFileNameWithoutExtension(filename),
                (true, false) => Path.GetFileNameWithoutExtension(filename),
                (true, true) => Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(filename)),
            };
        }
    }
}
