using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Typing {
    public sealed class MetaName : IMetaName {
        private static MetaName? _Empty;
        public static MetaName Empty => (_Empty = new MetaName());

        private string? _FullName;
        private string _Namespace;
        private string _Name;

        public MetaName() {
            this.ContainerName = string.Empty;
            this._Namespace = string.Empty;
            this._Name = string.Empty;
        }

        public MetaName(
            string @namespace,
            string name) {
            this.ContainerName = string.Empty;
            this._Namespace = @namespace;
            this._Name = name;
        }

        public MetaName(
            string containerName,
            string @namespace,
            string name) {
            this.ContainerName = containerName;
            this._Namespace = @namespace;
            this._Name = name;
        }

        public string ContainerName { get; set; }
        public string Namespace { get { return this._Namespace; } set { this._Namespace = value; this._FullName = null; } }
        public string Name { get { return this._Name; } set { this._Name = value; this._FullName = null; } }
        public string FullName => (this._FullName ??= this.GetFullName());

        private string GetFullName() {
            if (string.IsNullOrEmpty(this.Namespace)) {
                return this.Name;
            } else {
                return $"{this.Namespace}.{this.Name}";
            }
        }

        public override string ToString() {
            if (string.IsNullOrEmpty(this.ContainerName)) {
                if (string.IsNullOrEmpty(this.Namespace)) {
                    return this.Name;
                } else {
                    return $"{this.Namespace}.{this.Name}";
                }
            } else {
                return $"{this.ContainerName}.{this.Namespace}.{this.Name}";
            }
        }
    }
    public sealed class MetaNameComparer : IComparer<MetaName>, IEqualityComparer<MetaName> {
        private readonly StringComparer _StringComparer;

        public MetaNameComparer(StringComparer stringComparer) {
            this._StringComparer = stringComparer;
        }

        public int Compare(MetaName? x, MetaName? y) {
            if (ReferenceEquals(x, y)) { return 0; }
            if (x is null) { return -1; }
            if (y is null) { return +1; }
            //
            int result = 0;
            if (!string.IsNullOrEmpty(x.ContainerName) && !string.IsNullOrEmpty(y.ContainerName)) {
                result = this._StringComparer.Compare(x.ContainerName, y.ContainerName);
                if (result != 0) { return result; }
            }
            //
            result = this._StringComparer.Compare(x.Namespace, y.Namespace);
            if (result != 0) { return result; }
            //
            result = this._StringComparer.Compare(x.Name, y.Name);
            return result;
        }

        public bool Equals(MetaName? x, MetaName? y) {
            if (ReferenceEquals(x, y)) { return true; }
            if (x is null) { return false; }
            if (y is null) { return false; }
            //
            bool result = true;
            if (!string.IsNullOrEmpty(x.ContainerName) && !string.IsNullOrEmpty(y.ContainerName)) {
                result = this._StringComparer.Equals(x.ContainerName, y.ContainerName);
                if (!result) { return false; }
            }
            //
            result = this._StringComparer.Equals(x.Namespace, y.Namespace);
            if (!result) { return false; }
            //
            result = this._StringComparer.Equals(x.Name, y.Name);
            return result;
        }

        public int GetHashCode([DisallowNull] MetaName obj) {
            return obj.Namespace.GetHashCode() ^ obj.Name.GetHashCode();
        }
    }
}