using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Brimborium.Registrator.Internals;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Brimborium.Registrator {
    internal sealed class TypeSourceSelector : ITypeSourceSelector, ISelector {
        private static bool DefaultPredicate(AssemblyName assemblyName) => true;

        private readonly List<ISelector> _Selectors;
        private readonly Dictionary<AssemblyName, Assembly> _AssemblyLoaded;

        public TypeSourceSelector() {
            this._Selectors = new List<ISelector>();
            this._AssemblyLoaded = new Dictionary<AssemblyName, Assembly>();
        }

        /// <inheritdoc />
        public IImplementationTypeSelector FromAssemblyOf<T>() {
            return this.InternalFromAssembliesOf(new[] { typeof(T).GetTypeInfo() });
        }

        public IImplementationTypeSelector FromApplicationDependencies(DependencyContext context, Func<AssemblyName, bool>? predicate = default) {
            try {
                return this.FromDependencyContext(context, predicate);
            } catch {
                // Something went wrong when loading the DependencyContext, fall
                // back to loading all referenced assemblies of the entry assembly...
                var entryAssembly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Could not get entry assembly.");
                return this.FromAssemblyDependencies(context, entryAssembly, predicate);
            }
        }

        public IImplementationTypeSelector FromDependencyContext(DependencyContext context, Func<AssemblyName, bool>? predicate = default) {
            Preconditions.NotNull(context, nameof(context));
            predicate ??= ReflectionExtensions.DefaultPredicateAssemblyName;

            var assemblyNames = context.RuntimeLibraries
                .SelectMany(library => library.GetDefaultAssemblyNames(context))
                .Where(predicate)
                ;

            var assemblies = LoadAssemblies(assemblyNames);

            return this.InternalFromAssemblies(assemblies);
        }

        public IImplementationTypeSelector FromAssemblyDependencies(DependencyContext context, Assembly assembly, Func<AssemblyName, bool>? predicate = default) {
            Preconditions.NotNull(assembly, nameof(assembly));
            predicate ??= ReflectionExtensions.DefaultPredicateAssemblyName;

            var assemblies = new List<Assembly> { assembly };

            var dependencyNames = assembly.GetReferencedAssemblies()
                .Where(predicate)
                ;

            assemblies.AddRange(LoadAssemblies(dependencyNames));

            return this.InternalFromAssemblies(assemblies);
        }

        public IImplementationTypeSelector FromAssembliesOf(params Type[] types) {
            Preconditions.NotNull(types, nameof(types));

            return this.InternalFromAssembliesOf(types.Select(x => x.GetTypeInfo()));
        }

        public IImplementationTypeSelector FromAssembliesOf(IEnumerable<Type> types) {
            Preconditions.NotNull(types, nameof(types));

            return this.InternalFromAssembliesOf(types.Select(t => t.GetTypeInfo()));
        }

        public IImplementationTypeSelector FromAssemblies(params Assembly[] assemblies) {
            Preconditions.NotNull(assemblies, nameof(assemblies));

            return this.InternalFromAssemblies(assemblies);
        }

        public IImplementationTypeSelector FromAssemblies(IEnumerable<Assembly> assemblies) {
            Preconditions.NotNull(assemblies, nameof(assemblies));

            return this.InternalFromAssemblies(assemblies);
        }

        public IServiceTypeSelector AddTypes(params Type[] types) {
            Preconditions.NotNull(types, nameof(types));

            var selector = new ImplementationTypeSelector(this, types);

            this._Selectors.Add(selector);

            return selector.AddClasses();
        }

        public IServiceTypeSelector AddTypes(IEnumerable<Type> types) {
            Preconditions.NotNull(types, nameof(types));

            var selector = new ImplementationTypeSelector(this, types);

            this._Selectors.Add(selector);

            return selector.AddClasses();
        }

        void ISelector.Populate(RegistrationStrategy registrationStrategy, ISelectorTarget selectorTarget) {
            foreach (var selector in this._Selectors) {
                selector.Populate(registrationStrategy, selectorTarget);
            }
        }

        private IImplementationTypeSelector InternalFromAssembliesOf(IEnumerable<TypeInfo> typeInfos) {
            return this.InternalFromAssemblies(typeInfos.Select(t => t.Assembly));
        }

        private IImplementationTypeSelector InternalFromAssemblies(IEnumerable<Assembly> assemblies) {
            return this.AddSelector(assemblies.SelectMany(asm => asm.DefinedTypes).Select(x => x.AsType()));
        }

        private IEnumerable<Assembly> LoadAssemblies(IEnumerable<AssemblyName> assemblyNames) {
            var assemblies = new List<Assembly>();

            foreach (var assemblyName in assemblyNames) {
                try {
                    if (this._AssemblyLoaded.TryGetValue(assemblyName, out var assembly)) {
                        assemblies.Add(assembly);
                    } else {
                        // Try to load the referenced assembly...
                        assembly = Assembly.Load(assemblyName);
                        assemblies.Add(assembly);
                        this._AssemblyLoaded[assemblyName] = assembly;
                    }
                } catch {
                    // Failed to load assembly. Skip it.
                }
            }

            return assemblies;
        }

        private IImplementationTypeSelector AddSelector(IEnumerable<Type> types) {
            var selector = new ImplementationTypeSelector(this, types);

            this._Selectors.Add(selector);

            return selector;
        }
    }
}
