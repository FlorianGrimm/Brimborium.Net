using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeParameter: ICBCodeElement {
    }

    public class CBCodeParameter : ICBCodeParameter {
        public CBCodeParameter(
            string name,
            ICBCodeTypeReference type) {
            this.Name = name;
            this.Type = type;
        }

        public bool IsOut { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICBCodeTypeReference Type { get; set; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[] { this.Type };
    }

    public sealed class CBTemplateCSharpParameter : CBNamedTemplate<CBCodeParameter> {
        public CBTemplateCSharpParameter()
            : base(CBTemplateProvider.CSharp, string.Empty) {
        }

        public override void RenderT(CBCodeParameter value, CBRenderContext ctxt) {
            ctxt.If(value.IsOut, ctxt=>ctxt.Write("out "))
                .CallTemplateDynamic(value.Type)
                .Write(" ")
                .Write(value.Name)
                ;
        }
    }
}