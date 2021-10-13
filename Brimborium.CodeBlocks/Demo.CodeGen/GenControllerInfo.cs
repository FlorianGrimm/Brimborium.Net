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
        }
        public SrcIControllerInfo SrcInfo { get; }

        public CBCodeNamespace Namespace { get; }
        public string TypeName { get; }

        // public CBCodeType TypeNameIServerAPI { get; }
        public string TypeNameIServerAPI { get; }

        public List<ControllerMethodInfo> Methods { get; }

        public static GenControllerInfo Create(SrcIControllerInfo controllerInfo) {
            var result = new GenControllerInfo(controllerInfo);
            result.Methods.AddRange(
                controllerInfo.Methods.Select(method => new ControllerMethodInfo(result, method))
                );
            return result;
        }
    }

    public sealed class ControllerMethodInfo : CBCodeMethod {
        public ControllerMethodInfo(
            GenControllerInfo genControllerInfo,
            SrcIControllerMethodInfo sourceMethod) {
            this.GenControllerInfo = genControllerInfo;
            this.SourceMethod = sourceMethod;
            this.AccessibilityLevel = CBCodeAccessibilityLevel.Public;
            this.Name = sourceMethod.Name;
            this.ReturnType = sourceMethod.ReturnType;
            foreach (var parameter in sourceMethod.Parameters) {
                this.Parameters.Add(parameter);
            }
        }

        public GenControllerInfo GenControllerInfo { get; }
        public SrcIControllerMethodInfo SourceMethod { get; }
    }

    public sealed class CBTemplateCSharpControllerMethodInfo : CBNamedTemplate<ControllerMethodInfo> {
        public CBTemplateCSharpControllerMethodInfo()
            : base(CBTemplateProvider.CSharp, CBTemplateProvider.Declaration) {
        }

        public override void RenderT(ControllerMethodInfo value, CBRenderContext ctxt) {
            ctxt.CallTemplateStrict(value.AccessibilityLevel);
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

            ctxt.Write("var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? this.HttpContext?.TraceIdentifier ?? \"\";").WriteLine()
                .Write("try {").WriteLine(+1)
                .Write("this._Logger.LogInformation(\"").Write(value.Name).Write("({parameter}) {traceId}\", new { pattern }, traceId);").WriteLine()
                .Write(value.GenControllerInfo.TypeNameIServerAPI).Write(" service = this._RequestServices.GetRequiredService<").Write(value.GenControllerInfo.TypeNameIServerAPI).Write(">();").WriteLine()
                .Write("System.Security.Claims.ClaimsPrincipal user = this.HttpContext?.User ?? new System.Security.Claims.ClaimsPrincipal();").WriteLine()
                .Write("CancellationToken requestAborted = this.HttpContext?.RequestAborted ?? CancellationToken.None;").WriteLine()
                .Write("var request = new GnaServerGetRequest(pattern, user);").WriteLine()
                .Write("RequestResult<GnaServerGetResponse> response = await service.GetAsync(request, requestAborted);").WriteLine()
                .Write("ActionResult<IEnumerable<Gna>> actionResult = this._RequestServices.GetRequiredService<IActionResultConverter>()").WriteLine()
                .Write("    .ConvertToActionResultOfT<GnaServerGetResponse, IEnumerable<Gna>>(this, response);").WriteLine()
                .Write("return actionResult;").WriteLine(-1)
                .Write("} catch (System.Exception error) {").WriteLine(+1)
                .Write("this._Logger.LogError(error, \"").Write(value.Name).Write("({parameter}) {traceId}\", new { pattern }, traceId);").WriteLine()
                .Write("throw;").WriteLine(-1)
                .Write("}").WriteLine(-1)
                .Write("}").WriteLine()
                .WriteLine();
        }
    }

}
