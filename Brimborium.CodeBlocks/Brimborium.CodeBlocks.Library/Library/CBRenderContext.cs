using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Brimborium.CodeBlocks.Library {
    public sealed class CBRenderContext {
        public CBRenderContext(CBTemplateProvider templateProvider, IndentedTextWriter writer) {
            this.TemplateProvider = templateProvider;
            this.Writer = writer;
        }

        public CBTemplateProvider TemplateProvider { get; }

        public IndentedTextWriter Writer { get; }

        public int Indent { get => this.Writer.Indent; set => this.Writer.Indent = value; }

        public CBRenderContext IndentIncr() {
            this.Writer.Indent++;
            return this;
        }

        public CBRenderContext IndentDecr() {
            this.Writer.Indent--;
            return this;
        }

        public CBRenderContext WriteLine() {
            this.Writer.WriteLine();
            return this;
        }

        public CBRenderContext WriteLine(int indent) {
            this.Writer.WriteLine();
            this.Writer.Indent += indent;
            return this;
        }

        public CBRenderContext WriteLine(string text) {
            this.Writer.WriteLine(text);
            return this;
        }

        public CBRenderContext Write(string text) {
            this.Writer.Write(text);
            return this;
        }

        public CBRenderContext CallTemplateDynamic<T>([AllowNull] T value, string? name = default) {
            if (value is null) {
                //CBTemplate template = this.TemplateProvider.GetTemplate(typeof(T), typeof(T), name ?? string.Empty);
                //if (template is CBTemplate<T> templateT) {
                //    templateT.RenderT(value!, this);
                //} else {
                //    template.Render(value, this);
                //}
            } else {
                CBTemplate template = this.TemplateProvider.GetTemplate(value.GetType(), typeof(T), name ?? string.Empty);
                if (template is CBTemplate<T> templateT) {
                    templateT.RenderT(value, this);
                } else {
                    template.Render(value, this);
                }
            }
            return this;
        }


        public CBRenderContext CallTemplate<T>(T value, string? name = default) {
            CBTemplate template = this.TemplateProvider.GetTemplate(typeof(T), name ?? string.Empty);
            if (template is CBTemplate<T> templateT) {
                templateT.RenderT(value, this);
            } else {
                template.Render(value, this);
            }
            return this;
        }

        public CBRenderContext If(bool condition, Action<CBRenderContext> Then, Action<CBRenderContext>? Else = default) {
            if (condition) {
                Then(this);
            } else {
                if (Else is not null) {
                    Else(this);
                }
            }
            return this;
        }

        public CBRenderContext Foreach<T>(
            IEnumerable<T> items,
            Action<IteratedValue<T>, CBRenderContext> eachItem,
            Action<CBRenderContext>? isEmpty=default) {
            var lastIndex = items.Count() - 1;
            if (lastIndex < 0) {
                if (isEmpty is not null) {
                    isEmpty(this);
                }
            } else {
                var iteratedValues = items.Select((item, index) => new IteratedValue<T>(item, index == 0, index == lastIndex, index));
                foreach (var iteratedValue in iteratedValues) {
                    eachItem(iteratedValue, this);
                }
            }
            return this;
        }
    }

    public record IteratedValue<T>(T Value, bool IsFirst, bool IsLast, int Index);
}
