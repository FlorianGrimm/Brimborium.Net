using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Brimborium.Registrator.Internals {
    internal sealed class LifetimeSelector : ILifetimeSelector, ISelector {
        public LifetimeSelector(ServiceTypeSelector inner, IEnumerable<TypeMap> typeMaps, IEnumerable<TypeFactoryMap> typeFactoryMaps) {
            this.Inner = inner;
            this.TypeMaps = typeMaps;
            this.TypeFactoryMaps = typeFactoryMaps;
        }

        private ServiceTypeSelector Inner { get; }

        private IEnumerable<TypeMap> TypeMaps { get; }

        private IEnumerable<TypeFactoryMap> TypeFactoryMaps { get; }

        public ServiceLifetime? Lifetime { get; set; }

        public IImplementationTypeSelector WithSingletonLifetime() {
            this.Inner.PropagateLifetime(ServiceLifetime.Singleton);
            return this;
        }

        public IImplementationTypeSelector WithScopedLifetime() {
            this.Inner.PropagateLifetime(ServiceLifetime.Scoped);
            return this;
        }

        public IImplementationTypeSelector WithTransientLifetime() {
            this.Inner.PropagateLifetime(ServiceLifetime.Transient);
            return this;
        }

        public IImplementationTypeSelector WithLifetime(ServiceLifetime lifetime) {
            Preconditions.EnsureValidServiceLifetime(lifetime, nameof(lifetime));

            this.Inner.PropagateLifetime(lifetime);

            return this;
        }

        #region Chain Methods

        public IImplementationTypeSelector FromCallingAssembly() {
            return this.Inner.FromCallingAssembly();
        }

        public IImplementationTypeSelector FromExecutingAssembly() {
            return this.Inner.FromExecutingAssembly();
        }

        public IImplementationTypeSelector FromEntryAssembly() {
            return this.Inner.FromEntryAssembly();
        }

        public IImplementationTypeSelector FromApplicationDependencies() {
            return this.Inner.FromApplicationDependencies();
        }

        public IImplementationTypeSelector FromApplicationDependencies(Func<AssemblyName, bool> predicate) {
            return this.Inner.FromApplicationDependencies(predicate);
        }

        public IImplementationTypeSelector FromAssemblyDependencies(Assembly assembly) {
            return this.Inner.FromAssemblyDependencies(assembly);
        }

        public IImplementationTypeSelector FromDependencyContext(DependencyContext context) {
            return this.Inner.FromDependencyContext(context);
        }

        public IImplementationTypeSelector FromDependencyContext(DependencyContext context, Func<AssemblyName, bool> predicate) {
            return this.Inner.FromDependencyContext(context, predicate);
        }

        public IImplementationTypeSelector FromAssemblyOf<T>() {
            return this.Inner.FromAssemblyOf<T>();
        }

        public IImplementationTypeSelector FromAssembliesOf(params Type[] types) {
            return this.Inner.FromAssembliesOf(types);
        }

        public IImplementationTypeSelector FromAssembliesOf(IEnumerable<Type> types) {
            return this.Inner.FromAssembliesOf(types);
        }

        public IImplementationTypeSelector FromAssemblies(params Assembly[] assemblies) {
            return this.Inner.FromAssemblies(assemblies);
        }

        public IImplementationTypeSelector FromAssemblies(IEnumerable<Assembly> assemblies) {
            return this.Inner.FromAssemblies(assemblies);
        }

        public IServiceTypeSelector AddClasses() {
            return this.Inner.AddClasses();
        }

        public IServiceTypeSelector AddClasses(bool publicOnly) {
            return this.Inner.AddClasses(publicOnly);
        }

        public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action) {
            return this.Inner.AddClasses(action);
        }

        public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action, bool publicOnly) {
            return this.Inner.AddClasses(action, publicOnly);
        }

        public ILifetimeSelector AsSelf() {
            return this.Inner.AsSelf();
        }

        public ILifetimeSelector As<T>() {
            return this.Inner.As<T>();
        }

        public ILifetimeSelector As(params Type[] types) {
            return this.Inner.As(types);
        }

        public ILifetimeSelector As(IEnumerable<Type> types) {
            return this.Inner.As(types);
        }

        public ILifetimeSelector AsImplementedInterfaces() {
            return this.Inner.AsImplementedInterfaces();
        }

        public ILifetimeSelector AsSelfWithInterfaces() {
            return this.Inner.AsSelfWithInterfaces();
        }

        public ILifetimeSelector AsMatchingInterface() {
            return this.Inner.AsMatchingInterface();
        }

        public ILifetimeSelector AsMatchingInterface(Action<TypeInfo, IImplementationTypeFilter> action) {
            return this.Inner.AsMatchingInterface(action);
        }

        public ILifetimeSelector As(Func<Type, IEnumerable<Type>> selector) {
            return this.Inner.As(selector);
        }

        public IImplementationTypeSelector UsingAttributes(Func<Type, Type, bool>? predicate = default) {
            return this.Inner.UsingAttributes(predicate);
        }

        public IServiceTypeSelector UsingRegistrationStrategy(RegistrationStrategy registrationStrategy) {
            return this.Inner.UsingRegistrationStrategy(registrationStrategy);
        }

        #endregion

        void ISelector.Populate(IServiceCollection services, RegistrationStrategy strategy) {
            if (!this.Lifetime.HasValue) {
                this.Lifetime = ServiceLifetime.Transient;
            }

            strategy ??= RegistrationStrategy.Append;

            foreach (var typeMap in this.TypeMaps) {
                foreach (var serviceType in typeMap.ServiceTypes) {
                    var implementationType = typeMap.ImplementationType;

                    if (!implementationType.IsAssignableTo(serviceType)) {
                        throw new InvalidOperationException($@"Type ""{implementationType.ToFriendlyName()}"" is not assignable to ""${serviceType.ToFriendlyName()}"".");
                    }

                    var descriptor = new ServiceDescriptor(serviceType, implementationType, this.Lifetime.Value);

                    strategy.Apply(services, descriptor);
                }
            }

            foreach (var typeFactoryMap in this.TypeFactoryMaps) {
                foreach (var serviceType in typeFactoryMap.ServiceTypes) {
                    var descriptor = new ServiceDescriptor(serviceType, typeFactoryMap.ImplementationFactory, this.Lifetime.Value);

                    strategy.Apply(services, descriptor);
                }
            }
        }
    }
}
