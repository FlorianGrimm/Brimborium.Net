using System;
using System.Collections.Generic;
using System.Reflection;

namespace Brimborium.CodeBlocks.Library {
    public class CBCodeMethod : ICBCodeTypeMember {
        public static CBCodeMethod FromClr(MethodInfo methodInfo) {
            var result = new CBCodeMethod();
            result.MethodInfo = methodInfo;
            result.Name = methodInfo.Name;
            result.ReturnType = CBCodeType.FromClr(methodInfo.ReturnType);
            result.IsStatic = methodInfo.IsStatic;
            result.IsAbstract = methodInfo.IsAbstract;
            result.IsVirtual = methodInfo.IsVirtual;
            result.IsSealed = methodInfo.IsFinal;
            //result.IsAsync
            foreach (var parameterInfo in methodInfo.GetParameters()) {
                result.Parameters.Add(CBCodeParameter.FromClr(parameterInfo));
            }

            return result;
        }

        public CBCodeMethod() {
            this.Parameters = new CBList<CBCodeParameter>(this);
        }

        public CBCodeMethod(CBCodeMethod src) : this() {
            this.AccessibilityLevel = src.AccessibilityLevel;
            this.IsStatic = src.IsStatic;
            this.IsAsync = src.IsAsync;
            this.IsVirtual = src.IsVirtual;
            this.IsSealed = src.IsSealed;
            this.IsOverride = src.IsOverride;
            this.ReturnType = src.ReturnType;
            this.Name = src.Name;
            foreach (var p in src.Parameters) {
                this.Parameters.Add(new CBCodeParameter(p));
            }
        }

        public MethodInfo? MethodInfo;

        public CBCodeAccessibilityLevel AccessibilityLevel { get; set; }

        public bool IsStatic { get; set; }

        public bool IsAsync { get; set; }

        public bool IsNew { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsVirtual { get; set; }

        public bool IsSealed { get; set; }

        public bool IsOverride { get; set; }

        public CBCodeType? ReturnType { get; set; }

        public string Name { get; set; } = string.Empty;

        public CBList<CBCodeParameter> Parameters { get; }

        public ICBCodeElement? Code { get; }

        public ICBCodeElement? Parent { get; set; }

        public IEnumerable<ICBCodeElement> GetChildren() {
            var result = new List<ICBCodeElement>();
            if (this.ReturnType is not null) {
                result.Add(this.ReturnType);
            }
            result.AddRange(this.Parameters);
            if (this.Code is not null) {
                result.Add(this.Code);
            }
            return result;
        }
    }


    public sealed class CBTemplateCSharpMethodDeclaration : CBNamedTemplate<CBCodeMethod> {
        public CBTemplateCSharpMethodDeclaration()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Declaration) {
        }

        public override void RenderT(CBCodeMethod value, CBRenderContext ctxt) {
            ctxt.CallTemplateDynamic(value.AccessibilityLevel).Write(" ");
            ctxt.CallTemplateDynamic(value.ReturnType, CBTemplateProvider.TypeName);
            ctxt.Write(" ");
            ctxt.Write(value.Name).Write("(").IndentIncr()
                .Foreach(
                    value.Parameters,
                    (i, ctxt) => {
                        ctxt.CallTemplateDynamic(i.Value)
                        .If(
                            i.IsLast,
                            Then: ctxt => {
                                ctxt.WriteLine();
                            },
                            Else: ctxt => {
                                ctxt.WriteLine(",");
                            });

                    })
                .IndentDecr().Write(")")
                .If(value.Code is null,
                    Then: ctxt => ctxt.Write(";").WriteLine(),
                    Else: ctxt => ctxt
                        .Write(" {").WriteLine().IndentIncr()
                        .CallTemplateDynamic(value.Code)
                        .IndentDecr().Write("}").WriteLine()
                        .WriteLine()
                    );
        }
    }

}