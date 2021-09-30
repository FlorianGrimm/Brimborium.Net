using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brimborium.CodeBlocks.Library {
    public class CBRendererOutMost : CBRenderer {
        private readonly CBRenderer _Inner;

        // 0 - empty line 
        // 1 - WriteText called
        // 2 - EnsureNewLine called
        public int LineMode;

        public Stack<CBTemplate> TemplateStack { get; }

        public CBRendererOutMost(CBRenderer inner) {
            this._Inner = inner;
            this.LineMode = 0;
            this.TemplateStack = new Stack<CBTemplate>();
        }

        public CBRenderer Inner => this._Inner;

        public override void WriteText(string text) {
            if (this.LineMode == 2) {
                this._Inner.WriteLine(string.Empty);
                this.LineMode = 0;
            }
            this._Inner.WriteText(text);
            this.LineMode = 1;
        }

        public override void WriteLine(string text) {
            if (this.LineMode == 2) {
                this._Inner.WriteLine(string.Empty);
                this.LineMode = 0;
            } else { 
                this.LineMode = 0;
            }
            this._Inner.WriteLine(text);
        }

        public override void EnsureNewLine(bool newLine = false) {
            if (newLine) {
                this._Inner.WriteLine(string.Empty);
                this.LineMode = 0;
            } else {
                this.LineMode = 2;
                if (this.LineMode == 1) {
                } else {
                    this._Inner.WriteLine(string.Empty);
                    this.LineMode = 0;
                }
            }
        }

        public override async ValueTask CloseAsync() {
            if (this.LineMode == 2) {
                this.LineMode = 0;
                this._Inner.WriteLine(string.Empty);
            }
            await this.Inner.CloseAsync();
        }

        public override async ValueTask CallTemplate(CBTemplate template, CBValue context) {
            try {
                await this.BeginTemplate(template);
                await template.Render(context, this);
            } finally {
                await this.EndTemplate(template);
            }
        }

        public override ValueTask BeginTemplate(CBTemplate template) {
            this.TemplateStack.Push(template);
            return ValueTask.CompletedTask;
        }

        public override ValueTask EndTemplate(CBTemplate template) {
            var topTemplate = this.TemplateStack.Peek();
            if (ReferenceEquals(topTemplate, template)) {
                this.TemplateStack.Pop();
            } else {
                throw new ArgumentException($"EndTemplate {topTemplate?.GetType().FullName} vs {template.GetType().FullName}");
            }
            return ValueTask.CompletedTask;
        }
    }
}
