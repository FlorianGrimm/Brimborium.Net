using Brimborium.CodeBlocks.Library;

using System.Collections.Generic;
using System.Linq;

namespace Demo.CodeGen {
    public sealed class GenIServerAPIInfo {
        public GenIServerAPIInfo(SrcIControllerInfo controllerInfo) {
            this.SrcInfo = controllerInfo;
            this.Namespace = controllerInfo.TypeIController.Namespace.GetParentNamespace().GetSubNamespace("Server");
            this.TypeName = $"I{controllerInfo.ShortName}ServerAPI";
            this.PrefixTypeName = $"{controllerInfo.ShortName}Server";
            this.Methods = new List<GenIServerAPIMethodInfo>();
            this.CodeFile = new CBCodeFile();
            this.CodeInterface = new CBCodeType();
            this.CodeFile.Items.Add(this.CodeInterface);

        }

        public SrcIControllerInfo SrcInfo { get; }

        public CBCodeNamespace Namespace { get; }

        public string TypeName { get; }
        public string PrefixTypeName { get; }
        public List<GenIServerAPIMethodInfo> Methods { get; }

        public CBCodeFile CodeFile { get; }

        public CBCodeType CodeInterface { get; }

        private void BuildCodeInterface() {
            this.CodeFile.Namespace = this.Namespace;
            this.CodeInterface.Namespace = this.CodeFile.Namespace;
            this.CodeInterface.Name = this.TypeName;
            this.CodeInterface.IsInterface = true;
            this.CodeInterface.AccessibilityLevel = CBCodeAccessibilityLevel.Public;
            foreach (var method in this.Methods) {
                this.CodeInterface.Methods.Add(method);
                this.CodeFile.Items.Add(method.RequestType);
                this.CodeFile.Items.Add(method.ResposeType);
            }
        }

        public static GenIServerAPIInfo Create(SrcIControllerInfo controllerInfo) {
            var result = new GenIServerAPIInfo(controllerInfo);
            result.Methods.AddRange(
                controllerInfo.Methods.Select(method => GenIServerAPIMethodInfo.Create(result, method))
                );
            result.BuildCodeInterface();
            return result;
        }
    }

    public sealed class GenIServerAPIMethodInfo : CBCodeMethod {

        public static GenIServerAPIMethodInfo Create(
            GenIServerAPIInfo genIServerAPIInfo,
            SrcIControllerMethodInfo sourceMethod
            ) {
            var typeTask = CBCodeType.FromClr(typeof(System.Threading.Tasks.Task<>));
            var typeActionResult = CBCodeType.FromClr(typeof(Microsoft.AspNetCore.Mvc.ActionResult<>));
            var typeIEnumerable = CBCodeType.FromClr(typeof(IEnumerable<>));
            var typeActionRequestResult = CBCodeType.FromClr(typeof(Brimborium.CodeFlow.RequestHandler.RequestResult<>));
            var typeIServerRequest = CBCodeType.FromClr(typeof(Brimborium.CodeFlow.Logic.IServerRequest));
            var typeIServerResponseOfT = CBCodeType.FromClr(typeof(Brimborium.CodeFlow.Logic.IServerResponse<>));
            var typeResultVoid = CBCodeType.FromClr(typeof(Brimborium.CodeFlow.RequestHandler.ResultVoid));
            var result = new GenIServerAPIMethodInfo(genIServerAPIInfo, sourceMethod);

            result.Name = sourceMethod.Name;

            result.RequestType = new CBCodeType() {
                AccessibilityLevel = CBCodeAccessibilityLevel.Public,
                IsRecord = true,
                Namespace = genIServerAPIInfo.Namespace,
                Name = $"{genIServerAPIInfo.PrefixTypeName}{sourceMethod.Name}Request"
            };

            result.ResposeType = new CBCodeType() {
                AccessibilityLevel = CBCodeAccessibilityLevel.Public,
                IsRecord = true,
                Namespace = genIServerAPIInfo.Namespace,
                Name = $"{genIServerAPIInfo.PrefixTypeName}{sourceMethod.Name}Respose"
            };

            result.ReturnType = typeTask.WithGenericTypeArguments(typeActionRequestResult.WithGenericTypeArguments(result.ResposeType));

            result.Parameters.Add(new CBCodeParameter() {
                Name = "request",
                Type = result.RequestType
            });
            result.Parameters.Add(new CBCodeParameter() {
                Name = "cancellationToken",
                Type = CBCodeType.FromType<System.Threading.CancellationToken>()
            });

            result.RequestType.Interfaces.Add(typeIServerRequest);
            foreach (var parameter in sourceMethod.Parameters) {
                result.RequestType.RecordParameters.Add(new CBCodeParameter() {
                    Name = StringUtility.FirstUpperCase(parameter.Name),
                    Type = parameter.Type
                });
            }
            result.RequestType.RecordParameters.Add(new CBCodeParameter("User", CBCodeType.FromType<System.Security.Claims.ClaimsPrincipal>()));

            CBCodeType? innerReturnType = null;
            if (sourceMethod.ReturnType is not null
                && sourceMethod.ReturnType.TryGetSingleGenericTypeArguments(typeTask, out var ar)) {
                if (ar.TryGetSingleGenericTypeArguments(typeActionResult, out var innerActionResultType)) {
                    innerReturnType = innerActionResultType;
                    //if (ar.TryGetSingleGenericTypeArguments(typeIEnumerable, out var innerIEnumerable)) {
                    //    innerReturnType = innerIEnumerable;
                    //} else {
                    //}
                } else if (typeActionResult.HasEqualName(ar)) {
                    innerReturnType = typeResultVoid;
                } else {
                    throw new System.NotImplementedException();
                }
            } else {
                innerReturnType = typeResultVoid;
            }

            if (innerReturnType is null) {
                innerReturnType = typeResultVoid;
            }

            result.InnerReturnType = innerReturnType;

            result.ResposeType.RecordParameters.Add(new CBCodeParameter("Value", innerReturnType));
            result.ResposeType.Interfaces.Add(typeIServerResponseOfT.WithGenericTypeArguments(innerReturnType));

            return result;
        }
        public GenIServerAPIMethodInfo(
            GenIServerAPIInfo genIServerAPIInfo,
            SrcIControllerMethodInfo sourceMethod) : base() {
            this.GenIServerAPIInfo = genIServerAPIInfo;
            this.SourceMethod = sourceMethod;
            this.RequestType = this.ResposeType = CBCodeType.FromType<object>();
        }

        public GenIServerAPIInfo GenIServerAPIInfo { get; }

        public SrcIControllerMethodInfo SourceMethod { get; }

        public CBCodeType ResposeType;
        public CBCodeType RequestType;
        public CBCodeType? InnerReturnType;

        //public ICBCodeTypeReference? ServerRequest { get; set; }
        //public ICBCodeTypeReference? Request { get; set; }
        //public ICBCodeTypeReference? Response { get; set; }
        //public ICBCodeTypeReference? ServerResponse { get; set; }
        //public ICBCodeTypeReference? RequestHandler { get; set; }
    }

    public sealed class CBTemplateCSharpServerAPIMethod : CBNamedTemplate<GenIServerAPIMethodInfo> {
        public CBTemplateCSharpServerAPIMethod()
            : base(CBTemplateProvider.CSharp, string.Empty) {
        }

        public override void RenderT(GenIServerAPIMethodInfo value, CBRenderContext ctxt) {
            ctxt.CallTemplateDynamic(value.ReturnType, CBTemplateProvider.TypeName);
            ctxt.Write(" ");
            ctxt.Write(value.Name).Write("(").IndentIncr()
                .Foreach(
                    value.Parameters,
                    (i, ctxt) => {
                        ctxt
                            .If(i.IsFirst,
                                Then: ctxt => ctxt.WriteLine())
                            .CallTemplateDynamic(i.Value)
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

            //ctxt.Write("public async Task<RequestResult<").CallTemplateDynamic(value.ServerResponse).Write(">> ").Write(value.Name ?? "").Write("(").CallTemplateDynamic(value.ServerRequest).Write(" request, CancellationToken cancellationToken) {").WriteLine(indent: +1);
            //{
            //    ctxt.Write("IGlobalRequestHandlerFactory globalRequestHandlerFactory = this._RequestServices.GetRequiredService<IGlobalRequestHandlerFactory>();").WriteLine();
            //    ctxt.CallTemplateDynamic(value.RequestHandler).Write(" requestHandler = globalRequestHandlerFactory.CreateRequestHandler<").CallTemplateDynamic(value.RequestHandler).Write(">(this._RequestServices);").WriteLine();
            //    ctxt.Write("request.Deconstruct(").Write("out var Pattern, out var User").Write(");").WriteLine();
            //    ctxt.CallTemplateDynamic(value.Request).Write(" logicRequest = new GnaQueryRequest(Pattern ?? string.Empty, User);").WriteLine();
            //    ctxt.Write("RequestResult<").CallTemplateDynamic(value.Response).Write("> logicResponse = await requestHandler.ExecuteAsync(logicRequest, cancellationToken);").WriteLine();
            //    ctxt.Write("IServerRequestResultConverter serverRequestResultConverter = this._RequestServices.GetRequiredService<IServerRequestResultConverter>();").WriteLine();
            //    ctxt.Write("return serverRequestResultConverter.ConvertToServerResultOfT<").CallTemplateDynamic(value.Response).Write(", ").CallTemplateDynamic(value.ServerResponse).Write(">(logicResponse);").WriteLine(indent: -1);
            //}
            //ctxt.Write("}").WriteLine();
        }
    }
}
