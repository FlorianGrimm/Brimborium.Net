using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Typing {
    public sealed class MetaKind {
        private static MetaKind? _Empty;
        public static MetaKind Empty => (_Empty ??= new MetaKind(string.Empty));

        public MetaKind(string name) {
            this.Name = name;
        }

        public string Name { get; }

        public override bool Equals(object? obj) {
            if (obj is string s) {
                return string.Equals(this.Name, s, StringComparison.Ordinal);
            }
            if (obj is MetaKind other) {
                return string.Equals(this.Name, other.Name, StringComparison.Ordinal);
            }
            return false;
        }

        public override int GetHashCode() => this.Name.GetHashCode();

        public override string ToString() => this.Name;
    }

    public sealed class MetaKindComparer : IComparer<MetaKind>, IEqualityComparer<MetaKind> {
        private static MetaKindComparer? _Instance;
        public static MetaKindComparer Instance => (_Instance ??= new MetaKindComparer());

        public MetaKindComparer() {
        }

        public int Compare(MetaKind? x, MetaKind? y) {
            if (ReferenceEquals(x, y)) { return 0; }
            if (x is null) { return -1; }
            if (y is null) { return +1; }
            return StringComparer.Ordinal.Compare(x.Name, y.Name);
        }

        public bool Equals(MetaKind? x, MetaKind? y) {
            if (ReferenceEquals(x, y)) { return true; }
            if (x is null) { return false; }
            if (y is null) { return false; }
            return StringComparer.Ordinal.Equals(x.Name, y.Name);
        }

        public int GetHashCode([DisallowNull] MetaKind obj) {
            return obj.Name.GetHashCode();
        }
    }

    public sealed class MetaKindRepository {
        private static MetaKindRepository? _Instance;
        public static MetaKindRepository Instance => (_Instance ??= CreateInstance());


        public static MetaKindRepository CreateInstance() {
            var result = new MetaKindRepository();
            //
            result.Add(MetaKind.Empty);
            //
            result.CreateIfNeeded("Class");
            result.CreateIfNeeded("Struct");
            result.CreateIfNeeded("Entity");
            result.CreateIfNeeded("Property");
            result.CreateIfNeeded("Method");
            //
            result.CreateIfNeeded("Table");
            result.CreateIfNeeded("View");
            //
            return result;
        }

        public MetaKindRepository() {
            this.Items = new Dictionary<string, MetaKind>(StringComparer.Ordinal);
        }

        public Dictionary<string, MetaKind> Items { get; init; }

        public void Add(MetaKind metaKind) {
            this.Items.Add(metaKind.Name, metaKind);
        }

        public MetaKind Get(string name) {
            if (this.Items.TryGetValue(name, out var value)) {
                return value;
            } else {
                throw new ArgumentException(name);
            }
        }

        public bool TryGet(string name, [MaybeNullWhen(false)] out MetaKind value) {
            return this.Items.TryGetValue(name, out value);
        }

        public MetaKind CreateIfNeeded(string name) {
            if (this.Items.TryGetValue(name, out var result)) {
                return result;
            } else {
                result = new MetaKind(name);
                this.Items.Add(name, result);
                return result;
            }
        }
    }
}
