using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeTypeDecoration : ICBCodeElement {
    }

    public sealed class CBCodeTypeAttribute : ICBCodeTypeDecoration {
        public CBCodeTypeAttribute(ICBCodeTypeReference typeReference) {
            this.TypeReference = typeReference;
        }

        public ICBCodeTypeReference TypeReference { get; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[] { this.TypeReference };
    }

    public sealed class CBTemplateCSharpTypeAttribute : CBNamedTemplate<CBCodeTypeAttribute> {
        public CBTemplateCSharpTypeAttribute()
            : base(CBTemplateProvider.CSharp, string.Empty) {
        }

        public override void RenderT(CBCodeTypeAttribute value, CBRenderContext ctxt) {
            ctxt.Write("[").CallTemplateDynamic(value.TypeReference, "Attribute").Write("]").WriteLine();
        }
    }
}