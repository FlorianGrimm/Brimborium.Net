using System.CodeDom.Compiler;

namespace Brimborium.CodeBlocks.Library {
    public class CBRenderIndentedTextWriter : CBRenderer {
        private IndentedTextWriter _IndentedTextWriter;

        public CBRenderIndentedTextWriter(IndentedTextWriter indentedTextWriter) {
            this._IndentedTextWriter = indentedTextWriter;
        }

        public IndentedTextWriter IndentedTextWriter => this._IndentedTextWriter;

        public override void WriteText(string text) {
            this._IndentedTextWriter.Write(text);
        }

        public override void WriteLine(string text) {
            this._IndentedTextWriter.WriteLine(text);
        }

        public override void EnsureNewLine(bool newLine = false) {
            this._IndentedTextWriter.WriteLine();
        }

        public override void BeginIndent() {
            this._IndentedTextWriter.Indent++;
        }

        public override void EndIndent() {
            this._IndentedTextWriter.Indent--;
        }
    }
}
