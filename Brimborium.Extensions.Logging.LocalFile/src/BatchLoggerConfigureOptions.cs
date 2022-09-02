// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Brimborium.Extensions.Logging.LocalFile;

public class BatchLoggerConfigureOptions : IConfigureOptions<BatchingLoggerOptions> {
    protected readonly IConfiguration _configuration;
    protected readonly string _isEnabledKey;

    public BatchLoggerConfigureOptions(IConfiguration configuration, string isEnabledKey) {
        this._configuration = configuration;
        this._isEnabledKey = isEnabledKey;
    }

    public void Configure(BatchingLoggerOptions options) {
        options.IsEnabled = TextToBoolean(this._configuration.GetSection(this._isEnabledKey)?.Value);
        if (this._isEnabledKey == "LocalFileEnabled") { 
            options.IncludeScopes = TextToBoolean(this._configuration.GetSection("LocalFileIncludeScopes")?.Value);
            options.TimestampFormat = this._configuration.GetSection("LocalFileTimestampFormat")?.Value;
            options.TimestampFormat = this._configuration.GetSection("LocalFile")?.Value;
            options.UseUtcTimestamp = TextToBoolean(this._configuration.GetSection("LocalFileUseUtcTimestamp")?.Value);
            options.IncludeEventId = TextToBoolean(this._configuration.GetSection("LocalFileIncludeEventId")?.Value);
            options.UseJSONFormat = TextToBoolean(this._configuration.GetSection("LocalFileUseJSONFormat")?.Value);
        }
    }

    private static bool TextToBoolean(string text) {
        if (string.IsNullOrEmpty(text) ||
            !bool.TryParse(text, out var result)) {
            result = false;
        }

        return result;
    }
}
