using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Brimborium.Registrator.Internals;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Brimborium.Registrator {
    internal class ServiceTypeSelector : IServiceTypeSelector, ISelector
    {
        public ServiceTypeSelector(IImplementationTypeSelector inner, IEnumerable<Type> types)
        {
            this.Inner = inner;
            this.Types = types;
        }

        private IImplementationTypeSelector Inner { get; }

        private IEnumerable<Type> Types { get; }

        private List<ISelector> Selectors { get; } = new List<ISelector>();

        private RegistrationStrategy? RegistrationStrategy { get; set; }

        public ILifetimeSelector AsSelf()
        {
            return this.As(t => new[] { t });
        }

        public ILifetimeSelector As<T>()
        {
            return this.As(typeof(T));
        }

        public ILifetimeSelector As(params Type[] types)
        {
            Preconditions.NotNull(types, nameof(types));

            return this.As(types.AsEnumerable());
        }

        public ILifetimeSelector As(IEnumerable<Type> types)
        {
            Preconditions.NotNull(types, nameof(types));

            return this.AddSelector(this.Types.Select(t => new TypeMap(t, types)), Enumerable.Empty<TypeFactoryMap>());
        }

        public ILifetimeSelector AsImplementedInterfaces()
        {
            return this.AsTypeInfo(t => t.ImplementedInterfaces
                .Where(x => x.HasMatchingGenericArity(t))
                .Select(x => x.GetRegistrationType(t)));
        }

        public ILifetimeSelector AsSelfWithInterfaces()
        {
            IEnumerable<Type> Selector(TypeInfo info)
            {
                if (info.IsGenericTypeDefinition)
                {
                    // This prevents trying to register open generic types
                    // with an ImplementationFactory, which is unsupported.
                    return Enumerable.Empty<Type>();
                }

                return info.ImplementedInterfaces
                    .Where(x => x.HasMatchingGenericArity(info))
                    .Select(x => x.GetRegistrationType(info));
            }

            return this.AddSelector(
                this.Types.Select(t => new TypeMap(t, new[] { t })),
                this.Types.Select(t => new TypeFactoryMap(x => x.GetRequiredService(t), Selector(t.GetTypeInfo()))));
        }

        public ILifetimeSelector AsMatchingInterface()
        {
            return this.AsMatchingInterface(null);
        }

        public ILifetimeSelector AsMatchingInterface(Action<TypeInfo, IImplementationTypeFilter>? action)
        {
            return this.AsTypeInfo(t => t.FindMatchingInterface(action));
        }

        public ILifetimeSelector As(Func<Type, IEnumerable<Type>> selector)
        {
            Preconditions.NotNull(selector, nameof(selector));

            return this.AddSelector(this.Types.Select(t => new TypeMap(t, selector(t))), Enumerable.Empty<TypeFactoryMap>());
        }

        public IImplementationTypeSelector UsingAttributes(Func<Type, Type, bool>? predicate=default)
        {
            var selector = new AttributeSelector(this.Types, predicate);

            this.Selectors.Add(selector);

            return this;
        }

        public IServiceTypeSelector UsingRegistrationStrategy(RegistrationStrategy registrationStrategy)
        {
            Preconditions.NotNull(registrationStrategy, nameof(registrationStrategy));

            this.RegistrationStrategy = registrationStrategy;
            return this;
        }

        #region Chain Methods

        public IImplementationTypeSelector FromCallingAssembly()
        {
            return this.Inner.FromCallingAssembly();
        }

        public IImplementationTypeSelector FromExecutingAssembly()
        {
            return this.Inner.FromExecutingAssembly();
        }

        public IImplementationTypeSelector FromEntryAssembly()
        {
            return this.Inner.FromEntryAssembly();
        }

        public IImplementationTypeSelector FromApplicationDependencies()
        {
            return this.Inner.FromApplicationDependencies();
        }

        public IImplementationTypeSelector FromApplicationDependencies(Func<AssemblyName, bool> predicate)
        {
            return this.Inner.FromApplicationDependencies(predicate);
        }

        public IImplementationTypeSelector FromAssemblyDependencies(Assembly assembly)
        {
            return this.Inner.FromAssemblyDependencies(assembly);
        }

        public IImplementationTypeSelector FromDependencyContext(DependencyContext context)
        {
            return this.Inner.FromDependencyContext(context);
        }

        public IImplementationTypeSelector FromDependencyContext(DependencyContext context, Func<AssemblyName, bool> predicate)
        {
            return this.Inner.FromDependencyContext(context, predicate);
        }

        public IImplementationTypeSelector FromAssemblyOf<T>()
        {
            return this.Inner.FromAssemblyOf<T>();
        }

        public IImplementationTypeSelector FromAssembliesOf(params Type[] types)
        {
            return this.Inner.FromAssembliesOf(types);
        }

        public IImplementationTypeSelector FromAssembliesOf(IEnumerable<Type> types)
        {
            return this.Inner.FromAssembliesOf(types);
        }

        public IImplementationTypeSelector FromAssemblies(params Assembly[] assemblies)
        {
            return this.Inner.FromAssemblies(assemblies);
        }

        public IImplementationTypeSelector FromAssemblies(IEnumerable<Assembly> assemblies)
        {
            return this.Inner.FromAssemblies(assemblies);
        }

        public IServiceTypeSelector AddClasses()
        {
            return this.Inner.AddClasses();
        }

        public IServiceTypeSelector AddClasses(bool publicOnly)
        {
            return this.Inner.AddClasses(publicOnly);
        }

        public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action)
        {
            return this.Inner.AddClasses(action);
        }

        public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action, bool publicOnly)
        {
            return this.Inner.AddClasses(action, publicOnly);
        }

        #endregion

        internal void PropagateLifetime(ServiceLifetime lifetime)
        {
            foreach (var selector in this.Selectors.OfType<LifetimeSelector>())
            {
                selector.Lifetime = lifetime;
            }
        }

        void ISelector.Populate(IServiceCollection services, RegistrationStrategy registrationStrategy)
        {
            if (this.Selectors.Count == 0)
            {
                this.AsSelf();
            }

            var strategy = this.RegistrationStrategy ?? registrationStrategy;

            foreach (var selector in this.Selectors)
            {
                selector.Populate(services, strategy);
            }
        }

        private ILifetimeSelector AddSelector(IEnumerable<TypeMap> types, IEnumerable<TypeFactoryMap> factories)
        {
            var selector = new LifetimeSelector(this, types, factories);

            this.Selectors.Add(selector);

            return selector;
        }

        private ILifetimeSelector AsTypeInfo(Func<TypeInfo, IEnumerable<Type>> selector)
        {
            return this.As(t => selector(t.GetTypeInfo()));
        }
    }
}
