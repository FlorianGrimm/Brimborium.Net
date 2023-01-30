namespace Brimborium.Extensions.Logging.LocalFile;

/// <summary>
/// 
/// </summary>
/// <param name="IncludeScopes"></param>
/// <param name="ScopeProvider"></param>
/// <param name="IsEnabled"></param>
/// <param name="UseJSONFormat"></param>
/// <param name="IncludeEventId"></param>
/// <param name="JsonWriterOptions"></param>
/// <param name="TimestampFormat">
/// Gets or sets format string used to format timestamp in logging messages. Defaults to <c>null</c>.
/// </param>
/// <param name="UseUtcTimestamp">
/// Gets or sets indication whether or not UTC timezone should be used to format timestamps in logging messages. Defaults to <c>false</c>.
/// </param>
/// <param name="UseLogFormatter"></param>
/// <param name="LogFormatter"></param>
public record BatchingLoggerState(
    bool IncludeScopes,
    IExternalScopeProvider? ScopeProvider,
    bool IsEnabled,
    bool IncludeEventId,
    JsonWriterOptions JsonWriterOptions,
    string? TimestampFormat,
    bool UseUtcTimestamp,
    string? UseLogFormatter,
    ILogFormatterB? LogFormatter
) {
    private static BatchingLoggerState? _Empty;

    public static BatchingLoggerState Empty() => _Empty ??= new BatchingLoggerState(
        IncludeScopes: false,
        ScopeProvider: default,
        IsEnabled: false,
        IncludeEventId: false,
        JsonWriterOptions: new JsonWriterOptions(),
        TimestampFormat: null,
        UseUtcTimestamp: false,
        UseLogFormatter: default,
        LogFormatter: default);
}