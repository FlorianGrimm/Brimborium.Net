// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Brimborium.Extensions.Logging.LocalFile;

public class BatchLoggerConfigureOptions : IConfigureOptions<BatchingLoggerOptions>
{
    private readonly IConfiguration _configuration;
    private readonly string _isEnabledKey;

    public BatchLoggerConfigureOptions(IConfiguration configuration, string isEnabledKey)
    {
        this._configuration = configuration;
        this._isEnabledKey = isEnabledKey;
    }

    public void Configure(BatchingLoggerOptions options)
    {
        options.IsEnabled = TextToBoolean(this._configuration.GetSection(this._isEnabledKey)?.Value);
    }

    private static bool TextToBoolean(string text)
    {
        if (string.IsNullOrEmpty(text) ||
            !bool.TryParse(text, out var result))
        {
            result = false;
        }

        return result;
    }
}
