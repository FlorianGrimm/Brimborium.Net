using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Brimborium.Registrator.Internals {
    internal sealed class LifetimeSelector : ILifetimeSelector, ISelector {
        private readonly ServiceTypeSelector _Inner;

        private readonly ServicePopulation[] _Items;

        public ServiceLifetime? Lifetime { get; set; }

        internal LifetimeSelector(ServiceTypeSelector inner, IEnumerable<ServicePopulation> items) {
            this._Inner = inner;
            this._Items = items.ToArray();
        }

        public IImplementationTypeSelector WithSingletonLifetime() {
            this._Inner.PropagateLifetime(ServiceLifetime.Singleton);
            return this;
        }

        public IImplementationTypeSelector WithScopedLifetime() {
            this._Inner.PropagateLifetime(ServiceLifetime.Scoped);
            return this;
        }

        public IImplementationTypeSelector WithTransientLifetime() {
            this._Inner.PropagateLifetime(ServiceLifetime.Transient);
            return this;
        }

        public IImplementationTypeSelector WithLifetime(ServiceLifetime lifetime) {
            Preconditions.EnsureValidServiceLifetime(lifetime, nameof(lifetime));

            this._Inner.PropagateLifetime(lifetime);

            return this;
        }

        #region Chain Methods

        public IImplementationTypeSelector FromApplicationDependencies(DependencyContext context, Func<AssemblyName, bool>? predicate = default) {
            return this._Inner.FromApplicationDependencies(context, predicate);
        }

        public IImplementationTypeSelector FromAssemblyDependencies(DependencyContext context, Assembly assembly, Func<AssemblyName, bool>? predicate = default) {
            return this._Inner.FromAssemblyDependencies(context, assembly, predicate);
        }

        public IImplementationTypeSelector FromDependencyContext(DependencyContext context, Func<AssemblyName, bool>? predicate = default) {
            return this._Inner.FromDependencyContext(context, predicate);
        }

        public IImplementationTypeSelector FromAssemblyOf<T>() {
            return this._Inner.FromAssemblyOf<T>();
        }

        public IImplementationTypeSelector FromAssembliesOf(params Type[] types) {
            return this._Inner.FromAssembliesOf(types);
        }

        public IImplementationTypeSelector FromAssembliesOf(IEnumerable<Type> types) {
            return this._Inner.FromAssembliesOf(types);
        }

        public IImplementationTypeSelector FromAssemblies(params Assembly[] assemblies) {
            return this._Inner.FromAssemblies(assemblies);
        }

        public IImplementationTypeSelector FromAssemblies(IEnumerable<Assembly> assemblies) {
            return this._Inner.FromAssemblies(assemblies);
        }

        public IServiceTypeSelector AddClasses() {
            return this._Inner.AddClasses();
        }

        public IServiceTypeSelector AddClasses(bool publicOnly) {
            return this._Inner.AddClasses(publicOnly);
        }

        public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action) {
            return this._Inner.AddClasses(action);
        }

        public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action, bool publicOnly) {
            return this._Inner.AddClasses(action, publicOnly);
        }

        public ILifetimeSelector AsSelf() {
            return this._Inner.AsSelf();
        }

        public ILifetimeSelector As<T>() {
            return this._Inner.As<T>();
        }

        public ILifetimeSelector As(params Type[] types) {
            return this._Inner.As(types);
        }

        public ILifetimeSelector As(IEnumerable<Type> types) {
            return this._Inner.As(types);
        }

        public ILifetimeSelector AsImplementedInterfaces() {
            return this._Inner.AsImplementedInterfaces();
        }


        public ILifetimeSelector AsSelfWithInterfaces() {
            return this._Inner.AsSelfWithInterfaces();
        }

        public ILifetimeSelector AsMatchingInterface() {
            return this._Inner.AsMatchingInterface();
        }

        public ILifetimeSelector AsMatchingInterface(Action<TypeInfo, IImplementationTypeFilter> action) {
            return this._Inner.AsMatchingInterface(action);
        }

        public ILifetimeSelector As(Func<Type, IEnumerable<Type>> selector) {
            return this._Inner.As(selector);
        }

        public IImplementationTypeSelector UsingAttributes(Func<Type, Type, bool>? predicate = default) {
            return this._Inner.UsingAttributes(predicate);
        }

        public IServiceTypeSelector UsingRegistrationStrategy(RegistrationStrategy registrationStrategy) {
            return this._Inner.UsingRegistrationStrategy(registrationStrategy);
        }

        #endregion

        void ISelector.Populate(RegistrationStrategy strategy, ISelectorTarget selectorTarget) {
            if (!this.Lifetime.HasValue) {
                this.Lifetime = ServiceLifetime.Transient;
            }

            strategy ??= RegistrationStrategy.Append;

            foreach (var item in this._Items) {
                var itemLT = (item.Lifetime.HasValue) ? item : (item with { Lifetime = this.Lifetime });
                var itemSt = (itemLT.Strategy is null) ? itemLT : (itemLT with { Strategy = strategy });
                selectorTarget.AddServicePopulation(itemSt);
            }

            //foreach (var typeMap in this._TypeMaps) {
            //    var implementationType = typeMap.ImplementationType;
            //    var serviceTypes = typeMap.ServiceTypes.ToArray();
            //    foreach (var serviceType in serviceTypes) {

            //        if (!implementationType.IsAssignableTo(serviceType)) {
            //            throw new InvalidOperationException($@"Type ""{implementationType.ToFriendlyName()}"" is not assignable to ""${serviceType.ToFriendlyName()}"".");
            //        }

            //        //var descriptor = new ServiceDescriptor(serviceType, implementationType, this.Lifetime.Value);
            //        //strategy.Apply(services, descriptor);
            //    }
            //    selectorTarget.AddServicePopulation(
            //        new ServicePopulation(implementationType, serviceTypes, false, this.Lifetime.Value, strategy));
            //}

            //foreach (var typeFactoryMap in this._TypeFactoryMaps) {
            //    selectorTarget.AddServicePopulation(
            //            new ServicePopulation(typeFactoryMap.ImplementationType, typeFactoryMap.ServiceTypes.ToArray(), true, this.Lifetime.Value, strategy));
            //}
        }
    }
}
