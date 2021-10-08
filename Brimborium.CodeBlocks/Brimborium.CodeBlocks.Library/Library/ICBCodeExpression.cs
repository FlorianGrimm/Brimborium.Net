using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeExpression : ICBCodeElement {
    }

    public sealed class CBCodeCustomExpression : ICBCodeExpression {
        public CBCodeCustomExpression(string text) {
            this.Text = text;
        }

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
    public sealed class CBCodeAccessorExpression : ICBCodeExpression {
        public CBCodeAccessorExpression(params ICBCodeExpression[] codeExpressions) {
            this.Expressions = new List<ICBCodeExpression>(codeExpressions);
        }

        public ICBCodeElement? Parent { get; set; }
        public List<ICBCodeExpression> Expressions { get; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];
    }

    public sealed class CBTemplateCSharpAccessorExpression : CBNamedTemplate<CBCodeAccessorExpression> {
        public CBTemplateCSharpAccessorExpression()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Expression) {
        }

        public override void RenderT(CBCodeAccessorExpression value, CBRenderContext ctxt) {
            ctxt.Foreach(
                items: value.Expressions,
                eachItem: (i, ctxt) => {
                    if (i.IsFirst) { } else { ctxt.Write("."); }
                    ctxt.CallTemplateDynamic(i.Value, CBTemplateProvider.Expression);
                }
            );
        }
    }

    public sealed class CBCodeCallExpression : ICBCodeExpression {
        public CBCodeCallExpression(ICBCodeExpression method, params ICBCodeExpression[] arguments) {
            this.Method = method;
            this.Arguments = new List<ICBCodeExpression>(arguments);
        }

        public ICBCodeElement? Parent { get; set; }
        public ICBCodeExpression Method { get; set; }
        public List<ICBCodeExpression> Arguments { get; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];
    }

    public sealed class CBTemplateCSharpCallExpression : CBNamedTemplate<CBCodeCallExpression> {
        public CBTemplateCSharpCallExpression()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Expression) {
        }

        public override void RenderT(CBCodeCallExpression value, CBRenderContext ctxt) {
            ctxt.CallTemplateDynamic(value.Method, CBTemplateProvider.Expression);
            ctxt.Write("(");
            ctxt.Foreach(
                items: value.Arguments,
                eachItem: (i, ctxt) => {
                    if (i.IsFirst) { } else { ctxt.Write(", "); }
                    ctxt.CallTemplateDynamic(i.Value, CBTemplateProvider.Expression);
                }
            );
            ctxt.Write(")");
        }
    }

    public sealed class CBCodeFieldExpression : ICBCodeExpression {
        public readonly CBCodeDefinitionField Field;

        public CBCodeFieldExpression(CBCodeDefinitionField definitionField) {
            this.Field = definitionField;
        }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];
    }

    public sealed class CBTemplateCSharpFieldExpression : CBNamedTemplate<CBCodeFieldExpression> {
        public CBTemplateCSharpFieldExpression()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Expression) {
        }

        public override void RenderT(CBCodeFieldExpression value, CBRenderContext ctxt) {
            //ctxt.CallTemplateDynamic(value.Field, CBTemplateProvider.Expression);
            ctxt.Write(value.Field.Name);
        }
    }

    public sealed class CBCodeParameterExpression : ICBCodeExpression {
        public readonly CBCodeParameter Parameter;

        public CBCodeParameterExpression(CBCodeParameter parameter) {
            this.Parameter = parameter;
        }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];
    }

    public sealed class CBTemplateCSharpParameterExpression : CBNamedTemplate<CBCodeParameterExpression> {
        public CBTemplateCSharpParameterExpression()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Expression) {
        }

        public override void RenderT(CBCodeParameterExpression value, CBRenderContext ctxt) {
            ctxt.Write(value.Parameter.Name);
        }
    }

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