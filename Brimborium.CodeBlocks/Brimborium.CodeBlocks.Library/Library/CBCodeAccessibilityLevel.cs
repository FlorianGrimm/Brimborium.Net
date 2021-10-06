namespace Brimborium.CodeBlocks.Library {
    public enum CBCodeAccessibilityLevel { NotSpecified, Public, Protected, Internal, ProtectedInternal, Private }

    public sealed class CBTemplateCSharpAccessibilityLevelCommon : CBNamedTemplate<CBCodeAccessibilityLevel> {
        public CBTemplateCSharpAccessibilityLevelCommon()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Common) {
        }

        public override void RenderT(CBCodeAccessibilityLevel value, CBRenderContext ctxt) {
            switch (value) {
                case CBCodeAccessibilityLevel.NotSpecified:
                    break;
                case CBCodeAccessibilityLevel.Public:
                    ctxt.Write("public ");
                    break;
                case CBCodeAccessibilityLevel.Protected:
                    ctxt.Write("protected ");
                    break;
                case CBCodeAccessibilityLevel.Internal:
                    ctxt.Write("internal ");
                    break;
                case CBCodeAccessibilityLevel.ProtectedInternal:
                    ctxt.Write("protected internal ");
                    break;
                case CBCodeAccessibilityLevel.Private:
                    ctxt.Write("private ");
                    break;
                default:
                    ctxt.Write($"/* unexpected {value} */ ");
                    break;
            }
        }
    }
}