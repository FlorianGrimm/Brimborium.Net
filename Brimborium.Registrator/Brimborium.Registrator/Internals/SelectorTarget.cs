using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Brimborium.Registrator {
    internal sealed class SelectorTarget : ISelectorTarget {
        public List<ServicePopulation> Items { get; }

        public SelectorTarget() {
            this.Items = new List<ServicePopulation>();
        }

        public void AddServicePopulation(ServicePopulation value) {
            this.Items.Add(value);
        }

        public void Build(IServiceCollection services) {
            var groups = this.Items.GroupBy(sp => sp.ImplementationType);
            foreach (var grp in groups) {
                var implementationType = grp.Key;
                if (grp.Count() == 1) {
                    var servicePopulation = grp.First();
                    var strategy = (servicePopulation.Strategy ?? RegistrationStrategy.Append);
                    if (!servicePopulation.IsIncludeSelf
                        && (servicePopulation.ServiceTypes.Length == 1)) {
                        var descriptor = new ServiceDescriptor(
                            servicePopulation.ServiceTypes[0],
                            implementationType,
                            servicePopulation.Lifetime.GetValueOrDefault(ServiceLifetime.Transient));
                        strategy.Apply(services, descriptor);
                    } else {
                        {
                            var descriptor = new ServiceDescriptor(
                                implementationType,
                                implementationType,
                                servicePopulation.Lifetime.GetValueOrDefault(ServiceLifetime.Transient));
                            strategy.Apply(services, descriptor);
                        }
                        if (servicePopulation.ServiceTypes.Length > 0) {
                            Func<IServiceProvider, object> factory = (new FactoryOfType(implementationType)).factory;
                            foreach (var serviceType in servicePopulation.ServiceTypes) {
                                var descriptor = new ServiceDescriptor(
                                    serviceType,
                                    factory,
                                    servicePopulation.Lifetime.GetValueOrDefault(ServiceLifetime.Transient));
                                strategy.Apply(services, descriptor);
                            }
                        }
                    }
                } else {
                    var key = grp.Key;
                    var hsServiceTypes = new HashSet<Type>();
                    var isIncludeSelf = false;
                    foreach (var servicePopulation in grp) {
                        if (servicePopulation.IsIncludeSelf) {
                            isIncludeSelf = true;
                        }
                        foreach (var serviceType in servicePopulation.ServiceTypes) {
                            hsServiceTypes.Add(serviceType);
                        }
                    }
                    foreach (var servicePopulation in grp) {
                        var strategy = (servicePopulation.Strategy ?? RegistrationStrategy.Append);
                        if (!isIncludeSelf
                        && (hsServiceTypes.Count == 1)) {
                            var descriptor = new ServiceDescriptor(
                                servicePopulation.ServiceTypes[0],
                                implementationType,
                                servicePopulation.Lifetime.GetValueOrDefault(ServiceLifetime.Transient));
                            strategy.Apply(services, descriptor);
                        } else {
                            {
                                var descriptor = new ServiceDescriptor(
                                    implementationType,
                                    implementationType,
                                    servicePopulation.Lifetime.GetValueOrDefault(ServiceLifetime.Transient));
                                strategy.Apply(services, descriptor);
                            }
                            if (hsServiceTypes.Count > 0) {
                                Func<IServiceProvider, object> factory = (new FactoryOfType(implementationType)).factory;
                                foreach (var serviceType in hsServiceTypes) {
                                    var descriptor = new ServiceDescriptor(
                                        serviceType, 
                                        factory, 
                                        servicePopulation.Lifetime.GetValueOrDefault(ServiceLifetime.Transient));
                                    strategy.Apply(services, descriptor);
                                }
                            }
                        }
                    }
                }
            }
        }

        private record FactoryOfType(Type Type) {
            public object factory(IServiceProvider provider) {
                return provider.GetService(Type)!;
            }
        }
    }
}
