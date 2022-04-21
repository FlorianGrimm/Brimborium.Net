// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Brimborium.Extensions.Logging.LocalFile;

public class FileLoggerConfigureOptions : BatchLoggerConfigureOptions, IConfigureOptions<AzureFileLoggerOptions>
{
    private readonly IWebAppContext _context;

    public FileLoggerConfigureOptions(IConfiguration configuration, IWebAppContext context)
        : base(configuration, "AzureDriveEnabled")
    {
        _context = context;
    }

    public void Configure(AzureFileLoggerOptions options)
    {
        base.Configure(options);
        options.LogDirectory = Path.Combine(_context.HomeFolder, "LogFiles", "Application");
    }
}
