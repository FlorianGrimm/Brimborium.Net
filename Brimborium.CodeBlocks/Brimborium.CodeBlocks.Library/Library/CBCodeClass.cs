using System.Linq;

namespace Brimborium.CodeBlocks.Library {
    public sealed class CBCodeClass : CBCodeTypeDefinition, ICBCodeTypeDefinition {
        public CBCodeClass() : base() {
            this.Prefix = new CBList<ICBCodeElement>(this);
        }

        public CBCodeClass(CBCodeNamespace @namespace, string typeName) : base(@namespace, typeName) {
            this.Prefix = new CBList<ICBCodeElement>(this);
        }

        public CBList<ICBCodeElement> Prefix { get; }

        public bool IsSealed { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsPartial { get; set; }
    }

    public sealed class CBTemplateCSharpClassDeclaration : CBNamedTemplate<CBCodeClass> {
        public CBTemplateCSharpClassDeclaration()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Declaration) {
        }

        public override void RenderT(CBCodeClass value, CBRenderContext ctxt) {
            ctxt.Foreach(
                items: value.Prefix,
                eachItem: (i, ctxt) => ctxt.CallTemplateDynamic(i.Value));
            ctxt.Foreach(
                items: value.Attributes,
                eachItem: (i, ctxt) => ctxt.CallTemplateDynamic(i.Value, CBTemplateProvider.Attribute));
            ctxt.CallTemplateStrict(value.AccessibilityLevel)
                .If(value.IsAbstract, (ctxt) => ctxt.Write("abstract "))
                .If(value.IsSealed, (ctxt) => ctxt.Write("sealed "))
                .If(value.IsPartial, (ctxt) => ctxt.Write("partial "))
                .Write("class ")
                .Write(value.TypeName)
                .CallTemplateStrict<CBCodeTypeDefinition>(value, "BaseTypes")
                .Write(" {").WriteLine();
            foreach (var grp in value.Members
                .GroupBy(m => m switch { CBCodeDefinitionField => 0, CBCodeDefinitionConstructor => 1, CBCodeDefinitionProperty => 2, CBCodeDefinitionMethod => 3, _ => 4 })
                .OrderBy(kv => kv.Key)
                ) {
                foreach (var member in grp) {
                    ctxt.CallTemplateDynamic(member);
                }
                ctxt.WriteLine();
            }

            ctxt.IndentDecr().Write("}").WriteLine();
        }
    }

    public sealed class CBTemplateCSharpClassTypeName : CBNamedTemplate<CBCodeClass> {
        public CBTemplateCSharpClassTypeName()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.TypeName) {
        }

        public override void RenderT(CBCodeClass value, CBRenderContext ctxt) {
            ctxt.Write(value.Namespace.Namespace).Write(".").Write(value.TypeName);
            
        }
    }
}