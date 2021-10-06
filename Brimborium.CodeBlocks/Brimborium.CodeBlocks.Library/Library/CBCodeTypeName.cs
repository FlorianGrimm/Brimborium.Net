using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Brimborium.CodeBlocks.Library {
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class CBCodeTypeName : ICBCodeTypeName {
        public CBCodeTypeName() {
            this.Namespace = new CBCodeNamespace();
            this.TypeName = string.Empty;
            this.GenericTypeArguments = new ICBCodeTypeReference[0];
        }

        public CBCodeTypeName(CBCodeNamespace @namespace, string typeName) {
            this.Namespace = @namespace;
            this.TypeName = typeName;
            this.GenericTypeArguments = new ICBCodeTypeReference[0];
        }

        public CBCodeNamespace Namespace { get; set; }

        public string TypeName { get; set; }

        private string? _FullName;

        public string FullName {
            get { return (this._FullName ??= GetFullName()); }
            set { this.FullName = value; }
        }

        private string GetFullName() {
            return ((string.IsNullOrEmpty(this.Namespace.Namespace))
                ? (this.TypeName ?? string.Empty)
                : $"{this.Namespace.Namespace}.{this.TypeName}");
        }

        public ICBCodeTypeReference? GenericTypeDefinition { get; set; }
        public ICBCodeTypeReference[] GenericTypeArguments { get; set; }

        public ICBCodeElement? Parent { get; set; }
        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];

        public override string ToString() {
            return $"{this.Namespace.Namespace}.{this.TypeName}";
        }

        private string GetDebuggerDisplay() {
            return $"{this.Namespace.Namespace}.{this.TypeName}";
        }

        public static CBCodeTypeName FromType(Type type) {
            return new CBCodeTypeName(new CBCodeNamespace(type.Namespace ?? string.Empty), type.Name);
        }
    }

    public sealed class CBTemplateCSharpTypeName : CBNamedTemplate<CBCodeTypeName> {
        public CBTemplateCSharpTypeName()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.TypeName) {
        }

        public override void RenderT(CBCodeTypeName value, CBRenderContext ctxt) {
            var genericTypeDefinition = value.GenericTypeDefinition;
            var genericTypeArguments = value.GenericTypeArguments;
            if (genericTypeDefinition is not null && genericTypeArguments.Length > 0) {
                ctxt.CallTemplateDynamic(genericTypeDefinition.CodeTypeName, CBTemplateProvider.TypeName);
                ctxt.Write("<");
                ctxt.Foreach(
                    items: genericTypeArguments,
                    eachItem: (i, ctxt) => {
                        ctxt.CallTemplateDynamic(i.Value.CodeTypeName, CBTemplateProvider.TypeName)
                        .If(i.IsFirst,
                            Then: (ctxt) => {
                            },
                            Else: (ctxt) => {
                                ctxt.Write(", ");
                            });
                    }
                    );
                ctxt.Write(">");
            } else {
                ctxt.Write(value.Namespace.Namespace).Write(".").Write(value.TypeName);
            }
        }
    }
}