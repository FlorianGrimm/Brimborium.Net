﻿using Brimborium.CodeBlocks.Library;

using System.Collections.Generic;

namespace Demo.CodeGen {
    public sealed class GenControllerInfo {
        public GenControllerInfo(SrcIControllerInfo controllerInfo) {
            this.SrcInfo = controllerInfo;
            this.Namespace = controllerInfo.TypeIController.Namespace.GetParentNamespace().GetSubNamespace("Server");
            this.TypeName = $"{controllerInfo.ShortName}Controller";
            this.Methods = new List<ControllerMethodInfo>();
        }
        public SrcIControllerInfo SrcInfo { get; }

        public CBCodeNamespace Namespace { get; }
        public string TypeName { get; }

        public List<ControllerMethodInfo> Methods { get; }

        public static GenControllerInfo Create(SrcIControllerInfo controllerInfo) {
            var result = new GenControllerInfo(controllerInfo);
            return result;
        }
    }

    public sealed class ControllerMethodInfo : CBCodeMethod {
        public ControllerMethodInfo(CBCodeMethod sourceMethod) {
            this.SourceMethod = sourceMethod;
            this.AccessibilityLevel = CBCodeAccessibilityLevel.Public;
            this.Name = sourceMethod.Name;
            this.ReturnType = sourceMethod.ReturnType;
            foreach (var parameter in sourceMethod.Parameters) {
                this.Parameters.Add(parameter);
            }
        }

        public CBCodeMethod SourceMethod { get; }
    }

    public sealed class CBTemplateCSharpControllerMethodInfo : CBNamedTemplate<ControllerMethodInfo> {
        public CBTemplateCSharpControllerMethodInfo()
            : base(CBTemplateProvider.CSharp, string.Empty) {
        }

        public override void RenderT(ControllerMethodInfo value, CBRenderContext ctxt) {
            //ctxt.Write(value.Name).WriteLine();
            //CallTemplateDynamic(value.AccessibilityLevel).Write(" ")

            ctxt.CallTemplateDynamic(value.ReturnType).Write(" ")
                .Write(value.Name).Write("(").IndentIncr()
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
                .IndentDecr().Write(");").WriteLine();
        }
    }

}