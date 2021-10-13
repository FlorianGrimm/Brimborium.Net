using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public sealed class CBCodeSimpleExpression : ICBCodeExpression {
        public CBCodeSimpleExpression() {
        }

        public CBCodeSimpleExpression(
            string? prefix = default,
            ICBCodeExpression? value = default,
            string? suffix = default
            ) {
            this.Prefix = prefix;
            this.Value = value;
            this.Suffix = suffix;
        }

        public CBCodeType? ResultType { get; set; }

        public string? Prefix { get; set; }

        public ICBCodeExpression? Value { get; set; }

        public string? Suffix { get; set; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];
    }

    public sealed class CBTemplateCSharpSimpleExpression : CBNamedTemplate<CBCodeSimpleExpression> {
        public CBTemplateCSharpSimpleExpression()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Expression) {
        }

        public override void RenderT(CBCodeSimpleExpression value, CBRenderContext ctxt) {
            if (value.Prefix is not null) {
                ctxt.Write(value.Prefix);
            }
            if (value.Value is not null) {
                ctxt.CallTemplateDynamic(value.Value, CBTemplateProvider.Expression);
            }
            if (value.Suffix is not null) {
                ctxt.Write(value.Suffix);
            }
        }
    }
}