using System;
using System.Threading.Tasks;

namespace Brimborium.CodeBlocks.Library {
    public class CBRenderer {
        public CBRenderer() {
        }

        public virtual CBRendererIndent Indent() {
            this.BeginIndent();
            return new CBRendererIndent(this);
        }

        public virtual void BeginIndent() { }

        public virtual void EndIndent() { }

        public virtual void WriteText(string text) {
        }

        public virtual void WriteLine(string text) {
        }

        public virtual void EnsureNewLine(bool newLine = false) {
        }

        public virtual ValueTask CloseAsync() {
            return ValueTask.CompletedTask;
        }

        public virtual async ValueTask CallTemplate(
            CBTemplate template,
            CBValue context
            ) {
            try {
                await this.BeginTemplate(template);
                await template.Render(context, this);
            } finally {
                await this.EndTemplate(template);
            }
        }

        public virtual ValueTask BeginTemplate(CBTemplate template) {
            return ValueTask.CompletedTask;
        }

        public virtual ValueTask EndTemplate(CBTemplate template) {
            return ValueTask.CompletedTask;
        }
    }

    public struct CBRendererIndent : IDisposable {
        private readonly CBRenderer _Render;

        public CBRendererIndent(CBRenderer render) {
            this._Render = render;
        }

        public void Dispose() {
            this._Render.EndIndent();
        }
    }

    public struct CBRendererTemplate : IDisposable {
        public readonly CBRenderer Render;
        public readonly CBTemplate Template;

        public CBRendererTemplate(CBRenderer render, CBTemplate template) {
            this.Render = render;
            this.Template = template;
        }

        public void Dispose() {
            this.Render.EndTemplate(this.Template);
        }
    }



    public class CBRendererOuter : CBRenderer {
        private readonly CBRenderer _Inner;

        public CBRendererOuter(CBRenderer inner) {
            this._Inner = inner;
        }

        public CBRenderer Inner => this._Inner;
    }
}
