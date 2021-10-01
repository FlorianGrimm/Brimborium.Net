using System.Collections.Generic;
using System.Linq;

namespace Brimborium.Typing {
    public sealed class MetaMember {
        public MetaMember() {
            this.Name = string.Empty;
            this.MetaKinds = new HashSet<MetaKind>(MetaKindComparer.Instance);
        }

        public MetaMember(MetaType? owner, string name, MetaType? propertyType) {
            this.Owner = owner;
            this.Name = name;
            this.PropertyType = propertyType;
            this.MetaKinds = new HashSet<MetaKind>(MetaKindComparer.Instance);
        }

        public string Name { get; set; }

        public MetaType? Owner { get; set; }

        public MetaType? PropertyType { get; set; }

        // public MetaName OwnerFullName { get; set; }

        public string Kinds {
            get {
                return string.Join(";", this.MetaKinds.Select(mk => mk.Name).OrderBy(n => n));
            }
            set {
                this.MetaKinds.Clear();
                var repository = MetaKindRepository.Instance;
                foreach (var kind in value.Trim().Split(';')) {
                    this.MetaKinds.Add(repository.CreateIfNeeded(kind));
                }
            }
        }

        public HashSet<MetaKind> MetaKinds { get; }
    }

    public sealed class MetaValue {
        public MetaValue(MetaKind metaKind, string name) {
            this.MetaKind = metaKind;
            this.Name = name;
            this.MetaKinds = new HashSet<MetaKind>();
        }

        public MetaKind MetaKind { get; set; }

        public string Name { get; set; }

        public MetaName? ValueName { get; set; }
        public long? ValueLong { get; set; }

        public string Kinds {
            get {
                return string.Join(";", this.MetaKinds.Select(mk => mk.Name).OrderBy(n => n));
            }
            set {
                this.MetaKinds.Clear();
                var repository = MetaKindRepository.Instance;
                foreach (var kind in value.Trim().Split(';')) {
                    this.MetaKinds.Add(repository.CreateIfNeeded(kind));
                }
            }
        }

        public HashSet<MetaKind> MetaKinds { get; }
    }
}
