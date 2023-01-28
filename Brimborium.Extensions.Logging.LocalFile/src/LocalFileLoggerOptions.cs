// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Brimborium.Extensions.Logging.LocalFile;

/// <summary>
/// Options for local file logging.
/// </summary>
public class LocalFileLoggerOptions : FileBasedLoggerOptions {
    private int? _FilesPerPeriodicityLimit = 10;
    private string? _Extension = "txt";

    public LocalFileLoggerOptions() {
        this.BatchSize = null;

        // this.LogDirectory  = "Logs";
    }

    /// <summary>
    /// Gets or sets a value representing the maximum number of files allowed for a given <see cref="Periodicity"/> .
    /// Once the specified number of logs per periodicity are created, no more log files will be created. Note that these extra files
    /// do not count towards the RetrainedFileCountLimit. Defaults to <c>1</c>.
    /// </summary>
    public int? FilesPerPeriodicityLimit {
        get { return _FilesPerPeriodicityLimit; }
        set {
            if (value <= 0) {
                throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(FilesPerPeriodicityLimit)} must be greater than 0.");
            }
            _FilesPerPeriodicityLimit = value;
        }
    }

    /// <summary>
    /// Gets or sets the filename extension to use for log files.
    /// Defaults to <c>txt</c>.
    /// Will strip any prefixed .
    /// </summary>
    public string? Extension {
        get { return this._Extension; }
        set { this._Extension = value?.TrimStart('.'); }
    }

    /// <summary>
    /// Gets or sets the periodicity for rolling over log files.
    /// </summary>
    public PeriodicityOptions Periodicity { get; set; } = PeriodicityOptions.Daily;
}


public enum PeriodicityOptions {
    Daily,
    Hourly,
    Minutely,
    Monthly
}