using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public sealed class CBCodeCustomExpression : ICBCodeExpression {
        public CBCodeCustomExpression(string text) {
            this.Text = text;
        }
        public CBCodeType? ResultType { get; set; }
        public ICBCodeElement? Parent { get; set; }
        public string Text { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];
    }
    public sealed class CBTemplateCSharpCustomExpression : CBNamedTemplate<CBCodeCustomExpression> {
        public CBTemplateCSharpCustomExpression()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Expression) {
        }

        public override void RenderT(CBCodeCustomExpression value, CBRenderContext ctxt) {
            ctxt.Write(value.Text);
        }
    }
}