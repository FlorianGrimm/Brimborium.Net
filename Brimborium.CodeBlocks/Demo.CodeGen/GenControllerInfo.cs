using Brimborium.CodeBlocks.Library;

using System.Collections.Generic;
using System.Linq;

namespace Demo.CodeGen {
    public sealed class GenControllerInfo {
        public GenControllerInfo(SrcIControllerInfo controllerInfo) {
            this.SrcInfo = controllerInfo;
            this.Namespace = controllerInfo.TypeIController.Namespace.GetParentNamespace().GetSubNamespace("Server");
            this.TypeName = $"{controllerInfo.ShortName}Controller";
            this.TypeNameIServerAPI = $"I{controllerInfo.ShortName}ServerAPI";
            this.Methods = new List<ControllerMethodInfo>();
            this.CodeFile = new CBCodeFile() {
                Namespace = this.Namespace,
            };
            this.CodeClass = new CBCodeType() {
                Namespace = this.Namespace,
                Name = this.TypeName,
                IsInterface = true,
                AccessibilityLevel = CBCodeAccessibilityLevel.Public,
            };
            this.CodeFile.Items.Add(this.CodeClass);
        }

        public SrcIControllerInfo SrcInfo { get; }

        public CBCodeNamespace Namespace { get; }

        public string TypeName { get; }
        public string TypeNameIServerAPI { get; }

        public CBCodeFile CodeFile { get; }
        public CBCodeType CodeClass { get; }
        public List<ControllerMethodInfo> Methods { get; }

        public static GenControllerInfo Create(SrcIControllerInfo controllerInfo, GenIServerAPIInfo genIServerAPIInfo) {
            var result = new GenControllerInfo(controllerInfo);
            result.Methods.AddRange(
                genIServerAPIInfo.Methods.Select(method => ControllerMethodInfo.Create(result, method.SourceMethod, method))
                );
            return result;
        }
    }

    public sealed class ControllerMethodInfo : CBCodeMethod {

        public static ControllerMethodInfo Create(
                GenControllerInfo genControllerInfo,
                SrcIControllerMethodInfo sourceMethod,
                GenIServerAPIMethodInfo iServerMethod) {
            var result = new ControllerMethodInfo(genControllerInfo, sourceMethod);
            result.AccessibilityLevel = CBCodeAccessibilityLevel.Public;
            result.IsAsync = true;
            result.Name = sourceMethod.Name;
            result.ReturnType = sourceMethod.ReturnType;
            foreach (var parameter in sourceMethod.Parameters) {
                result.Parameters.Add(parameter);
            }
            result.ServerRequestType = iServerMethod.RequestType;
            result.ServerResposeType = iServerMethod.ResposeType;
            result.InnerReturnType = iServerMethod.InnerReturnType;
            return result;
        }
        public ControllerMethodInfo(
                GenControllerInfo genControllerInfo,
                SrcIControllerMethodInfo sourceMethod
            ) {
            this.GenControllerInfo = genControllerInfo;
            this.SourceMethod = sourceMethod;
            this.ServerRequestType = this.ServerResposeType = CBCodeType.FromType<object>();
        }

        public GenControllerInfo GenControllerInfo { get; }
        public SrcIControllerMethodInfo SourceMethod { get; }
        public CBCodeType ServerRequestType;
        public CBCodeType ServerResposeType;
        public CBCodeType? InnerReturnType;
    }

    public sealed class CBTemplateCSharpControllerMethodInfo : CBNamedTemplate<ControllerMethodInfo> {
        public CBTemplateCSharpControllerMethodInfo()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Declaration) {
        }

        public override void RenderT(ControllerMethodInfo value, CBRenderContext ctxt) {
            ctxt.CallTemplateStrict(value.AccessibilityLevel);
            ctxt.Write("async ");
            ctxt.CallTemplateDynamic(value.ReturnType, CBTemplateProvider.TypeName).Write(" ")
                .Write(value.Name).Write("(").IndentIncr().WriteLine()
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
                .Write(") {").WriteLine();
            {

                // var typeRequestResultOfRequestType = CBCodeType.FromClr(typeof(Brimborium.CodeFlow.RequestHandler.RequestResult<>)).WithGenericTypeArguments(value.ServerRequestType);
                var typeResponseResultOfRequestType = CBCodeType.FromClr(typeof(Brimborium.CodeFlow.RequestHandler.RequestResult<>)).WithGenericTypeArguments(value.ServerResposeType);

                if (value.SourceMethod.ReturnType!.TryGetSingleGenericTypeArguments(CBCodeType.FromClr(typeof(System.Threading.Tasks.Task<>)), out var typeActionResultOf)) {
                } else {
                    typeActionResultOf = CBCodeType.FromClr(typeof(Microsoft.AspNetCore.Mvc.ActionResult));
                }
                var innerReturnType = typeActionResultOf.GenericTypeArguments.FirstOrDefault();


                ctxt.Write("var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? this.HttpContext?.TraceIdentifier ?? \"\";").WriteLine();

                ctxt.Write("try {").WriteLine(+1);
                {
                    ctxt.Write("this._Logger.LogInformation(\"").Write(value.Name).Write("({parameter}) {traceId}\", new { ")
                         .Foreach(value.Parameters, (i, ctxt) => {
                             ctxt
                             .If(i.IsFirst, Then: ctxt => { }, Else: ctxt => { ctxt.Write(", "); })
                             .Write(i.Value.Name);
                         })
                        .Write(" }, traceId);").WriteLine()
                        .Write(value.GenControllerInfo.TypeNameIServerAPI).Write(" service = this._RequestServices.GetRequiredService<").Write(value.GenControllerInfo.TypeNameIServerAPI).Write(">();").WriteLine()
                        .Write("System.Security.Claims.ClaimsPrincipal user = this.HttpContext?.User ?? new System.Security.Claims.ClaimsPrincipal();").WriteLine()
                        .Write("CancellationToken requestAborted = this.HttpContext?.RequestAborted ?? CancellationToken.None;").WriteLine()

                        .Write("var request = new ").CallTemplateDynamic(value.ServerRequestType, CBTemplateProvider.TypeName).Write("(").IndentIncr()
                        .Foreach(value.Parameters, (i, ctxt) => {
                            ctxt
                            .If(i.IsFirst, Then: ctxt => { ctxt.WriteLine(); }, Else: ctxt => { ctxt.Write(",").WriteLine(); })
                            .Write(i.Value.Name);
                        })
                        .Write(",").WriteLine().Write("user").WriteLine()
                        .IndentDecr().Write(");").WriteLine()
                        .CallTemplateDynamic(typeResponseResultOfRequestType, CBTemplateProvider.TypeName).Write("response = await service.").Write(value.Name).Write("(request, requestAborted);").WriteLine();

                    if (innerReturnType is not null) {
                        ctxt.CallTemplateDynamic(typeActionResultOf, CBTemplateProvider.TypeName).Write(" actionResult = this._RequestServices.GetRequiredService<IActionResultConverter>()").WriteLine()
                            .Write("    .ConvertToActionResultOfT<").CallTemplateDynamic(value.ServerResposeType, CBTemplateProvider.TypeName).Write(", ").CallTemplateDynamic(innerReturnType, CBTemplateProvider.TypeName).Write(">(this, response);").WriteLine();
                    } else {
                        ctxt.CallTemplateDynamic(typeActionResultOf, CBTemplateProvider.TypeName).Write(" actionResult = this._RequestServices.GetRequiredService<IActionResultConverter>()").WriteLine()
                            .Write("    .ConvertToActionResultVoid<").CallTemplateDynamic(value.ServerResposeType, CBTemplateProvider.TypeName).Write(">(this, response);").WriteLine();
                    }


                    ctxt.Write("return actionResult;").WriteLine(-1);
                }
                ctxt.Write("} catch (System.Exception error) {").WriteLine(+1);
                {
                    ctxt.Write("this._Logger.LogError(error, \"").Write(value.Name).Write("({parameter}) {traceId}\", new { ")
                         .Foreach(value.Parameters, (i, ctxt) => {
                             ctxt
                             .If(i.IsFirst, Then: ctxt => { }, Else: ctxt => { ctxt.Write(", "); })
                             .Write(i.Value.Name);
                         })
                        .Write(" }, traceId);").WriteLine()
                        .Write("throw;").WriteLine(-1)
                        .Write("}").WriteLine(-1);
                }
                ctxt.Write("}").WriteLine();
            }
        }
    }

}
