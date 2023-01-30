// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace Brimborium.Extensions.Logging.LocalFile;

/// <summary>
/// A <see cref="BatchingLoggerProvider"/> which writes out to a file.
/// </summary>
[ProviderAlias("AzureAppServicesFile")]
public class AzureAppServicesFileLoggerProvider : BatchingLoggerProvider {
    public static AzureAppServicesFileLoggerProvider Create(
        IOptionsMonitor<AzureAppServicesFileLoggerOptions> options,
        ILocalFileAccess? localFileAccess = null
    ) => new AzureAppServicesFileLoggerProvider(
        options, localFileAccess);

    private record class State(
        string? path,
        string fileName,
        int maxFileSize,
        int maxRetainedFiles
        );
    private State _State;
    private ILocalFileAccess _LocalFileAccess;


    /// <summary>
    /// Creates a new instance of <see cref="AzureAppServicesFileLoggerProvider"/>.
    /// </summary>
    /// <param name="options">The options to use when creating a provider.</param>
    [SuppressMessage("ApiDesign", "RS0022:Constructor make noninheritable base class inheritable", Justification = "Required for backwards compatibility")]
    [ActivatorUtilitiesConstructor]
    public AzureAppServicesFileLoggerProvider(
        IOptionsMonitor<AzureAppServicesFileLoggerOptions> options
        ) : this(options, null){ 
    }

    [SuppressMessage("ApiDesign", "RS0022:Constructor make noninheritable base class inheritable", Justification = "Required for backwards compatibility")]
    protected AzureAppServicesFileLoggerProvider(
        IOptionsMonitor<AzureAppServicesFileLoggerOptions> options,
        ILocalFileAccess? localFileAccess
        ) : base(options) {
        this._LocalFileAccess = localFileAccess ?? new LocalFileAccess();
        this._State = ConvertOptions(options.CurrentValue);
    }
    
    protected override void UpdateOptions(BatchingLoggerOptions options) {
        if (options is AzureAppServicesFileLoggerOptions azureAppServicesFileLoggerOptions) {
            this._State = ConvertOptions(azureAppServicesFileLoggerOptions);
        }
        base.UpdateOptions(options);
    }

    private State ConvertOptions(AzureAppServicesFileLoggerOptions options)
        => new State(
            path: options.LogDirectory,
            fileName: options.FileName,
            //extension: options.Extension,
            maxFileSize: options.FileSizeLimit ?? 0,
            maxRetainedFiles: options.RetainedFileCountLimit ?? 0
            //maxFileCountPerPeriodicity: options.FilesPerPeriodicityLimit ?? 1,
            //periodicity: options.Periodicity
            );

    internal protected override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken cancellationToken) {
        var state = this._State;
        if (state.path is null) { throw new System.ArgumentException("path is null"); }

        this._LocalFileAccess.CreateDirectory(state.path);

        foreach (var group in messages.GroupBy(this.GetGrouping)) {
            var fullName = this.GetFullName(group.Key, state);
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

    private string GetFullName(FileGroupingYMD group, in State state) {
        if (state.path is null) { throw new System.ArgumentException("_path is null"); }

        return Path.Combine(state.path, $"{state.fileName}{group.Year:0000}{group.Month:00}{group.Day:00}.txt");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private FileGroupingYMD GetGrouping(LogMessage message) 
        => FileGroupingYMD.FromDateTimeOffset(message.Timestamp);
    
    private void RollFiles(in State state) {
        if (state.path is null) { throw new System.ArgumentException("path is null"); }

        try {
            if (state.maxRetainedFiles > 0) {
                var files = this._LocalFileAccess.GetFiles(state.path, state.fileName + "*")
                    .OrderByDescending(f => f.Name)
                    .Skip(state.maxRetainedFiles);

                foreach (var fileToDelete in files) {
                    this._LocalFileAccess.DeleteFile(fileToDelete.FullName);
                }
            }
        } catch (System.Exception error) {
            System.Console.Error.WriteLine(error.ToString());
        }
    }
}