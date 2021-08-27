using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.DependencyInjection;

namespace Brimborium.Registrator.Internals {
    internal class AttributeSelector : ISelector {
        private readonly IEnumerable<Type> _Types;
        private readonly Func<Type, Type, bool> _Predicate;

        private static bool defaultPredicate(Type type, Type serviceType) {
            return !(typeof(System.IDisposable).Equals(serviceType));
            
        }

        public AttributeSelector(IEnumerable<Type> types, Func<Type, Type, bool>? predicate) {
            this._Types = types;
            this._Predicate = predicate ?? defaultPredicate;
        }


        void ISelector.Populate(IServiceCollection services, RegistrationStrategy registrationStrategy) {
            var strategy = registrationStrategy ?? RegistrationStrategy.Append;

            foreach (var type in this._Types) {
                var typeInfo = type.GetTypeInfo();

                var attributes = typeInfo.GetCustomAttributes<ServiceDescriptorAttribute>().Concat(
                        typeInfo.GetCustomAttributes<SingletonAttribute>()
                    ).Concat(
                        typeInfo.GetCustomAttributes<ScopedAttribute>()
                    ).Concat(
                        typeInfo.GetCustomAttributes<TransientAttribute>()
                    )
                    .ToArray();

                List<AttributeServiceType> attributeServiceTypes = attributes
                    .SelectMany(attribute =>
                        attribute.GetServiceTypes(type)
                            .Where(serviceType => this._Predicate(type, serviceType))
                            .Select(serviceType => new AttributeServiceType(attribute, serviceType))
                    ).ToList();

                // Check if the type has multiple attributes with same ServiceType.
                var duplicates = attributeServiceTypes.GroupBy(ast => ast.ServiceType).Where(grp => grp.Skip(1).Any());
                if (duplicates.Any()) {
                    var serviceTypeNames = string.Join(", ", duplicates.Select(grp => grp.Key.ToFriendlyName()));

                    throw new InvalidOperationException($@"Type ""{type.ToFriendlyName()}"" has multiple ServiceDescriptor attributes with the same service type ""{serviceTypeNames}"".");
                }

                if (attributeServiceTypes.Count == 1) {
                    var ast = attributeServiceTypes[0];
                    var descriptor = new ServiceDescriptor(ast.ServiceType, type, ast.Attribute.Lifetime);
                    strategy.Apply(services, descriptor);
                } else {
                    var distLifetime = attributeServiceTypes.Select(ast => ast.Attribute.Lifetime).Distinct();
                    if (distLifetime.Count() == 1) {
                        var lifetime = distLifetime.First();
                        {
                            var descriptor = new ServiceDescriptor(type, type, lifetime);
                            strategy.Apply(services, descriptor);
                        }
                        var factory = getFactory(type);
                        foreach (var ast in attributeServiceTypes) {
                            var descriptor = new ServiceDescriptor(ast.ServiceType, factory, lifetime);
                            strategy.Apply(services, descriptor);
                        }
                    } else {
                        // error ?
                        foreach (var ast in attributeServiceTypes) {
                            var descriptor = new ServiceDescriptor(ast.ServiceType, type, ast.Attribute.Lifetime);
                            strategy.Apply(services, descriptor);
                        }
                    }
                }
            }
        }

        static Func<IServiceProvider, object> getFactory(Type type) {
            return factory;

            object factory(IServiceProvider provider) {
                return provider.GetService(type)!;
            }
        }


        private record AttributeServiceType(ServiceDescriptorAttribute Attribute, Type ServiceType);
    }
}
