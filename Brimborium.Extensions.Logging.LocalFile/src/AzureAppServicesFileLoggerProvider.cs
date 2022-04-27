// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Brimborium.Extensions.Logging.LocalFile;

/// <summary>
/// A <see cref="BatchingLoggerProvider"/> which writes out to a file.
/// </summary>
[ProviderAlias("AzureAppServicesFile")]
public class AzureAppServicesFileLoggerProvider : BatchingLoggerProvider {
    private readonly string _path;
    private readonly string _fileName;
    private readonly int? _maxFileSize;
    private readonly int? _maxRetainedFiles;

    /// <summary>
    /// Creates a new instance of <see cref="AzureAppServicesFileLoggerProvider"/>.
    /// </summary>
    /// <param name="options">The options to use when creating a provider.</param>
    [SuppressMessage("ApiDesign", "RS0022:Constructor make noninheritable base class inheritable", Justification = "Required for backwards compatibility")]
    public AzureAppServicesFileLoggerProvider(IOptionsMonitor<AzureAppServicesFileLoggerOptions> options) : base(options) {
        var loggerOptions = options.CurrentValue;
        this._path = loggerOptions.LogDirectory;
        this._fileName = loggerOptions.FileName;
        this._maxFileSize = loggerOptions.FileSizeLimit;
        this._maxRetainedFiles = loggerOptions.RetainedFileCountLimit;
    }

    internal protected override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken cancellationToken) {
        Directory.CreateDirectory(this._path);

        foreach (var group in messages.GroupBy(this.GetGrouping)) {
            var fullName = this.GetFullName(group.Key);
            var fileInfo = new FileInfo(fullName);
            if (this._maxFileSize > 0 && fileInfo.Exists && fileInfo.Length > this._maxFileSize) {
                return;
            }

            try {
                using (var streamWriter = File.AppendText(fullName)) {
                    foreach (var item in group) {
                        await streamWriter.WriteAsync(item.Message).ConfigureAwait(false);
                    }
                    streamWriter.Close();
                }
            } catch (System.Exception error) {
                System.Console.Error.WriteLine(error.ToString());
            }
        }

        this.RollFiles();
    }

    private string GetFullName((int Year, int Month, int Day) group) {
        return Path.Combine(this._path, $"{this._fileName}{group.Year:0000}{group.Month:00}{group.Day:00}.txt");
    }

    private (int Year, int Month, int Day) GetGrouping(LogMessage message) {
        return (message.Timestamp.Year, message.Timestamp.Month, message.Timestamp.Day);
    }

    private void RollFiles() {
        try {
            if (this._maxRetainedFiles > 0) {
                var files = new DirectoryInfo(this._path)
                    .GetFiles(this._fileName + "*")
                    .OrderByDescending(f => f.Name)
                    .Skip(this._maxRetainedFiles.Value);

                foreach (var item in files) {
                    item.Delete();
                }
            }
        } catch (System.Exception error) {
            System.Console.Error.WriteLine(error.ToString());
        }
    }
}