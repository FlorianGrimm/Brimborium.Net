using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Brimborium.Registrator.Internals {
    internal class AttributeSelector : ISelector {
        private readonly IEnumerable<Type> _Types;
        private readonly Func<Type, Type, bool> _Predicate;

        private static bool defaultPredicate(Type type, Type serviceType) {
            if (typeof(System.IDisposable).Equals(serviceType)) { return false; }
            if (typeof(System.IAsyncDisposable).Equals(serviceType)) { return false; }
            return true;
        }

        public AttributeSelector(IEnumerable<Type> types, Func<Type, Type, bool>? predicate) {
            this._Types = types;
            this._Predicate = predicate ?? defaultPredicate;
        }


        void ISelector.Populate(IServiceCollection services, RegistrationStrategy registrationStrategy) {
            var strategy = registrationStrategy ?? RegistrationStrategy.Append;

            foreach (var implementationType in this._Types) {
                var typeInfo = implementationType.GetTypeInfo();

                var attributes = typeInfo.GetCustomAttributes<ServiceDescriptorAttribute>();
                if (attributes.Any()) {
                    var distLifetime = attributes.Select(sda => sda.Lifetime).Distinct();
                    if (distLifetime.Skip(1).Any()) {
                        var txtLifetime = string.Join(", ", distLifetime.Select(l => l.ToString()));

                        throw new InvalidOperationException($@"Type ""{implementationType.ToFriendlyName()}"" has multiple Lifetimes ""{txtLifetime}"".");
                    }
                    var lifetime = distLifetime.First();

                    var isIncludeSelf = false;
                    var isIncludeInterfaces = false;
                    var isIncludeClasses = false;
                    var hsServiceTypes = new HashSet<Type>();
                    foreach (var sda in attributes) {
                        if (sda.Mode == ServiceTypeMode.Auto) {
                            if (sda.ServiceType is not null) {
                                // hsServiceTypes.Add(sda.ServiceType);
                            } else {
                                isIncludeInterfaces = true;
                            }
                        } else {
                            if (sda.Mode.HasFlag(ServiceTypeMode.IncludeSelf)) {
                                isIncludeSelf = true;
                            }
                            if (sda.Mode.HasFlag(ServiceTypeMode.IncludeInterfaces)) {
                                isIncludeInterfaces = true;
                            }
                            if (sda.Mode.HasFlag(ServiceTypeMode.IncludeClasses)) {
                                isIncludeClasses = true;
                            }

                        }
                        if (sda.ServiceType is not null) {
                            if (implementationType.IsAssignableTo(sda.ServiceType)) {
                                hsServiceTypes.Add(sda.ServiceType);
                            } else {
                                throw new InvalidOperationException($@"Type ""{implementationType.ToFriendlyName()}"" is not assignable to ""{sda.ServiceType.ToFriendlyName()}"".");
                            }
                        }
                    }

                    if (isIncludeInterfaces || isIncludeClasses) {
                        foreach (var serviceType in implementationType.GetBaseTypes(isIncludeInterfaces, isIncludeClasses)) {
                            hsServiceTypes.Add(serviceType);
                        }
                    }

                    if (!isIncludeSelf && hsServiceTypes.Count == 1) {
                        var descriptor = new ServiceDescriptor(hsServiceTypes.First(), implementationType, lifetime);
                        strategy.Apply(services, descriptor);
                    } else {
                        {
                            var descriptor = new ServiceDescriptor(implementationType, implementationType, lifetime);
                            strategy.Apply(services, descriptor);
                        }
                        if (hsServiceTypes.Count > 0) {
                            Func<IServiceProvider, object> factory = (new FactoryOfType(implementationType)).factory;
                            foreach (var serviceType in hsServiceTypes) {
                                var descriptor = new ServiceDescriptor(serviceType, factory, lifetime);
                                strategy.Apply(services, descriptor);
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
