using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace Brimborium.Registrator.Test {
    public class DependencyInjectionTest {
        [Fact]
        public void SameInstanceInterfaceTwice() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddSingleton<IA, DummyImpl2>();
            services.AddSingleton<IB, DummyImpl2>();
            using var provider = services.BuildServiceProvider();
            var ia = provider.GetRequiredService<IA>();
            var ia2 = provider.GetRequiredService<IA>();
            var ib = provider.GetRequiredService<IB>();
            Assert.Same(ia, ia2);
        }
        [Fact]
        public void AnotherInstanceDifferentInterface() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddSingleton<IA, DummyImpl2>();
            services.AddSingleton<IB, DummyImpl2>();
            using var provider = services.BuildServiceProvider();
            var ia = provider.GetRequiredService<IA>();
            var ia2 = provider.GetRequiredService<IA>();
            var ib = provider.GetRequiredService<IB>();
            Assert.Same(ia, ia2);
            Assert.NotSame(ia, ib);
        }
        [Fact]
        public void SameInstanceDifferentInterface() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddSingleton<DummyImpl2>();
            services.AddSingleton<IA, DummyImpl2>((provider) => provider.GetRequiredService<DummyImpl2>());
            services.AddSingleton<IB, DummyImpl2>((provider) => provider.GetRequiredService<DummyImpl2>());
            using var provider = services.BuildServiceProvider();
            var ia = provider.GetRequiredService<IA>();
            var ia2 = provider.GetRequiredService<IA>();
            var ib = provider.GetRequiredService<IB>();
            Assert.Same(ia, ia2);
            Assert.Same(ia, ib);
        }
    }
}
