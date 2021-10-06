using Brimborium.CodeBlocks.Tool;

using System.CodeDom.Compiler;
using System.IO;
using System.Text;

namespace Brimborium.CodeBlocks.Library {
    public class CBCodeFileGeneration {

        public CBCodeFileGeneration(CBCodeFile codeFile, string fileName) {
            this.CodeFile = codeFile;
            this.FileName = fileName;
        }

        public CBCodeFile CodeFile { get; }
        public string FileName { get; }

        public virtual void Generate(
            ToolService toolService,
            CBTemplateProvider templateProvider,
            string? name = default
            ) {
            var sbOutput = new StringBuilder();
            using (var writer = new IndentedTextWriter(new StringWriter(sbOutput), "    ")) {
                CBRenderContext ctxt = new CBRenderContext(templateProvider, writer);
                ctxt.CallTemplateDynamic(this.CodeFile, name);
                writer.Flush();
            }
            toolService.SetFileContent(new CBFileContent(this.FileName, sbOutput.ToString()));
        }
    }
}
