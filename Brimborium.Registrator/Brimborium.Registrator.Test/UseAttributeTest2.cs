using Microsoft.Extensions.DependencyInjection;

using System;

using Xunit;

namespace Brimborium.Registrator.Test {


    public class UseAttributeTest2 {
        [Fact]
        public void UseAttribute_2_SameInstanceDifferentInterface() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddServicesWithRegistrator(a => {
                a.AddType<DummyImpl2AttributeSingleton>().UsingAttributes();
            });
            Assert.Contains(services, sd => sd.ServiceType == typeof(IA));
            Assert.Contains(services, sd => sd.ServiceType == typeof(IB));
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl2AttributeSingleton));

            using var provider = services.BuildServiceProvider();
            var ia = provider.GetRequiredService<IA>();
            var ia2 = provider.GetRequiredService<IA>();
            var ib = provider.GetRequiredService<IB>();
            Assert.Same(ia, ia2);
            Assert.Same(ia, ib);
        }

        [Fact]
        public void UseAttribute_2_ServiceTypeMode_Auto() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddServicesWithRegistrator(a => {
                a.AddType<DummyImpl2AttributeSingleton>().UsingAttributes();
            });
            Assert.Contains(services, sd => sd.ServiceType == typeof(IA) && sd.ImplementationFactory is not null);
            Assert.Contains(services, sd => sd.ServiceType == typeof(IB) && sd.ImplementationFactory is not null);
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl2AttributeSingleton) && sd.ImplementationType is not null);
        }

        [Fact]
        public void UseAttribute_ServiceTypeMode_Auto_IA() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddServicesWithRegistrator(a => {
                a.AddType<DummyImpl2IAAttributeSingletonAutoIA>().UsingAttributes();
            });
            Assert.Contains(services, sd => sd.ServiceType == typeof(IA) && sd.ImplementationType is not null);
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IB));
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(DummyImpl2IAAttributeSingletonAutoIA));
            Assert.Single(services, sd => sd.ServiceType == typeof(IA));
        }

        [Fact]
        public void UseAttribute_ServiceTypeMode_IncludeSelf() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddServicesWithRegistrator(a => {
                a.AddType<DummyImpl2AttributeSingletonIncludeSelf>().UsingAttributes();
            });
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IA));
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IB));
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl2AttributeSingletonIncludeSelf) && sd.ImplementationType is not null);
        }

        [Fact]
        public void UseAttribute_2_ServiceTypeMode_IncludeClasses() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddServicesWithRegistrator(a => {
                a.AddType<DummyImpl2AttributeSingletonIncludeClasses>().UsingAttributes();
            });
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IA));
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IB));
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl2AttributeSingletonIncludeClasses));
            Assert.Single(services, sd => sd.ServiceType == typeof(DummyImpl2AttributeSingletonIncludeClasses));
        }

        [Fact]
        public void UseAttribute_2_ServiceTypeMode_IncludeClasses_ServiceType() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddServicesWithRegistrator(a => {
                a.AddType<DummyImpl2AttributeSingletonIncludeClassesServiceType>().UsingAttributes();
            });
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IA));
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IB));
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl2AttributeSingletonIncludeClassesServiceType));
            Assert.Single(services, sd => sd.ServiceType == typeof(DummyImpl2AttributeSingletonIncludeClassesServiceType));
        }


        [Fact]
        public void UseAttribute_2_ServiceTypeMode_IncludeClasses_ServiceTypeIA() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddServicesWithRegistrator(a => {
                a.AddType<DummyImpl2AttributeSingletonIncludeClassesServiceTypeIA>().UsingAttributes();
            });
            Assert.Contains(services, sd => sd.ServiceType == typeof(IA) && sd.ImplementationFactory is not null);
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IB));
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl2AttributeSingletonIncludeClassesServiceTypeIA));
            Assert.Single(services, sd => sd.ServiceType == typeof(DummyImpl2AttributeSingletonIncludeClassesServiceTypeIA));
        }



        [Fact]
        public void UseAttribute_2_ServiceTypeMode_IncludeInterfaces() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddServicesWithRegistrator(a => {
                a.AddType<DummyImpl2AttributeSingletonIncludeInterfaces>().UsingAttributes();
            });
            Assert.Contains(services, sd => sd.ServiceType == typeof(IA) && sd.ImplementationFactory is not null);
            Assert.Contains(services, sd => sd.ServiceType == typeof(IB) && sd.ImplementationFactory is not null);
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl2AttributeSingletonIncludeInterfaces));
            Assert.Single(services, sd => sd.ServiceType == typeof(IA));
        }

        [Fact]
        public void UseAttribute_2_ServiceTypeMode_IncludeeInterfaces_ServiceType() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddServicesWithRegistrator(a => {
                a.AddType<DummyImpl2AttributeSingletonIncludeInterfacesServiceType>().UsingAttributes();
            });
            Assert.Contains(services, sd => sd.ServiceType == typeof(IA) && sd.ImplementationFactory is not null);
            Assert.Contains(services, sd => sd.ServiceType == typeof(IB) && sd.ImplementationFactory is not null);
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl2AttributeSingletonIncludeInterfacesServiceType));
            Assert.Single(services, sd => sd.ServiceType == typeof(IA));
        }

        [Fact]
        public void UseAttribute_2_ServiceTypeMode_IncludeClassesAndInterfaces() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddServicesWithRegistrator(a => {
                a.AddType<DummyImpl2AttributeSingletonIncludeClassesAndInterfaces>().UsingAttributes();
            });
            Assert.Contains(services, sd => sd.ServiceType == typeof(IA));
            Assert.Contains(services, sd => sd.ServiceType == typeof(IB));
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl2AttributeSingletonIncludeClassesAndInterfaces));
            Assert.Single(services, sd => sd.ServiceType == typeof(IA));
        }


        [Fact]
        public void UseAttribute_2_ServiceTypeMode_IncludeClassesAndInterfaces_ServiceType() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddServicesWithRegistrator(a => {
                a.AddType<DummyImpl2AttributeSingletonIncludeClassesAndInterfacesServiceType>().UsingAttributes();
            });
            Assert.Contains(services, sd => sd.ServiceType == typeof(IA));
            Assert.Contains(services, sd => sd.ServiceType == typeof(IB));
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl2AttributeSingletonIncludeClassesAndInterfacesServiceType));
            Assert.Single(services, sd => sd.ServiceType == typeof(IA));
        }

        [Fact]
        public void UseAttributeTwoDifferentLifetiems() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            Assert.Throws<InvalidOperationException>(() => {
                services.AddServicesWithRegistrator(a => {
                    a.AddType<DummyImplAttributeSingletonTransient>().UsingAttributes();
                });
            });
        }

    }
}
