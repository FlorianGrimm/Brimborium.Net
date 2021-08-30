using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Brimborium.Registrator.Internals {
    internal class ImplementationTypeSelector : IImplementationTypeSelector, ISelector {
        private readonly ITypeSourceSelector _Inner;
        private readonly IEnumerable<Type> _Types;
        private readonly List<ISelector> _Selectors;

        public ImplementationTypeSelector(ITypeSourceSelector inner, IEnumerable<Type> types) {
            this._Inner = inner;
            this._Types = types;
            this._Selectors = new List<ISelector>();
        }

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

        #endregion

        void ISelector.Populate(RegistrationStrategy registrationStrategy, ISelectorTarget selectorTarget) {
            if (this._Selectors.Count == 0) {
                this.AddClasses();
            }

            foreach (var selector in this._Selectors) {
                selector.Populate(registrationStrategy, selectorTarget);
            }
        }

        private IServiceTypeSelector AddSelector(IEnumerable<Type> types) {
            var selector = new ServiceTypeSelector(this, types);

            this._Selectors.Add(selector);

            return selector;
        }

        private IEnumerable<Type> GetNonAbstractClasses(bool publicOnly) {
            return this._Types.Where(t => t.IsNonAbstractClass(publicOnly));
        }
    }
}
