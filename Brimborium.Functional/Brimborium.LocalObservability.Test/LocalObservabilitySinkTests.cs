#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
using System.Linq;

namespace Brimborium.LocalObservability;

public class LocalObservabilitySinkTests {
    [Fact]
    public void TestRead0() {
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddSimpleLocalObservabilitySink();
        var serviceProvider = services.BuildServiceProvider();
        var sutSink = serviceProvider.GetRequiredService<ILocalObservabilitySink>();
        var sutCollector = serviceProvider.GetRequiredService<ISimpleLocalObservabilityCollector>();

        Assert.Equal(0, sutCollector.Read().Count());
    }


    [Fact]
    public void TestRead1() {
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddSimpleLocalObservabilitySink();
        var serviceProvider = services.BuildServiceProvider();
        var sutSink = serviceProvider.GetRequiredService<ILocalObservabilitySink>();
        var sutCollector = serviceProvider.GetRequiredService<ISimpleLocalObservabilityCollector>();

        Assert.Equal(0, sutCollector.Read().Count());
        var c1 = new CodePoint();

        sutSink.Visit(c1);
        Assert.Equal(1, sutCollector.Read().Count());
    }

    [Fact]
    public void TestRead2() {
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddSimpleLocalObservabilitySink();
        var serviceProvider = services.BuildServiceProvider();
        var sutSink = serviceProvider.GetRequiredService<ILocalObservabilitySink>();
        var sutCollector = serviceProvider.GetRequiredService<ISimpleLocalObservabilityCollector>();

        Assert.Equal(0, sutCollector.Read().Count());
        var c1 = new CodePoint();
        var c2 = new CodePoint();

        sutSink.Visit(c1);
        sutSink.Visit(c2);
        Assert.Equal(2, sutCollector.Read().Count());
    }

    [Fact]
    public void TestRead2048() {
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddSimpleLocalObservabilitySink();
        var serviceProvider = services.BuildServiceProvider();
        var sutSink = serviceProvider.GetRequiredService<ILocalObservabilitySink>();
        var sutCollector = serviceProvider.GetRequiredService<ISimpleLocalObservabilityCollector>();

        Assert.Equal(0, sutCollector.Read().Count());
        var c1 = new CodePoint();
        var c2 = new CodePoint();

        for (var idx = 0; idx < 1024; idx++) {
            sutSink.Visit(c1);
            sutSink.Visit(c2);
        }

        Assert.Equal(1024, sutCollector.Read().Count());
    }

}