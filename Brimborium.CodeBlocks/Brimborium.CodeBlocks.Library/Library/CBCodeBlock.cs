using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public sealed class CBCodeBlock : ICBCodeElement {
        public CBCodeBlock() {
            this.Statements = new CBList<ICBCodeElement>(this);
        }

        public CBList<ICBCodeElement> Statements { get; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => this.Statements;
    }

    public sealed class CBTemplateCSharpBlock : CBNamedTemplate<CBCodeBlock> {
        public CBTemplateCSharpBlock()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Expression) {
        }

        public override void RenderT(CBCodeBlock value, CBRenderContext ctxt) {
            foreach (var statement in value.Statements) {
                ctxt.CallTemplateDynamic(statement);
            }
        }
    }
}