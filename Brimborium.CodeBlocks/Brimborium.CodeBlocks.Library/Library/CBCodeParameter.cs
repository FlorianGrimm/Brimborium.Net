using System;
using System.Collections.Generic;
using System.Reflection;

namespace Brimborium.CodeBlocks.Library {
    public class CBCodeParameter : ICBCodeElement {
        public static CBCodeParameter FromClr(ParameterInfo parameterInfo) {
            var result = new CBCodeParameter(
                parameterInfo.Name ?? string.Empty,
                CBCodeType.FromClr(parameterInfo.ParameterType)) {
                IsOut = parameterInfo.IsOut,
                IsOptional = parameterInfo.IsOptional,
                IsRef = parameterInfo.IsRetval
            };

            return result;
        }

        public CBCodeParameter() {
            this.Name = string.Empty;
            this.Type = CBCodeType.FromClr(typeof(object));
        }

        public CBCodeParameter(
            string name,
            CBCodeType type) {
            this.Name = name;
            this.Type = type;
        }

        public CBCodeParameter(CBCodeParameter src) {
            this.Name = src.Name;
            this.Type = src.Type;
            this.IsOut = src.IsOut;
            this.IsRef = src.IsRef;
            this.IsOptional = src.IsOptional;
        }


        public string Name { get; set; }

        public CBCodeType Type { get; set; }

        public bool IsOut { get; set; }

        public bool IsRef { get; set; }

        public bool IsOptional { get; set; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[] { this.Type };
    }

    public sealed class CBTemplateCSharpParameterDeclaration : CBNamedTemplate<CBCodeParameter> {
        public CBTemplateCSharpParameterDeclaration()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Declaration) {
        }

        public override void RenderT(CBCodeParameter value, CBRenderContext ctxt) {
            ctxt.If(value.IsOut, ctxt => ctxt.Write("out "))
                .If(value.IsRef, ctxt => ctxt.Write("ref "))
                .CallTemplateDynamic(value.Type, CBTemplateProvider.TypeName)
                .Write(" ")
                .Write(value.Name)
                ;
        }
    }

    public sealed class CBTemplateCSharpParameterExpression : CBNamedTemplate<CBCodeParameter> {
        public CBTemplateCSharpParameterExpression()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Expression) {
        }

        public override void RenderT(CBCodeParameter value, CBRenderContext ctxt) {
            if (string.IsNullOrEmpty(value.Name)) {
                ctxt.Write("parameter has no name");
            } else { 
                ctxt.Write(value.Name);
            }
        }
    }
}