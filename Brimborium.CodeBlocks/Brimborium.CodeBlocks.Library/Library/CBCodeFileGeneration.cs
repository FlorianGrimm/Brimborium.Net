using Brimborium.CodeBlocks.Tool;

namespace Brimborium.CodeBlocks.Library {
    public class CBCodeFileGeneration {
        public CBCodeFileGeneration() {
            this.FileName = string.Empty;
        }

        public CBCodeFileGeneration(string fileName) {
            this.FileName = fileName;
        }

        public string FileName { get; set; }

        public virtual void Generate(
            ToolService toolService,
            CBTemplateProvider templateProvider,
            string? name = default
            ) {
            //var sbOutput = new StringBuilder();
            //using (var writer = new IndentedTextWriter(new StringWriter(sbOutput), "    ")) {
            //    CBRenderContext ctxt = new CBRenderContext(templateProvider, writer);
            //    ctxt.CallTemplateDynamic(this.CodeFile, name);
            //    writer.Flush();
            //}
            //toolService.SetFileContent(new CBFileContent(this.FileName, sbOutput.ToString()));
        }
    }
}
