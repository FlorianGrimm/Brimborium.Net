using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Brimborium.Registrator;
using System.Linq;

namespace Brimborium.Registrator.Test {
   
    public class UseAttributeTest1 {
        [Fact]
        public void UseAttribute_1_SameInstanceDifferentInterface() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRegistrator(a=> {
                a.AddType<DummyImpl1AttributeSingletonIncludeClassesAndInterfaces>().UsingAttributes();
            });
            Assert.Contains(services, sd => sd.ServiceType == typeof(IA));
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl1AttributeSingletonIncludeClassesAndInterfaces));

            using var provider = services.BuildServiceProvider();
            var ia = provider.GetRequiredService<IA>();
            var ia2 = provider.GetRequiredService<IA>();
            var instance = provider.GetRequiredService<DummyImpl1AttributeSingletonIncludeClassesAndInterfaces>();
            Assert.Same(ia, ia2);
            Assert.Same(ia, instance);
        }

        [Fact]
        public void UseAttribute_1_ServiceTypeMode_Auto() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRegistrator(a => {
                a.AddType<DummyImpl1AttributeSingleton>().UsingAttributes();
            });
            Assert.Contains(services, sd => sd.ServiceType == typeof(IA) && sd.ImplementationType is not null);
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IB));
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(DummyImpl1AttributeSingleton));
        }

        [Fact]
        public void UseAttribute_ServiceTypeMode_Auto_IA() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRegistrator(a => {
                a.AddType<DummyImpl1IAAttributeSingletonAutoIA>().UsingAttributes();
            });
            Assert.Contains(services, sd => sd.ServiceType == typeof(IA) && sd.ImplementationType is not null);
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IB));
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(DummyImpl1IAAttributeSingletonAutoIA));
            Assert.Single(services, sd => sd.ServiceType == typeof(IA));
        }

        [Fact]
        public void UseAttribute_ServiceTypeMode_IncludeSelf() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRegistrator(a => {
                a.AddType<DummyImpl1AttributeSingletonIncludeSelf>().UsingAttributes();
            });
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IA));
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IB));
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl1AttributeSingletonIncludeSelf) && sd.ImplementationType is not null);
        }

        [Fact]
        public void UseAttribute_1_ServiceTypeMode_IncludeClasses() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRegistrator(a => {
                a.AddType<DummyImpl1AttributeSingletonIncludeClasses>().UsingAttributes();
            });
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IA));
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IB));
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl1AttributeSingletonIncludeClasses));
            Assert.Single(services, sd => sd.ServiceType == typeof(DummyImpl1AttributeSingletonIncludeClasses));
        }

        [Fact]
        public void UseAttribute_1_ServiceTypeMode_IncludeClasses_ServiceType() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRegistrator(a => {
                a.AddType<DummyImpl1AttributeSingletonIncludeClassesServiceType>().UsingAttributes();
            });
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IA));
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IB));
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl1AttributeSingletonIncludeClassesServiceType));
            Assert.Single(services, sd => sd.ServiceType == typeof(DummyImpl1AttributeSingletonIncludeClassesServiceType));
        }


        [Fact]
        public void UseAttribute_1_ServiceTypeMode_IncludeClasses_ServiceTypeIA() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRegistrator(a => {
                a.AddType<DummyImpl1AttributeSingletonIncludeClassesServiceTypeIA>().UsingAttributes();
            });
            Assert.Contains(services, sd => sd.ServiceType == typeof(IA) && sd.ImplementationFactory is not null);
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IB));
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl1AttributeSingletonIncludeClassesServiceTypeIA));
            Assert.Single(services, sd => sd.ServiceType == typeof(DummyImpl1AttributeSingletonIncludeClassesServiceTypeIA));
        }



        [Fact]
        public void UseAttribute_1_ServiceTypeMode_IncludeInterfaces() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRegistrator(a => {
                a.AddType<DummyImpl1AttributeSingletonIncludeInterfaces>().UsingAttributes();
            });
            Assert.Contains(services, sd => sd.ServiceType == typeof(IA) && sd.ImplementationType is not null);
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(DummyImpl1AttributeSingletonIncludeInterfacesServiceType));
            Assert.Single(services, sd => sd.ServiceType == typeof(IA));
        }

        [Fact]
        public void UseAttribute_1_ServiceTypeMode_IncludeeInterfaces_ServiceType() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRegistrator(a => {
                a.AddType<DummyImpl1AttributeSingletonIncludeInterfacesServiceType>().UsingAttributes();
            });
            Assert.Contains(services, sd => sd.ServiceType == typeof(IA) && sd.ImplementationType is not null);
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(DummyImpl1AttributeSingletonIncludeInterfacesServiceType));
            Assert.Single(services, sd => sd.ServiceType == typeof(IA));
        }

        [Fact]
        public void UseAttribute_1_ServiceTypeMode_IncludeClassesAndInterfaces() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRegistrator(a => {
                a.AddType<DummyImpl1AttributeSingletonIncludeClassesAndInterfaces>().UsingAttributes();
            });
            Assert.Contains(services, sd => sd.ServiceType == typeof(IA));
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IB));
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl1AttributeSingletonIncludeClassesAndInterfaces));
            Assert.Single(services, sd => sd.ServiceType == typeof(IA));
        }


        [Fact]
        public void UseAttribute_1_ServiceTypeMode_IncludeClassesAndInterfaces_ServiceType() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRegistrator(a => {
                a.AddType<DummyImpl1AttributeSingletonIncludeClassesAndInterfacesServiceType>().UsingAttributes();
            });
            Assert.Contains(services, sd => sd.ServiceType == typeof(IA));
            Assert.DoesNotContain(services, sd => sd.ServiceType == typeof(IB));
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl1AttributeSingletonIncludeClassesAndInterfacesServiceType));
            Assert.Single(services, sd => sd.ServiceType == typeof(IA));
        }

        [Fact]
        public void UseAttribute_1_ServiceTypeMode_IncludeSelfAndInterfaces() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRegistrator(a => {
                a.AddType<DummyImpl1AttributeSingletonIncludeSelfAndInterfaces>().UsingAttributes();
            });
            Assert.Contains(services, sd => sd.ServiceType == typeof(IA) && sd.ImplementationFactory is not null);
            Assert.Contains(services, sd => sd.ServiceType == typeof(DummyImpl1AttributeSingletonIncludeSelfAndInterfaces) && sd.ImplementationType is not null);
            Assert.Single(services, sd => sd.ServiceType == typeof(IA));
        }
        

        [Fact]
        public void UseAttributeTwoDifferentLifetiems() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            Assert.Throws<InvalidOperationException>(() => {
                services.AddRegistrator(a => {
                    a.AddType<DummyImplAttributeSingletonTransient>().UsingAttributes();
                });
            });
        }
        
    }
}
