using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

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

        public CBRenderContext CallTemplateDynamic([AllowNull] object value) {
            if (value is null) {
                //
            } else {
                CBTemplate template = this.TemplateProvider.GetTemplate(value.GetType(), string.Empty);
                template.Render(value, this);
            }
            return this;
        }

        public CBRenderContext CallTemplate<T>(T value) {
            CBTemplate template = this.TemplateProvider.GetTemplate(typeof(T), string.Empty);
            if (template is CBTemplate<T> templateT) {
                templateT.RenderT(value, this);
            } else {
                template.Render(value, this);
            }
            return this;
        }

        public CBRenderContext CallTemplate<T>(T value, string name) {
            CBTemplate template = this.TemplateProvider.GetTemplate(typeof(T), name);
            if (template is CBTemplate<T> templateT) {
                templateT.RenderT(value, this);
            } else {
                template.Render(value, this);
            }
            return this;
        }

        public static CBRenderContext operator +(CBRenderContext that, string? text) {
            if (text is not null) {
                that.Write(text);
            }
            return that;
        }

        public static CBRenderContext operator -(CBRenderContext that, int diffIndent) {
            that.Writer.WriteLine();
            if (diffIndent == 0) {
            } else {
                that.Writer.Indent += diffIndent;
            }
            return that;
        }
        public static CBRenderContext operator /(CBRenderContext that, int diffIndent) {
            that.Writer.WriteLine();
            if (diffIndent == 0) {
            } else {
                that.Writer.Indent += diffIndent;
            }
            return that;
        }
    }
}
