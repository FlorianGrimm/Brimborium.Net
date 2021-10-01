using System;
using System.Collections.Generic;
using System.Linq;

namespace Brimborium.Typing {
    public sealed class MetaType : IMetaName {
        private string? _FullName;
        private string _Namespace;
        private string _Name;

        public MetaType() {
            this.ContainerName = string.Empty;
            this._Namespace = string.Empty;
            this._Name = string.Empty;
            this.Members = new List<MetaMember>();
            this.MetaKind = new HashSet<MetaKind>(MetaKindComparer.Instance);
        }

        public string ContainerName { get; set; }
        public string Namespace { get { return this._Namespace; } set { this._Namespace = value; this._FullName = null; } }
        public string Name { get { return this._Name; } set { this._Name = value; this._FullName = null; } }
        public string FullName => (this._FullName ??= this.GetFullName());

        public List<MetaMember> Members { get; set; }

        private string GetFullName() {
            if (string.IsNullOrEmpty(this.Namespace)) {
                return this.Name;
            } else {
                return $"{this.Namespace}.{this.Name}";
            }
        }

        public string Kinds {
            get {
                return string.Join(";", this.MetaKind.Select(mk => mk.Name).OrderBy(n => n));
            }
            set {
                this.MetaKind.Clear();
                var repository = MetaKindRepository.Instance;
                foreach (var kind in value.Trim().Split(';')) {
                    this.MetaKind.Add(repository.CreateIfNeeded(kind));
                }
            }
        }

        public HashSet<MetaKind> MetaKind { get; }
    }
}
