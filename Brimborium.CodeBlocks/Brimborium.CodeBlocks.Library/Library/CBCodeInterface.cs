using System.Linq;

namespace Brimborium.CodeBlocks.Library {
    public sealed class CBCodeInterface : CBCodeTypeDefinition, ICBCodeTypeDefinition {
        public CBCodeInterface() : base() {
        }

        public CBCodeInterface(CBCodeNamespace @namespace, string typeName) : base(@namespace, typeName) {
        }

        public bool IsPartial { get; set; }
    }

    public sealed class CBTemplateCSharpInterfaceDeclaration : CBNamedTemplate<CBCodeInterface> {
        public CBTemplateCSharpInterfaceDeclaration()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Declaration) {
        }

        public override void RenderT(CBCodeInterface value, CBRenderContext ctxt) {
            ctxt.Foreach(
                items: value.Attributes,
                eachItem: (i, ctxt) => ctxt.CallTemplateDynamic(i.Value, CBTemplateProvider.Attribute));
            ctxt.CallTemplateStrict(value.AccessibilityLevel)
                .If(value.IsPartial, (ctxt) => ctxt.Write("partial "))
                .Write("interface ")
                .Write(value.TypeName)
                .CallTemplateStrict<CBCodeTypeDefinition>(value, "BaseTypes")
                .Write(" {").WriteLine();
            foreach (var grp in value.Members
                .GroupBy(m => m switch { CBCodeDefinitionField => 0, CBCodeDefinitionConstructor => 1, CBCodeDefinitionProperty => 2, CBCodeDefinitionMethod => 3, _ => 4 })
                .OrderBy(kv => kv.Key)
                ) {
                foreach (var member in grp) {
                    ctxt.CallTemplateDynamic(member, CBTemplateProvider.Declaration);
                }
                ctxt.WriteLine();
            }

            ctxt.IndentDecr().Write("}").WriteLine();
        }

    }
}