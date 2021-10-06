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

    public sealed class CBTemplateCSharpTypeNameReference : CBNamedTemplate<CBCodeTypeNameReference> {
        public CBTemplateCSharpTypeNameReference()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.TypeName) {
        }

        public override void RenderT(CBCodeTypeNameReference value, CBRenderContext ctxt) {
            ctxt.CallTemplateDynamic(value.CodeTypeName);
        }
    }

    public sealed class CBTemplateCSharpAttributeTypeNameReference : CBNamedTemplate<CBCodeTypeNameReference> {
        public CBTemplateCSharpAttributeTypeNameReference()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Attribute) {
        }

        public override void RenderT(CBCodeTypeNameReference value, CBRenderContext ctxt) {
            ctxt.Write(CSharpUtility.WihoutAttribute(value.CodeTypeName));
        }
    }

    public sealed class CBTemplateCSharpAttributeClrTypeReference : CBNamedTemplate<CBCodeClrTypeReference> {
        public CBTemplateCSharpAttributeClrTypeReference()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Attribute) {
        }

        public override void RenderT(CBCodeClrTypeReference value, CBRenderContext ctxt) {
            ctxt.Write(CSharpUtility.WihoutAttribute(value));
        }
    }
}