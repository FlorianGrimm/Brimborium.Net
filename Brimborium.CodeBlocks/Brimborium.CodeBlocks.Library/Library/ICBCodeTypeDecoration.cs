using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeTypeDecoration : ICBCodeElement {
    }
    public class CBCodeTypeAttribute : ICBCodeTypeDecoration {
        public CBCodeTypeAttribute(CBCodeType type) {
            this.Type = type;
            this.Arguments = new CBList<ICBCodeElement>(this);
        }

        public CBList<ICBCodeElement> Arguments { get; }

        public CBCodeType Type { get; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[] { this.Type };
    }

    public sealed class CBTemplateCSharpTypeAttributeDeclaration : CBNamedTemplate<CBCodeTypeAttribute> {
        public CBTemplateCSharpTypeAttributeDeclaration()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Declaration) {
        }

        public override void RenderT(CBCodeTypeAttribute value, CBRenderContext ctxt) {
            ctxt.Write("[").CallTemplateDynamic(value.Type, CBTemplateProvider.Attribute).Write("]").WriteLine();
        }
    }
}