using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Brimborium.Registrator.Internals;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Brimborium.Registrator {
    public class TypeSourceSelector : ITypeSourceSelector, ISelector {
        private static Assembly EntryAssembly => Assembly.GetEntryAssembly()
            ?? throw new InvalidOperationException("Could not get entry assembly.");

        private readonly List<ISelector> Selectors = new List<ISelector>();

        /// <inheritdoc />
        public IImplementationTypeSelector FromAssemblyOf<T>() {
            return this.InternalFromAssembliesOf(new[] { typeof(T).GetTypeInfo() });
        }

        public IImplementationTypeSelector FromCallingAssembly() {
            return this.FromAssemblies(Assembly.GetCallingAssembly());
        }

        public IImplementationTypeSelector FromExecutingAssembly() {
            return this.FromAssemblies(Assembly.GetExecutingAssembly());
        }

        public IImplementationTypeSelector FromEntryAssembly() {
            return this.FromAssemblies(EntryAssembly);
        }

        public IImplementationTypeSelector FromApplicationDependencies() {
            return this.FromApplicationDependencies(_ => true);
        }

        public IImplementationTypeSelector FromApplicationDependencies(Func<AssemblyName, bool> predicate) {
            try {
                return this.FromDependencyContext(DependencyContext.Default, predicate);
            } catch {
                // Something went wrong when loading the DependencyContext, fall
                // back to loading all referenced assemblies of the entry assembly...
                return this.FromAssemblyDependencies(EntryAssembly);
            }
        }

        public IImplementationTypeSelector FromDependencyContext(DependencyContext context) {
            return this.FromDependencyContext(context, _ => true);
        }

        public IImplementationTypeSelector FromDependencyContext(DependencyContext context, Func<AssemblyName, bool> predicate) {
            Preconditions.NotNull(context, nameof(context));
            Preconditions.NotNull(predicate, nameof(predicate));

            var assemblyNames = context.RuntimeLibraries
                .SelectMany(library => library.GetDefaultAssemblyNames(context))
                .Where(predicate)
                ;

            var assemblies = LoadAssemblies(assemblyNames);

            return this.InternalFromAssemblies(assemblies);
        }

        public IImplementationTypeSelector FromAssemblyDependencies(Assembly assembly) {
            Preconditions.NotNull(assembly, nameof(assembly));

            var assemblies = new List<Assembly> { assembly };

            try {
                var dependencyNames = assembly.GetReferencedAssemblies();

                assemblies.AddRange(LoadAssemblies(dependencyNames));

                return this.InternalFromAssemblies(assemblies);
            } catch {
                return this.InternalFromAssemblies(assemblies);
            }
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

            this.Selectors.Add(selector);

            return selector.AddClasses();
        }

        public IServiceTypeSelector AddTypes(IEnumerable<Type> types) {
            Preconditions.NotNull(types, nameof(types));

            var selector = new ImplementationTypeSelector(this, types);

            this.Selectors.Add(selector);

            return selector.AddClasses();
        }

        public void Populate(IServiceCollection services, RegistrationStrategy registrationStrategy) {
            foreach (var selector in this.Selectors) {
                selector.Populate(services, registrationStrategy);
            }
        }

        private IImplementationTypeSelector InternalFromAssembliesOf(IEnumerable<TypeInfo> typeInfos) {
            return this.InternalFromAssemblies(typeInfos.Select(t => t.Assembly));
        }

        private IImplementationTypeSelector InternalFromAssemblies(IEnumerable<Assembly> assemblies) {
            return this.AddSelector(assemblies.SelectMany(asm => asm.DefinedTypes).Select(x => x.AsType()));
        }

        private static IEnumerable<Assembly> LoadAssemblies(IEnumerable<AssemblyName> assemblyNames) {
            var assemblies = new List<Assembly>();
#warning optimize needed?
            foreach (var assemblyName in assemblyNames) {
                try {
                    // Try to load the referenced assembly...
                    assemblies.Add(Assembly.Load(assemblyName));
                } catch {
                    // Failed to load assembly. Skip it.
                }
            }

            return assemblies;
        }

        private IImplementationTypeSelector AddSelector(IEnumerable<Type> types) {
            var selector = new ImplementationTypeSelector(this, types);

            this.Selectors.Add(selector);

            return selector;
        }
    }
}
