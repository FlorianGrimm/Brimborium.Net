using Brimborium.CodeBlocks.Library;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Demo.CodeGen {
    public sealed class GenIServerAPIInfo {
        public GenIServerAPIInfo(SrcIControllerInfo controllerInfo) {
            this.SrcInfo = controllerInfo;
            this.Namespace = controllerInfo.TypeIController.Namespace.GetParentNamespace().GetSubNamespace("Server");
            this.TypeName = $"I{controllerInfo.ShortName}ServerAPI";
            this.Methods = new List<ControllerMethodInfo>();
        }

        public SrcIControllerInfo SrcInfo { get; }

        public CBCodeNamespace Namespace { get; }
        public string TypeName { get; }

        public List<ControllerMethodInfo> Methods { get; }

        public static GenIServerAPIInfo Create(SrcIControllerInfo controllerInfo) {
            var result = new GenIServerAPIInfo(controllerInfo);
            return result;
        }
    }

    public sealed class GenIServerAPIMethodInfo : CBCodeDefinitionCustomMember {
        public GenIServerAPIMethodInfo() {
        }
        //public ICBCodeTypeReference? ServerRequest { get; set; }
        //public ICBCodeTypeReference? Request { get; set; }
        //public ICBCodeTypeReference? Response { get; set; }
        //public ICBCodeTypeReference? ServerResponse { get; set; }
        //public ICBCodeTypeReference? RequestHandler { get; set; }


    }
    /*
    public sealed class CBTemplateCSharpServerAPIMethod : CBNamedTemplate<ServerAPIMethod> {
        public CBTemplateCSharpServerAPIMethod()
            : base(CBTemplateProvider.CSharp, string.Empty) {
        }

        public override void RenderT(ServerAPIMethod value, CBRenderContext ctxt) {

            ctxt.Write("public async Task<RequestResult<").CallTemplateDynamic(value.ServerResponse).Write(">> ").Write(value.Name ?? "").Write("(").CallTemplateDynamic(value.ServerRequest).Write(" request, CancellationToken cancellationToken) {").WriteLine(indent: +1);
            {
                ctxt.Write("IGlobalRequestHandlerFactory globalRequestHandlerFactory = this._RequestServices.GetRequiredService<IGlobalRequestHandlerFactory>();").WriteLine();
                ctxt.CallTemplateDynamic(value.RequestHandler).Write(" requestHandler = globalRequestHandlerFactory.CreateRequestHandler<").CallTemplateDynamic(value.RequestHandler).Write(">(this._RequestServices);").WriteLine();
                ctxt.Write("request.Deconstruct(").Write("out var Pattern, out var User").Write(");").WriteLine();
                ctxt.CallTemplateDynamic(value.Request).Write(" logicRequest = new GnaQueryRequest(Pattern ?? string.Empty, User);").WriteLine();
                ctxt.Write("RequestResult<").CallTemplateDynamic(value.Response).Write("> logicResponse = await requestHandler.ExecuteAsync(logicRequest, cancellationToken);").WriteLine();
                ctxt.Write("IServerRequestResultConverter serverRequestResultConverter = this._RequestServices.GetRequiredService<IServerRequestResultConverter>();").WriteLine();
                ctxt.Write("return serverRequestResultConverter.ConvertToServerResultOfT<").CallTemplateDynamic(value.Response).Write(", ").CallTemplateDynamic(value.ServerResponse).Write(">(logicResponse);").WriteLine(indent: -1);
            }
            ctxt.Write("}").WriteLine();
        }
    }*/
}
