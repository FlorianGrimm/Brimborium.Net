using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public sealed class CBCodeAssignment : ICBCodeElement {
        public CBCodeAssignment() {
            this.Expressions = new CBList<ICBCodeExpression>(this);
        }

        public string? VariableName { get; set; }

        public CBCodeType? VariableType { get; set; }

        public CBList<ICBCodeExpression> Expressions { get; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => this.Expressions;
    }

    public sealed class CBTemplateCSharpAssignment : CBNamedTemplate<CBCodeAssignment> {
        public CBTemplateCSharpAssignment()
            : base(CBTemplateProvider.CSharp, string.Empty) {
        }

        public override void RenderT(CBCodeAssignment value, CBRenderContext ctxt) {
            if (!string.IsNullOrEmpty(value.VariableName)) {
                if (value.VariableType is null) {
                    ctxt.Write("var ");
                } else {
                    ctxt.CallTemplateDynamic(value.VariableType).Write(" ");
                }
                ctxt.Write(value.VariableName);
                if (value.Expressions.Count > 0) {
                    ctxt.Write(" = ");
                }
            }
            ctxt.Foreach(
                items: value.Expressions,
                eachItem: (i, ctxt) => {
                    ctxt.CallTemplateDynamic(i.Value);
                    if (i.IsFirst) { } else { ctxt.Write(" = "); }
                }
                );
            ctxt.Write(";").WriteLine();
        }
    }
}