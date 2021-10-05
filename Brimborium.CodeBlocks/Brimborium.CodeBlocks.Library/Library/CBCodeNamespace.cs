using System.Diagnostics;

namespace Brimborium.CodeBlocks.Library {
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class CBCodeNamespace {
        public CBCodeNamespace() {
            this.Namespace = string.Empty;
        }

        public CBCodeNamespace(string @namespace) {
            this.Namespace = @namespace;
        }

        public CBCodeNamespace(CBCodeNamespace source) {
            this.Namespace = source.Namespace;
        }

        public string Namespace { get; set; }

        public override string ToString() {
            return this.Namespace;
        }

        private string GetDebuggerDisplay() {
            return this.Namespace;
        }

        public CBCodeNamespace GetSubNamespace(string subName) {
            if (string.IsNullOrEmpty(this.Namespace)) {
                return new CBCodeNamespace(subName);
            } else {
                return new CBCodeNamespace($"{this.Namespace}.{subName}");
            }
        }

        public CBCodeNamespace GetParentNamespace() {
            var index = this.Namespace.LastIndexOf('.');
            if (index >= 0) {
                return new CBCodeNamespace(this.Namespace.Substring(0, index));
            }
            return new CBCodeNamespace();
        }
    }
}