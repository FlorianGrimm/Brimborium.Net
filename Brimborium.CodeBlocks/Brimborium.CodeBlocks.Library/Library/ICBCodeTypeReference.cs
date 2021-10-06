using System.Collections.Generic;
using System.Diagnostics;

namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeTypeReference : ICBCodeElement {
        ICBCodeTypeName CodeTypeName { get; set; }
    }

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public sealed class CBCodeTypeNameReference : ICBCodeTypeReference {
        public CBCodeTypeNameReference(ICBCodeTypeName codeTypeName) {
            this.CodeTypeName = codeTypeName;
        }

        public ICBCodeTypeName CodeTypeName { get; set; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[] { this.CodeTypeName };

        public override string ToString() {
            return "->" + (this.CodeTypeName.ToString() ?? string.Empty);
        }

        private string GetDebuggerDisplay() {
            return "->" + (this.CodeTypeName.ToString() ?? string.Empty);
        }
    }

    public sealed class CBTemplateCSharpTypeNameReferenceTypeName : CBNamedTemplate<CBCodeTypeNameReference> {
        public CBTemplateCSharpTypeNameReferenceTypeName()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.TypeName) {
        }

        public override void RenderT(CBCodeTypeNameReference value, CBRenderContext ctxt) {
            ctxt.CallTemplateDynamic(value.CodeTypeName);
        }
    }

    public sealed class CBTemplateCSharpAttributeTypeNameReferenceAttribute : CBNamedTemplate<CBCodeTypeNameReference> {
        public CBTemplateCSharpAttributeTypeNameReferenceAttribute()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Attribute) {
        }

        public override void RenderT(CBCodeTypeNameReference value, CBRenderContext ctxt) {
#warning TODO
            ctxt.Write(CSharpUtility.WihoutAttribute(value.CodeTypeName));
        }
    }

    public sealed class CBTemplateCSharpAttributeClrTypeReferenceAttribute : CBNamedTemplate<CBCodeClrTypeReference> {
        public CBTemplateCSharpAttributeClrTypeReferenceAttribute()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Attribute) {
        }

        public override void RenderT(CBCodeClrTypeReference value, CBRenderContext ctxt) {
#warning TODO
            ctxt.Write(CSharpUtility.WihoutAttribute(value));
        }
    }
}