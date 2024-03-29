﻿using System;
using System.Diagnostics;

namespace Brimborium.CodeBlocks.Library {
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class CBCodeNamespace : IEquatable<CBCodeNamespace>{
        public CBCodeNamespace() {
            this.Name = string.Empty;
        }

        public CBCodeNamespace(string @namespace) {
            this.Name = @namespace;
        }

        public CBCodeNamespace(CBCodeNamespace source) {
            this.Name = source.Name;
        }

        public string Name { get; set; }

        public override string ToString() {
            return this.Name;
        }

        private string GetDebuggerDisplay() {
            return this.Name;
        }

        public CBCodeNamespace GetSubNamespace(string subName) {
            if (string.IsNullOrEmpty(this.Name)) {
                return new CBCodeNamespace(subName);
            } else {
                return new CBCodeNamespace($"{this.Name}.{subName}");
            }
        }

        public CBCodeNamespace GetParentNamespace() {
            var index = this.Name.LastIndexOf('.');
            if (index >= 0) {
                return new CBCodeNamespace(this.Name.Substring(0, index));
            }
            return new CBCodeNamespace();
        }

        public override bool Equals(object? obj) {
            return this.Equals(obj as CBCodeNamespace);
        }

        public bool Equals(CBCodeNamespace? other) {
            if (ReferenceEquals(this, other)) { return true; }
            if (ReferenceEquals(other, null)) { return false; }
            return string.Equals(this.Name, other.Name, StringComparison.Ordinal);
        }
        public override int GetHashCode() {
            return this.Name.GetHashCode();
        }
    }
}