using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeStatement : ICBCodeElement {
    }

    public sealed class CBCodeBlock : ICBCodeStatement {
        public CBCodeBlock() {
            this.Statements = new CBList<ICBCodeStatement>(this);
        }

        public CBList<ICBCodeStatement> Statements { get; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => this.Statements;
    }

    public sealed class CBTemplateCSharpBlock : CBNamedTemplate<CBCodeBlock> {
        public CBTemplateCSharpBlock()
            : base(CBTemplateProvider.CSharp, string.Empty) {
        }

        public override void RenderT(CBCodeBlock value, CBRenderContext ctxt) {
            foreach (var statement in value.Statements) {
                ctxt.CallTemplate(statement);
            }
        }
    }

    public sealed class CBCodeAssignment : ICBCodeStatement {
        public CBCodeAssignment() {
            this.Expressions = new CBList<ICBCodeExpression>(this);
        }

        public CBList<ICBCodeExpression> Expressions { get; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => this.Expressions;
    }

    public sealed class CBTemplateCSharpAssignment : CBNamedTemplate<CBCodeAssignment> {
        public CBTemplateCSharpAssignment()
            : base(CBTemplateProvider.CSharp, string.Empty) {
        }

        public override void RenderT(CBCodeAssignment value, CBRenderContext ctxt) {
            ctxt.Foreach(
                items: value.Expressions,
                eachItem: (i, ctxt) => {
                    ctxt.CallTemplateDynamic(i.Value);
                    if (i.IsFirst) { } else { ctxt.Write(" = "); }
                    if (i.IsLast) {
                        ctxt.Write(";").WriteLine();
                    }
                }
                );
        }
    }

    public sealed class CBCodeStatement : ICBCodeStatement {
        public CBCodeStatement() {
            this.Lines = new List<string>();
        }

        public List<string> Lines { get; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];
    }

    public sealed class CBTemplateCSharpStatement : CBNamedTemplate<CBCodeStatement> {
        public CBTemplateCSharpStatement()
            : base(CBTemplateProvider.CSharp, string.Empty) {
        }

        public override void RenderT(CBCodeStatement value, CBRenderContext ctxt) {
            foreach (var line in value.Lines) {
                ctxt.Write(line).WriteLine();
            }
        }
    }
}