// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Xunit;

namespace Brimborium.Extensions.Logging.LocalFile.Test;

public class WebConfigurationLevelSwitchTests
{
    [Theory]
    [InlineData("Error", LogLevel.Error)]
    [InlineData("Warning", LogLevel.Warning)]
    [InlineData("Information", LogLevel.Information)]
    [InlineData("Verbose", LogLevel.Trace)]
    [InlineData("ABCD", LogLevel.None)]
    public void AddsRuleWithCorrectLevel(string levelValue, LogLevel expectedLevel)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(
            new[]
            {
                    new KeyValuePair<string, string>("levelKey", levelValue),
            })
            .Build();

        var levelSwitcher = new ConfigurationBasedLevelSwitcher(configuration, typeof(AzureAppServicesTestFileLoggerProvider), "levelKey");

        var filterConfiguration = new LoggerFilterOptions();
        levelSwitcher.Configure(filterConfiguration);

        Assert.Equal(1, filterConfiguration.Rules.Count);

        var rule = filterConfiguration.Rules[0];
        Assert.Equal(typeof(AzureAppServicesTestFileLoggerProvider).FullName, rule.ProviderName);
        Assert.Equal(expectedLevel, rule.LogLevel);
    }
}
