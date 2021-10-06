using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public class CBCodeTypeDefinition : CBCodeTypeName, ICBCodeTypeDefinition {
        protected CBCodeTypeDefinition() : base() {
            this.Attributes = new CBList<ICBCodeTypeDecoration>(this);
            this.Interfaces = new CBList<ICBCodeTypeReference>(this);
            this.Members = new CBList<ICBCodeDefinitionMember>(this);
        }

        protected CBCodeTypeDefinition(CBCodeNamespace @namespace, string typeName) : base(@namespace, typeName) {
            this.Attributes = new CBList<ICBCodeTypeDecoration>(this);
            this.Interfaces = new CBList<ICBCodeTypeReference>(this);
            this.Members = new CBList<ICBCodeDefinitionMember>(this);
        }

        public CBCodeAccessibilityLevel AccessibilityLevel { get; set; } = CBCodeAccessibilityLevel.Public;

        public CBList<ICBCodeTypeDecoration> Attributes { get; init; }

        public ICBCodeTypeReference? BaseType { get; set; } = null;

        public CBList<ICBCodeTypeReference> Interfaces { get; init; }

        public CBList<ICBCodeDefinitionMember> Members { get; init; }
    }

    public sealed class CBTemplateCSharpTypeDefinitionBaseTypes : CBNamedTemplate<CBCodeTypeDefinition> {
        public CBTemplateCSharpTypeDefinitionBaseTypes()
            : base(CBTemplateProvider.CSharp, "BaseTypes") {
        }

        public override void RenderT(CBCodeTypeDefinition value, CBRenderContext ctxt) {
            var lstBaseTypes = new List<ICBCodeTypeReference>();
            if (value.BaseType is not null) {
                lstBaseTypes.Add(value.BaseType);
            }
            lstBaseTypes.AddRange(value.Interfaces);

            ctxt.Foreach(
                items: lstBaseTypes,
                eachItem: (i, ctxt) => {
                    if (i.IsFirst) {
                        ctxt.WriteLine(indent: +1);
                        ctxt.Write(" : ");
                    } else {
                        ctxt.WriteLine();
                        ctxt.Write(", ");
                    }
                    ctxt.CallTemplateDynamic(i.Value, CBTemplateProvider.TypeName);
                },
                isEmpty: (ctxt) => {
                    ctxt.IndentIncr();
                });
        }
    }
}