using System;
using System.Collections.Generic;
using System.Linq;

namespace Brimborium.Registrator.Internals {
    internal class ImplementationTypeFilter : IImplementationTypeFilter {
        public ImplementationTypeFilter(IEnumerable<Type> types) {
            this.Types = types;
        }

        internal IEnumerable<Type> Types { get; private set; }

        public IImplementationTypeFilter AssignableTo<T>() {
            return this.AssignableTo(typeof(T));
        }

        public IImplementationTypeFilter AssignableTo(Type type) {
            Preconditions.NotNull(type, nameof(type));

            return this.AssignableToAny(type);
        }

        public IImplementationTypeFilter AssignableToAny(params Type[] types) {
            Preconditions.NotNull(types, nameof(types));

            return this.AssignableToAny(types.AsEnumerable());
        }

        public IImplementationTypeFilter AssignableToAny(IEnumerable<Type> types) {
            Preconditions.NotNull(types, nameof(types));

            return this.Where(t => types.Any(t.IsAssignableTo));
        }

        public IImplementationTypeFilter WithAttribute<T>() where T : Attribute {
            return this.WithAttribute(typeof(T));
        }

        public IImplementationTypeFilter WithAttribute(Type attributeType) {
            Preconditions.NotNull(attributeType, nameof(attributeType));

            return this.Where(t => t.HasAttribute(attributeType));
        }

        public IImplementationTypeFilter WithAttribute<T>(Func<T, bool> predicate) where T : Attribute {
            Preconditions.NotNull(predicate, nameof(predicate));

            return this.Where(t => t.HasAttribute(predicate));
        }

        public IImplementationTypeFilter WithoutAttribute<T>() where T : Attribute {
            return this.WithoutAttribute(typeof(T));
        }

        public IImplementationTypeFilter WithoutAttribute(Type attributeType) {
            Preconditions.NotNull(attributeType, nameof(attributeType));

            return this.Where(t => !t.HasAttribute(attributeType));
        }

        public IImplementationTypeFilter WithoutAttribute<T>(Func<T, bool> predicate) where T : Attribute {
            Preconditions.NotNull(predicate, nameof(predicate));

            return this.Where(t => !t.HasAttribute(predicate));
        }

        public IImplementationTypeFilter InNamespaceOf<T>() {
            return this.InNamespaceOf(typeof(T));
        }

        public IImplementationTypeFilter InNamespaceOf(params Type[] types) {
            Preconditions.NotNull(types, nameof(types));

            return this.InNamespaces(types.Select(t => t.Namespace ?? string.Empty));
        }

        public IImplementationTypeFilter InNamespaces(params string[] namespaces) {
            Preconditions.NotNull(namespaces, nameof(namespaces));

            return this.InNamespaces(namespaces.AsEnumerable());
        }

        public IImplementationTypeFilter InExactNamespaceOf<T>() {
            return this.InExactNamespaceOf(typeof(T));
        }

        public IImplementationTypeFilter InExactNamespaceOf(params Type[] types) {
            Preconditions.NotNull(types, nameof(types));
            return this.Where(t => types.Any(x => t.IsInExactNamespace(x.Namespace ?? string.Empty)));
        }

        public IImplementationTypeFilter InExactNamespaces(params string[] namespaces) {
            Preconditions.NotNull(namespaces, nameof(namespaces));

            return this.Where(t => namespaces.Any(t.IsInExactNamespace));
        }

        public IImplementationTypeFilter InNamespaces(IEnumerable<string> namespaces) {
            Preconditions.NotNull(namespaces, nameof(namespaces));

            return this.Where(t => namespaces.Any(t.IsInNamespace));
        }

        public IImplementationTypeFilter NotInNamespaceOf<T>() {
            return this.NotInNamespaceOf(typeof(T));
        }

        public IImplementationTypeFilter NotInNamespaceOf(params Type[] types) {
            Preconditions.NotNull(types, nameof(types));

            return this.NotInNamespaces(types.Select(t => t.Namespace ?? string.Empty));
        }

        public IImplementationTypeFilter NotInNamespaces(params string[] namespaces) {
            Preconditions.NotNull(namespaces, nameof(namespaces));

            return this.NotInNamespaces(namespaces.AsEnumerable());
        }

        public IImplementationTypeFilter NotInNamespaces(IEnumerable<string> namespaces) {
            Preconditions.NotNull(namespaces, nameof(namespaces));

            return this.Where(t => namespaces.All(ns => !t.IsInNamespace(ns)));
        }

        public IImplementationTypeFilter Where(Func<Type, bool> predicate) {
            Preconditions.NotNull(predicate, nameof(predicate));

            this.Types = this.Types.Where(predicate).ToList();

            return this;
        }
    }
}
