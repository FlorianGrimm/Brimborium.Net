// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using System.IO;

namespace Brimborium.Extensions.Logging.LocalFile;

public class AzureAppServicesFileLoggerConfigureOptions : BatchLoggerConfigureOptions, IConfigureOptions<AzureAppServicesFileLoggerOptions>
{
    private readonly IWebAppContext _context;

    public AzureAppServicesFileLoggerConfigureOptions(IConfiguration configuration, IWebAppContext context)
        : base(configuration, "AzureDriveEnabled")
    {
        this._context = context;
    }

    public void Configure(AzureAppServicesFileLoggerOptions options)
    {
        base.Configure(options);
        options.LogDirectory = Path.Combine(this._context.HomeFolder, "LogFiles", "Application");
    }
}