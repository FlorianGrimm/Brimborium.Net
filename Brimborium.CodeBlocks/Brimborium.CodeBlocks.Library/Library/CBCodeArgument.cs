using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public class CBCodeArgument : ICBCodeExpression {
        public CBCodeArgument() {

        }
        public CBCodeType? ResultType { get; set; }

        public CBCodeParameter? Parameter { get; set; }

        public string? Name { get; set; }

        public ICBCodeExpression? Value { get; set; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];
    }

    public sealed class CBTemplateCSharpArgumentExpression : CBNamedTemplate<CBCodeArgument> {
        public CBTemplateCSharpArgumentExpression()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Expression) {
        }

        public override void RenderT(CBCodeArgument value, CBRenderContext ctxt) {
            if (value.Parameter is not null) {
                ctxt.Write(value.Parameter.Name).Write(": ");
            } else if (!string.IsNullOrEmpty(value.Name)) {
                ctxt.Write(value.Name).Write(": ");
            }
            ctxt.CallTemplateDynamic(value.Value, CBTemplateProvider.Expression);
        }
    }
}