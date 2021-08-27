using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Brimborium.Registrator.Test {
    public class DependencyInjectionTest {
        [Fact]
        public void SameInstanceInterfaceTwice() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddSingleton<IA, DummyImpl>();
            services.AddSingleton<IB, DummyImpl>();
            using var provider = services.BuildServiceProvider();
            var ia = provider.GetRequiredService<IA>();
            var ia2 = provider.GetRequiredService<IA>();
            var ib = provider.GetRequiredService<IB>();
            Assert.Same(ia, ia2);
        }
        [Fact]
        public void AnotherInstanceDifferentInterface() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddSingleton<IA, DummyImpl>();
            services.AddSingleton<IB, DummyImpl>();
            using var provider = services.BuildServiceProvider();
            var ia = provider.GetRequiredService<IA>();
            var ia2 = provider.GetRequiredService<IA>();
            var ib = provider.GetRequiredService<IB>();
            Assert.Same(ia, ia2);
            Assert.NotSame(ia, ib);
        }
        [Fact]
        public void AnotherInstanceSameInterface() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddSingleton<DummyImpl>();
            services.AddSingleton<IA, DummyImpl>((provider)=>provider.GetRequiredService< DummyImpl>());
            services.AddSingleton<IB, DummyImpl>((provider) => provider.GetRequiredService<DummyImpl>());
            using var provider = services.BuildServiceProvider();
            var ia = provider.GetRequiredService<IA>();
            var ia2 = provider.GetRequiredService<IA>();
            var ib = provider.GetRequiredService<IB>();
            Assert.Same(ia, ia2);
            Assert.Same(ia, ib);
        }
        public interface IA { }
        public interface IB { }
        public class DummyImpl : IA, IB{ }
    }
}
