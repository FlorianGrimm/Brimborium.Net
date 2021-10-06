using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeExpression : ICBCodeElement {
    }

    public sealed class CBCodeSimpleExpression : ICBCodeExpression {
        public CBCodeSimpleExpression() {
        }

        public CBCodeSimpleExpression(
            string? prefix,
            ICBCodeExpression? value,
            string? suffix
            ) {
            this.Prefix = prefix;
            this.Value = value;
            this.Suffix = suffix;
        }

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

    public class CBCodeComplexExpression : ICBCodeExpression {
        public CBCodeComplexExpression() {
            this.Prefix = new List<string?>();
            this.Value = new List<ICBCodeExpression?>();
            this.Suffix = new List<string?>();
        }

        public CBCodeComplexExpression Add(
            string? prefix,
            ICBCodeExpression? value,
            string? suffix
            ) {
            this.Prefix.Add(prefix);
            this.Value.Add(value);
            this.Suffix.Add(suffix);
            return this;
        }

        public List<string?> Prefix { get; }

        public List<ICBCodeExpression?> Value { get; }

        public List<string?> Suffix { get; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() {
            var result = new List<ICBCodeElement>();
            foreach (var item in this.Value) {
                if (item is not null) { 
                    result.Add(item);
                }
            }
            return result;
        }
    }

    public sealed class CBTemplateCSharpComplexExpression : CBNamedTemplate<CBCodeComplexExpression> {
        public CBTemplateCSharpComplexExpression()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Expression) {
        }

        public override void RenderT(CBCodeComplexExpression value, CBRenderContext ctxt) {
            for (int idx = 0; idx < value.Value.Count; idx++) {
                if (value.Prefix[idx] is string prefix) { 
                    ctxt.Write(prefix);
                }
                if (value.Value[idx] is ICBCodeExpression item) {
                    ctxt.CallTemplateDynamic(item, CBTemplateProvider.Expression);
                }
                if (value.Suffix[idx] is string suffix) {
                    ctxt.Write(suffix);
                }
            }
        }
    }
}