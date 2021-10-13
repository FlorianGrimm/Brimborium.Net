using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public class CBCodeAccessor : ICBCodeExpression {
        public CBCodeAccessor(params ICBCodeElement[] expressions) {
            this.Expressions = new CBList<ICBCodeElement>(this);
            if (expressions.Length > 0) {
                this.Expressions.AddRange(expressions);
            }
        }

        public bool This { get; set; }

        public bool Base { get; set; }

        public CBList<ICBCodeElement> Expressions { get; }

        public CBCodeType? ResultType { get; set; }
        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];
    }

    public sealed class CBTemplateCSharpAccessorExpression : CBNamedTemplate<CBCodeAccessor> {
        public CBTemplateCSharpAccessorExpression()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Expression) {
        }

        public override void RenderT(CBCodeAccessor value, CBRenderContext ctxt) {
            bool dot = false;
            if (value.This) {
                ctxt.Write("this");
                dot = true;
            } else if (value.Base) {
                ctxt.Write("base");
                dot = true;
            }
            ctxt.Foreach(
                value.Expressions,
                (i, ctxt) => {
                    if (i.IsFirst) {
                        if (dot) {
                            ctxt.Write(".");
                        }
                    } else {
                        ctxt.Write(".");
                    }

                    ctxt.CallTemplateDynamic(i.Value);
                });
        }
    }
}