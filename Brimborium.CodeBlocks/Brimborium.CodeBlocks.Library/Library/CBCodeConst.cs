using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public sealed class CBCodeConst : ICBCodeElement {
        public CBCodeConst(string text) {
            this.Text = text;
        }
        public string Text { get; set; }

        public ICBCodeElement? Parent { get; set; }
        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];
    }

    public sealed class CBTemplateCSharpConst : CBNamedTemplate<CBCodeConst> {
        public CBTemplateCSharpConst()
            : base(CBTemplateProvider.CSharp, string.Empty) {
        }

        public override void RenderT(CBCodeConst value, CBRenderContext ctxt) {
            ctxt.Write(value.Text).WriteLine();
        }
    }
}