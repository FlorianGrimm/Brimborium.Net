using Brimborium.CodeBlocks.Tool;

using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeImport : ICBCodeElement {
    }

    public record CBCodeImportNamespace(CBCodeNamespace Namespace) : ICBCodeImport {
        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];

        public override string ToString() {
            return this.Namespace.Name;
        }
    }

    public record CBCodeImportAlias(string ReferencedType, string Alias) : ICBCodeImport {
        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];

        public override string ToString() {
            return this.Alias;
        }
    }

    public class CBCodeFile : CBCodeFileGeneration, ICBCodeElement {
        public static (CBCodeFile, CBCodeType) CreateFileAndType(CBCodeNamespace @namespace, string typeName, string fileName) {
            var codeFile = new CBCodeFile() { FileName=fileName };
            var codeType = new CBCodeType() { Namespace = @namespace, Name = typeName };
            codeFile.Namespace = @namespace;
            codeFile.Items.Add(codeType);
            return (codeFile, codeType);
        }

        public CBCodeFile() {
            this.FileName = string.Empty;
            this.Imports = new HashSet<ICBCodeImport>();
            this.Namespace = new CBCodeNamespace();
            this.Items = new CBList<ICBCodeElement>(this);
        }

        public HashSet<ICBCodeImport> Imports { get; }

        public CBCodeNamespace Namespace { get; set; }

        public CBList<ICBCodeElement> Items { get; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() {
            var result = new List<ICBCodeElement>();
            result.AddRange(this.Imports);
            result.AddRange(this.Items);
            return result;
        }

        public override void Generate(
            ToolService toolService,
            CBTemplateProvider templateProvider,
            string? name = default
            ) {
            var sbOutput = new StringBuilder();
            using (var writer = new IndentedTextWriter(new StringWriter(sbOutput), "    ")) {
                CBRenderContext ctxt = new CBRenderContext(templateProvider, writer);
                ctxt.CallTemplateDynamic(this, name);
                writer.Flush();
            }
            toolService.SetFileContent(new CBFileContent(this.FileName, sbOutput.ToString()));
        }
    }

    public sealed class CBTemplateCSharpCodeFileCommon : CBNamedTemplate<CBCodeFile> {
        public CBTemplateCSharpCodeFileCommon() 
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Common) {
        }

        public override void RenderT(CBCodeFile value, CBRenderContext ctxt) {
            var grps = value.Imports
                .OrderBy(import => import.ToString())
                .GroupBy(import => import.ToString()!.Split('.').FirstOrDefault() ?? string.Empty);
            foreach (var grp in grps) {
                foreach (var ns in grp) {
                    ctxt.Write($"using {ns};").WriteLine();
                }
                ctxt.Write("").WriteLine();
            }
            ctxt.Write("namespace ").Write(value.Namespace.Name)
                .Write(" {").WriteLine().IndentIncr();
            foreach (var item in value.Items) {
                ctxt.CallTemplateDynamic(item, CBTemplateProvider.Declaration).WriteLine();
            }
            ctxt.IndentDecr().Write("}").WriteLine();
        }
    }

    public sealed class CBTemplateCSharpImportNamespaceCommon : CBNamedTemplate<CBCodeImportNamespace> {
        public CBTemplateCSharpImportNamespaceCommon()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Common) {
        }

        public override void RenderT(CBCodeImportNamespace value, CBRenderContext ctxt) {
            ctxt.Write($"using {value.Namespace.Name};").WriteLine();
        }
    }

    public sealed class CBTemplateCSharpImportAliasCommon : CBNamedTemplate<CBCodeImportAlias> {
        public CBTemplateCSharpImportAliasCommon()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Common) {
        }

        public override void RenderT(CBCodeImportAlias value, CBRenderContext ctxt) {
            ctxt.Write($"using {value.Alias} = {value.ReferencedType};").WriteLine();
        }
    }
}
