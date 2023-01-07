// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Brimborium.Extensions.Logging.LocalFile;

public class BatchLoggerConfigureOptions : IConfigureOptions<BatchingLoggerOptions> {
    protected readonly IConfiguration _configuration;
    protected readonly string _isEnabledKey;
    protected readonly bool _AlwaysEnabled;

    public BatchLoggerConfigureOptions(IConfiguration configuration, string isEnabledKey, bool alwaysEnabled) {
        this._configuration = configuration;
        this._isEnabledKey = isEnabledKey;
        this._AlwaysEnabled = alwaysEnabled;
    }

    public void Configure(BatchingLoggerOptions options) {
        options.IsEnabled = this._AlwaysEnabled || TextToBoolean(this._configuration.GetSection(this._isEnabledKey)?.Value);
    }

    protected static bool TextToBoolean(string? text) {
        if (string.IsNullOrEmpty(text) ||
            !bool.TryParse(text, out var result)) {
            result = false;
        }

        return result;
    }
}
