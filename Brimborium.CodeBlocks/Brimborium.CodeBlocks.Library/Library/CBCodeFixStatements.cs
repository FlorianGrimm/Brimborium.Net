using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public sealed class CBCodeFixStatements : ICBCodeElement {
        public CBCodeFixStatements() {
            this.Lines = new List<string>();
        }

        public List<string> Lines { get; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];
    }

    public sealed class CBTemplateCSharpStatement : CBNamedTemplate<CBCodeFixStatements> {
        public CBTemplateCSharpStatement()
            : base(CBTemplateProvider.CSharp, string.Empty) {
        }

        public override void RenderT(CBCodeFixStatements value, CBRenderContext ctxt) {
            foreach (var line in value.Lines) {
                ctxt.Write(line).WriteLine();
            }
        }
    }
}