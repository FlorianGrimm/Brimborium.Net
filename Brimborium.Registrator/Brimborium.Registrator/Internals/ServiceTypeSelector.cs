using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Brimborium.Registrator.Internals;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Brimborium.Registrator {
    internal class ServiceTypeSelector : IServiceTypeSelector, ISelector {
        private IImplementationTypeSelector _Inner;
        private IEnumerable<Type> _Types;
        private List<ISelector> _Selectors;
        private RegistrationStrategy? RegistrationStrategy;

        public ServiceTypeSelector(IImplementationTypeSelector inner, IEnumerable<Type> types) {
            this._Inner = inner;
            this._Types = types;
            this._Selectors = new List<ISelector>();
        }

        public ILifetimeSelector AsSelf() {
            return this.As(t => new[] { t });
        }

        public ILifetimeSelector As<T>() {
            return this.As(typeof(T));
        }

        public ILifetimeSelector As(params Type[] types) {
            Preconditions.NotNull(types, nameof(types));

            return this.As(types.AsEnumerable());
        }

        public ILifetimeSelector As(IEnumerable<Type> types) {
            Preconditions.NotNull(types, nameof(types));

            return this.AddSelector(this._Types.Select(t => new ServicePopulation(t, types.ToArray(), false, null, null)));
        }

        public ILifetimeSelector AsImplementedInterfaces() {
            return this.AsTypeInfo(t => t.ImplementedInterfaces
                .Where(x => x.HasMatchingGenericArity(t))
                .Select(x => x.GetRegistrationType(t)));
        }

        public ILifetimeSelector AsSelfWithInterfaces() {
            IEnumerable<Type> Selector(TypeInfo info) {
                if (info.IsGenericTypeDefinition) {
                    // This prevents trying to register open generic types
                    // with an ImplementationFactory, which is unsupported.
                    return Enumerable.Empty<Type>();
                }

                return info.ImplementedInterfaces
                    .Where(x => x.HasMatchingGenericArity(info))
                    .Select(x => x.GetRegistrationType(info));
            }

            //return this.AddSelector(
            //    this._Types.Select(t => new TypeMap(t, new[] { t })),
            //    this._Types.Select(t => new TypeFactoryMap(t, Selector(t.GetTypeInfo()))));

            return this.AddSelector(
                this._Types.Select(t => new ServicePopulation(t, Selector(t.GetTypeInfo()).ToArray(), true, null, null)));
        }

        public ILifetimeSelector AsMatchingInterface() {
            return this.AsMatchingInterface(null);
        }

        public ILifetimeSelector AsMatchingInterface(Action<TypeInfo, IImplementationTypeFilter>? action) {
            return this.AsTypeInfo(t => t.FindMatchingInterface(action));
        }

        public ILifetimeSelector As(Func<Type, IEnumerable<Type>> selector) {
            Preconditions.NotNull(selector, nameof(selector));

            return this.AddSelector(this._Types.Select(t => new ServicePopulation(t, selector(t).ToArray(), false, null, null)));
        }

        public IImplementationTypeSelector UsingAttributes(Func<Type, Type, bool>? predicate = default) {
            var selector = new AttributeSelector(this._Types, predicate);

            this._Selectors.Add(selector);

            return this;
        }

        public IServiceTypeSelector UsingRegistrationStrategy(RegistrationStrategy registrationStrategy) {
            Preconditions.NotNull(registrationStrategy, nameof(registrationStrategy));

            this.RegistrationStrategy = registrationStrategy;
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

        #endregion

        internal void PropagateLifetime(ServiceLifetime lifetime) {
            foreach (var selector in this._Selectors.OfType<LifetimeSelector>()) {
                selector.Lifetime = lifetime;
            }
        }

        void ISelector.Populate(RegistrationStrategy registrationStrategy, ISelectorTarget selectorTarget) {
            if (this._Selectors.Count == 0) {
                this.AsSelf();
            }

            var strategy = this.RegistrationStrategy ?? registrationStrategy;

            foreach (var selector in this._Selectors) {
                selector.Populate(strategy, selectorTarget);
            }
        }

        private ILifetimeSelector AddSelector(IEnumerable<ServicePopulation> items) {
            var selector = new LifetimeSelector(this, items);

            this._Selectors.Add(selector);

            return selector;
        }

        private ILifetimeSelector AsTypeInfo(Func<TypeInfo, IEnumerable<Type>> selector) {
            return this.As(t => selector(t.GetTypeInfo()));
        }
    }
}
