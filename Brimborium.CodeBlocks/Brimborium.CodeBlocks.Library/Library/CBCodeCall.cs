using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public class CBCodeCall : ICBCodeExpression {
        public CBCodeCall() {
            this.Arguments = new CBList<ICBCodeElement>(this);
        }

        public ICBCodeElement? Method { get; set; }

        // public string? Name { get; set; }

        public CBList<ICBCodeElement> Arguments { get; }

        public CBCodeType? ResultType { get; set; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];
    }

    public sealed class CBTemplateCSharpCallExpression : CBNamedTemplate<CBCodeCall> {
        public CBTemplateCSharpCallExpression()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Expression) {
        }

        public override void RenderT(CBCodeCall value, CBRenderContext ctxt) {
            if (value.Method is not null) {
                ctxt.CallTemplateDynamic(value.Method);
            }
            ctxt.Write("(");
            bool newLines = value.Arguments.Count > 1;
            if (newLines) {
                ctxt.WriteLine().IndentIncr();
            }
            ctxt.Foreach(
                value.Arguments,
                (i, ctxt) => {
                    if (i.IsLast) {
                    } else {
                        if (newLines) {
                            ctxt.Write(",").WriteLine();
                        } else {
                            ctxt.Write(", ");
                        }
                    }
                }
                );
            if (newLines) { ctxt.IndentDecr(); }
            ctxt.Write(")");
        }
    }
}