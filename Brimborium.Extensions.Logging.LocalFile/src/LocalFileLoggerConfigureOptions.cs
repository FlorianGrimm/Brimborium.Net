// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Brimborium.Extensions.Logging.LocalFile;

public class LocalFileLoggerConfigureOptions : BatchLoggerConfigureOptions, IConfigureOptions<LocalFileLoggerOptions> {
    private readonly IWebAppContext _context;

    public LocalFileLoggerConfigureOptions(
        IConfiguration configuration, 
        IWebAppContext context, 
        bool alwaysEnabled=false)
        : base(configuration, "LocalFileEnabled", alwaysEnabled) {
        this._context = context;
    }

    public void Configure(LocalFileLoggerOptions options) {
        base.Configure(options);
        
        options.IncludeScopes = TextToBoolean(this._configuration.GetSection("LocalFileIncludeScopes")?.Value);
        options.TimestampFormat = this._configuration.GetSection("LocalFileTimestampFormat")?.Value;
        options.TimestampFormat = this._configuration.GetSection("LocalFile")?.Value;
        options.UseUtcTimestamp = TextToBoolean(this._configuration.GetSection("LocalFileUseUtcTimestamp")?.Value);
        options.IncludeEventId = TextToBoolean(this._configuration.GetSection("LocalFileIncludeEventId")?.Value);

#warning HERE
        //options.UseJSONFormat = TextToBoolean(this._configuration.GetSection("LocalFileUseJSONFormat")?.Value);

        {
            var logDirectory = this._configuration.GetSection("LocalFileDirectory")?.Value;
            if (!string.IsNullOrEmpty(logDirectory)) {
                options.LogDirectory = logDirectory;
            }
        }
        if (string.IsNullOrEmpty(options.LogDirectory)) {
            options.LogDirectory = Path.Combine(this._context.HomeFolder ?? ".", "LogFiles", "Application");
        } 
        if (!System.IO.Path.IsPathRooted(options.LogDirectory)) {
            options.LogDirectory = System.IO.Path.GetFullPath(options.LogDirectory);
        }
    }
}

