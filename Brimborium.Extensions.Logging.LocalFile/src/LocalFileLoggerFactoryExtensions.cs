// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

using static Microsoft.Extensions.DependencyInjection.ServiceDescriptor;

namespace Brimborium.Extensions.Logging.LocalFile;

/// <summary>
/// Extension methods for adding Azure diagnostics logger.
/// </summary>
public static class LocalFileLoggerFactoryExtensions {
    /// <summary>
    /// Adds an Azure Web Apps diagnostics logger.
    /// </summary>
    /// <param name="builder">The extension method argument</param>
    /// <returns></returns>
    public static ILoggingBuilder AddWebAppDiagnostics(this ILoggingBuilder builder) {
        var context = WebAppContext.Default;

        // Only add the provider if we're in Azure WebApp. That cannot change once the apps started
        return builder.AddWebAppDiagnostics(context, _ => { });
    }

    /// <summary>
    /// Adds an Azure Web Apps diagnostics logger.
    /// </summary>
    /// <param name="builder">The extension method argument.</param>
    /// <param name="configureBlobLoggerOptions">An Action to configure the <see cref="AzureBlobLoggerOptions"/>.</param>
    /// <returns></returns>
    public static ILoggingBuilder AddWebAppDiagnostics(this ILoggingBuilder builder, Action<AzureBlobLoggerOptions> configureBlobLoggerOptions) {
        var context = WebAppContext.Default;

        // Only add the provider if we're in Azure WebApp. That cannot change once the apps started
        return builder.AddWebAppDiagnostics(context, configureBlobLoggerOptions);
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static ILoggingBuilder AddWebAppDiagnostics(this ILoggingBuilder builder, IWebAppContext context, Action<AzureBlobLoggerOptions> configureBlobLoggerOptions) {
        var isRunningInAzureWebApp = context.IsRunningInAzureWebApp;

        builder.AddConfiguration();

        var config = SiteConfigurationProvider.GetAzureLoggingConfiguration(context);
        var services = builder.Services;

        var addedAzureFileLogger = (isRunningInAzureWebApp) && TryAddEnumerable(services, Singleton<ILoggerProvider, AzureAppServicesFileLoggerProvider>());
        var addedBlobLogger = (isRunningInAzureWebApp) && TryAddEnumerable(services, Singleton<ILoggerProvider, BlobLoggerProvider>());
        var addedLocalFileLogger = TryAddEnumerable(services, Singleton<ILoggerProvider, LocalFileLoggerProvider>());

        if (addedAzureFileLogger || addedBlobLogger || addedLocalFileLogger) {
            services.AddSingleton(context);
            services.AddSingleton<IOptionsChangeTokenSource<LoggerFilterOptions>>(
                new ConfigurationChangeTokenSource<LoggerFilterOptions>(config));
        }

        if (addedAzureFileLogger) {
            services.AddSingleton<IConfigureOptions<LoggerFilterOptions>>(CreateAzureFileFilterConfigureOptions(config));
            services.AddSingleton<IConfigureOptions<AzureAppServicesFileLoggerOptions>>(new AzureAppServicesFileLoggerConfigureOptions(config, context));
            services.AddSingleton<IOptionsChangeTokenSource<AzureAppServicesFileLoggerOptions>>(
                new ConfigurationChangeTokenSource<AzureAppServicesFileLoggerOptions>(config));
            LoggerProviderOptions.RegisterProviderOptions<AzureAppServicesFileLoggerOptions, AzureAppServicesFileLoggerProvider>(builder.Services);
        }

        if (addedBlobLogger) {
            services.AddSingleton<IConfigureOptions<LoggerFilterOptions>>(CreateBlobFilterConfigureOptions(config));
            services.AddSingleton<IConfigureOptions<AzureBlobLoggerOptions>>(new BlobLoggerConfigureOptions(config, context, configureBlobLoggerOptions));
            services.AddSingleton<IOptionsChangeTokenSource<AzureBlobLoggerOptions>>(
                new ConfigurationChangeTokenSource<AzureBlobLoggerOptions>(config));
            LoggerProviderOptions.RegisterProviderOptions<AzureBlobLoggerOptions, BlobLoggerProvider>(builder.Services);
        }

        if (addedLocalFileLogger) {
            services.AddSingleton<IConfigureOptions<LoggerFilterOptions>>(CreateLocalFileFilterConfigureOptions(config));
            services.AddSingleton<IConfigureOptions<LocalFileLoggerOptions>>(new AzureAppServicesFileLoggerConfigureOptions(config, context));
            services.AddSingleton<IOptionsChangeTokenSource<LocalFileLoggerOptions>>(
                new ConfigurationChangeTokenSource<LocalFileLoggerOptions>(config));
            LoggerProviderOptions.RegisterProviderOptions<LocalFileLoggerOptions, LocalFileLoggerProvider>(builder.Services);
        }

        return builder;
    }

    private static bool TryAddEnumerable(IServiceCollection collection, ServiceDescriptor descriptor) {
        var beforeCount = collection.Count;
        collection.TryAddEnumerable(descriptor);
        return beforeCount != collection.Count;
    }

    private static ConfigurationBasedLevelSwitcher CreateBlobFilterConfigureOptions(IConfiguration config) {
        return new ConfigurationBasedLevelSwitcher(
            configuration: config,
            provider: typeof(BlobLoggerProvider),
            levelKey: "AzureBlobTraceLevel");
    }

    private static ConfigurationBasedLevelSwitcher CreateAzureFileFilterConfigureOptions(IConfiguration config) {
        return new ConfigurationBasedLevelSwitcher(
            configuration: config,
            provider: typeof(AzureAppServicesFileLoggerProvider),
            levelKey: "AzureDriveTraceLevel");
    }

    private static ConfigurationBasedLevelSwitcher CreateLocalFileFilterConfigureOptions(IConfiguration config) {
        return new ConfigurationBasedLevelSwitcher(
            configuration: config,
            provider: typeof(AzureAppServicesFileLoggerProvider),
            levelKey: "LocalFileTraceLevel");
    }
}
