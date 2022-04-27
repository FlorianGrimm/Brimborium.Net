// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Brimborium.Extensions.Logging.LocalFile;

public class LocalFileLoggerConfigureOptions : BatchLoggerConfigureOptions, IConfigureOptions<LocalFileLoggerOptions> {
    private readonly IWebAppContext _context;

    public LocalFileLoggerConfigureOptions(IConfiguration configuration, IWebAppContext context)
        : base(configuration, "LocalFileEnabled") {
        this._context = context;
    }

    public void Configure(LocalFileLoggerOptions options) {
        base.Configure(options);
        if (string.IsNullOrEmpty(options.LogDirectory)) {
            options.LogDirectory = Path.Combine(this._context.HomeFolder, "LogFiles", "Application");
        } else if (!System.IO.Path.IsPathRooted(options.LogDirectory)) {
            options.LogDirectory = System.IO.Path.GetFullPath(options.LogDirectory);
        }
    }
}

