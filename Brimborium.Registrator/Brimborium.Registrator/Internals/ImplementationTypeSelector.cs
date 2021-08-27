using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Brimborium.Registrator.Internals {
    internal class ImplementationTypeSelector : IImplementationTypeSelector, ISelector {
        public ImplementationTypeSelector(ITypeSourceSelector inner, IEnumerable<Type> types) {
            this.Inner = inner;
            this.Types = types;
        }

        private ITypeSourceSelector Inner { get; }

        private IEnumerable<Type> Types { get; }

        private List<ISelector> Selectors { get; } = new List<ISelector>();

        public IServiceTypeSelector AddClasses() {
            return this.AddClasses(publicOnly: true);
        }

        public IServiceTypeSelector AddClasses(bool publicOnly) {
            var classes = this.GetNonAbstractClasses(publicOnly);

            return this.AddSelector(classes);
        }

        public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action) {
            return this.AddClasses(action, publicOnly: false);
        }

        public IServiceTypeSelector AddClasses(Action<IImplementationTypeFilter> action, bool publicOnly) {
            Preconditions.NotNull(action, nameof(action));

            var classes = this.GetNonAbstractClasses(publicOnly);

            var filter = new ImplementationTypeFilter(classes);

            action(filter);

            return this.AddSelector(filter.Types);
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

        #endregion

        void ISelector.Populate(IServiceCollection services, RegistrationStrategy registrationStrategy) {
            if (this.Selectors.Count == 0) {
                this.AddClasses();
            }

            foreach (var selector in this.Selectors) {
                selector.Populate(services, registrationStrategy);
            }
        }

        private IServiceTypeSelector AddSelector(IEnumerable<Type> types) {
            var selector = new ServiceTypeSelector(this, types);

            this.Selectors.Add(selector);

            return selector;
        }

        private IEnumerable<Type> GetNonAbstractClasses(bool publicOnly) {
            return this.Types.Where(t => t.IsNonAbstractClass(publicOnly));
        }
    }
}
