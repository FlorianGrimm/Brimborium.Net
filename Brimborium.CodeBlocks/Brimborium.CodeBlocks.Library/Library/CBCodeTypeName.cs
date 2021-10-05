using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Brimborium.CodeBlocks.Library {
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class CBCodeTypeName : ICBCodeTypeName {
        public CBCodeTypeName() {
            this.Namespace = new CBCodeNamespace();
            this.TypeName = string.Empty;
        }

        public CBCodeTypeName(CBCodeNamespace @namespace, string typeName) {
            this.Namespace = @namespace;
            this.TypeName = typeName;
        }

        public CBCodeNamespace Namespace { get; set; }

        public string TypeName { get; set; }

        private string? _FullName;
        public string FullName {
            get { return this._FullName ?? GetFullName(); }
            set { this.FullName = value; }
        }

        private string GetFullName() {
            return ((string.IsNullOrEmpty(this.Namespace.Namespace)) 
                ? (this.TypeName ?? string.Empty) 
                : $"{this.Namespace.Namespace}.{this.TypeName}");
        }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];

        public override string ToString() {
            return $"{this.Namespace}.{this.TypeName}";
        }

        private string GetDebuggerDisplay() {
            return this.ToString();
        }

        public static CBCodeTypeName FromType(Type type) {
            return new CBCodeTypeName(new CBCodeNamespace(type.Namespace ?? string.Empty), type.Name);
        }
    }
}