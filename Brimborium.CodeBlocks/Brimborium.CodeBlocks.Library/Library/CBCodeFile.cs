using System.Collections.Generic;
using System.Linq;

namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeImport : ICBCodeElement {
    }

    public record CBCodeImportNamespace(CBCodeNamespace Namespace) : ICBCodeImport {
        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];

        public override string ToString() {
            return this.Namespace.Namespace;
        }
    }

    public record CBCodeImportAlias(string ReferencedType, string Alias) : ICBCodeImport {
        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];

        public override string ToString() {
            return this.Alias;
        }
    }

    public class CBCodeFile : ICBCodeElement {
        public CBCodeFile() {
            this.Imports = new HashSet<ICBCodeImport>();
            this.Namespace = new CBCodeNamespace();
            this.Items = new CBList<ICBCodeTypeDefinition>(this);
        }

        public HashSet<ICBCodeImport> Imports { get; }
        public CBCodeNamespace Namespace { get; set; }
        public CBList<ICBCodeTypeDefinition> Items { get; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() {
            var result = new List<ICBCodeElement>();
            result.AddRange(this.Imports);
            result.AddRange(this.Items);
            return result;
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
            ctxt.Write("namespace ").Write(value.Namespace.Namespace)
                .Write(" {").WriteLine().IndentIncr();
            foreach (var item in value.Items) {
                ctxt.CallTemplateDynamic(item);
            }
            ctxt.IndentDecr().Write("}").WriteLine();
        }
    }

    public sealed class CBTemplateCSharpImportNamespaceCommon : CBNamedTemplate<CBCodeImportNamespace> {
        public CBTemplateCSharpImportNamespaceCommon()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Common) {
        }

        public override void RenderT(CBCodeImportNamespace value, CBRenderContext ctxt) {
            ctxt.Write($"using {value.Namespace.Namespace};").WriteLine();
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
