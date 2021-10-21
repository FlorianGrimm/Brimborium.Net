using Brimborium.CodeBlocks.Library;
using Brimborium.CodeBlocks.Tool;

using Microsoft.Extensions.Logging;

using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;

namespace Demo.CodeGen {
    public class CodeGenTaskIServerAPI : ICodeGenTask {
        private readonly ToolService _ToolService;
        private readonly CBTemplateProvider _TemplateProvider;
        private readonly ILogger<CodeGenTaskIServerAPI> _Logger;

        public CodeGenTaskIServerAPI(
            ToolService toolService,
            CBTemplateProvider templateProvider,
            ILogger<CodeGenTaskIServerAPI> logger
            ) {
            this._ToolService = toolService;
            this._TemplateProvider = templateProvider.GetTemplateByLanguage(CBTemplateProvider.CSharp);
            this._Logger = logger;
        }

        public int GetStep() => 100;

        public void Execute() {
            //var lstIServerAPI = typeof(Demo.Server.IEbbesServerAPI).Assembly.GetTypes().Where(t => typeof(IServerAPI).IsAssignableFrom(t)).ToList();
            //this._Logger.LogDebug("ServerAPI : {ServerAPIs}", string.Join(", ", lstIServerAPI.Select(t => t.FullName)));
            //foreach (var typeIServerAPI in lstIServerAPI) {
            //    this.ScanServerAPI(CBCodeClrTypeReference.CreateFrom(typeIServerAPI));
            //}
            var lstTypeIController = this.GetType().Assembly.GetTypes().Where(t => t.Namespace == "Demo.Controllers").ToList();
            this._Logger.LogDebug("IController  : {lstTypeIController}", string.Join(", ", lstTypeIController.Select(t => t.FullName)));
            foreach (var typeIController in lstTypeIController) {
                this.ScanServerAPI(CBCodeType.FromClr(typeIController));
            }
        }

        private void ScanServerAPI(CBCodeType typeIController) {
            var controllerInfo = SrcIControllerInfo.Create(typeIController);
            var genIServerAPIInfo = GenIServerAPIInfo.Create(controllerInfo);
            var codeFile = genIServerAPIInfo.CodeFile;
            codeFile.FileName = $@"Demo.Abstracts\Server\{genIServerAPIInfo.TypeName}.txt";
            var codeInterface = genIServerAPIInfo.CodeInterface;

            foreach (var ns in new string[]{
                "System",
                "System.Collections.Generic",
                "System.Linq",
                "System.Threading",
                "System.Threading.Tasks"
            }) {
                codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace(ns)));
            }
            codeFile.Imports.Add(new CBCodeImportNamespace(codeInterface.Namespace));

            codeFile.Generate(this._ToolService, this._TemplateProvider, CBTemplateProvider.Declaration);

            /*
            var typeServerAPI = new CBCodeClass(typeIServerAPI.Namespace, typeIServerAPI.TypeName.Substring(1));
            typeServerAPI.Interfaces.Add(typeIServerAPI);
            typeServerAPI.Attributes.Add(new CBCodeTypeAttribute(CBCodeClrTypeReference.Create<Brimborium.Registrator.ScopedAttribute>()));
            var ns = typeServerAPI.Namespace.GetParentNamespace();

            var codeFile = new CBCodeFile();
            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("Brimborium.CodeFlow.RequestHandler")));
            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("Brimborium.CodeFlow.Server")));
            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("Brimborium.Registrator")));

            codeFile.Imports.Add(new CBCodeImportNamespace(ns.GetSubNamespace("API")));
            codeFile.Imports.Add(new CBCodeImportNamespace(ns.GetSubNamespace("Logic")));

            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("Microsoft.Extensions.DependencyInjection")));

            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("System")));
            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("System.Collections.Generic")));
            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("System.Linq")));
            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("System.Threading")));
            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("System.Threading.Tasks")));

            codeFile.Namespace = typeIServerAPI.Namespace;
            codeFile.Items.Add(typeServerAPI);
            var ctor = new CBCodeDefinitionConstructor();
            typeServerAPI.Members.Add(ctor);
            ctor.AddParameterAssignToField("requestServices", CBCodeClrTypeReference.Create<System.IServiceProvider>());

            foreach (var memberInfo in typeIServerAPI.GetMembers()) {
                if (memberInfo is CBCodeClrMethodInfo methodInfo) {
                    var serverAPIMethod = new GenIServerAPIMethodInfo();
                    serverAPIMethod.Name = memberInfo.Name;
                    if (methodInfo.Parameters.Length == 2) {
                        serverAPIMethod.ServerRequest = methodInfo.Parameters[0].ParameterType;
                    }

                    if (methodInfo.ReturnType.TryGetGenericTypeArguments(typeof(System.Threading.Tasks.Task<>), out var taskOfArgs)) {
                        if (taskOfArgs[0].TryGetGenericTypeArguments(typeof(Brimborium.CodeFlow.RequestHandler.RequestResult<>), out var responseArgs)) {
                            serverAPIMethod.ServerResponse = responseArgs[0];
                        }
                    }
                    serverAPIMethod.Request = serverAPIMethod.ServerRequest;
                    serverAPIMethod.Response = serverAPIMethod.ServerResponse;
                    typeServerAPI.Members.Add(serverAPIMethod);
                }
            }
            */

            /*
            typeServerAPI.Members.Add(new ServerAPIMethod() {
                Request = new CBCodeTypeNameReference(new CBCodeTypeName(new CBCodeNamespace(""), "GnaQueryRequest")),
                ServerRequest = new CBCodeTypeNameReference(new CBCodeTypeName(new CBCodeNamespace(""), "GnaServerGetRequest")),
                Response = new CBCodeTypeNameReference(new CBCodeTypeName(new CBCodeNamespace(""), "GnaQueryResponse")),
                ServerResponse = new CBCodeTypeNameReference(new CBCodeTypeName(new CBCodeNamespace(""), "GnaServerGetResponse")),
                RequestHandler = new CBCodeTypeNameReference(new CBCodeTypeName(new CBCodeNamespace(""), "IGnaQueryRequestHandler")),
            });
            */

            //var sbOutput = new StringBuilder();
            //var writer = new IndentedTextWriter(new StringWriter(sbOutput), "    ");
            //CBRenderContext ctxt = new CBRenderContext(this._TemplateProvider, writer);
            //ctxt.CallTemplateDynamic(codeFile);
            //this._ToolService.SetFileContent(new CBFileContent($@"Demo.Logic\Server\{typeServerAPI.TypeName}.txt", sbOutput.ToString()));
        }
    }
 
}

#if false
namespace Demo.CodeGen {
    public class CodeGenTaskServerAPI /*: ICodeGenTask*/ {
        private readonly ToolService _ToolService;
        private readonly CBTemplateProvider _TemplateProvider;
        private readonly ILogger<CodeGenTaskServerAPI> _Logger;

        public CodeGenTaskServerAPI(
            ToolService toolService,
            CBTemplateProvider templateProvider,
            ILogger<CodeGenTaskServerAPI> logger
            ) {
            this._ToolService = toolService;
            this._TemplateProvider = templateProvider.GetTemplateByLanguage(CBTemplateProvider.CSharp);
            this._Logger = logger;
        }

        public int GetStep() => 100;

        public void Execute() {
            var lstIServerAPI = typeof(Demo.Server.IEbbesServerAPI).Assembly.GetTypes().Where(t => typeof(IServerAPI).IsAssignableFrom(t)).ToList();
            this._Logger.LogDebug("ServerAPI : {ServerAPIs}", string.Join(", ", lstIServerAPI.Select(t => t.FullName)));
            foreach (var typeIServerAPI in lstIServerAPI) {
                this.ScanServerAPI(CBCodeClrTypeReference.CreateFrom(typeIServerAPI));
            }
        }

        private void ScanServerAPI(CBCodeClrTypeReference typeIServerAPI) {
            if (!typeIServerAPI.TypeName.StartsWith("I")) {
                throw new System.ArgumentException($"{typeIServerAPI.TypeName} must start with an I.");
            }


            var typeServerAPI = new CBCodeClass(typeIServerAPI.Namespace, typeIServerAPI.TypeName.Substring(1));
            typeServerAPI.Interfaces.Add(typeIServerAPI);
            typeServerAPI.Attributes.Add(new CBCodeTypeAttribute(CBCodeClrTypeReference.Create<Brimborium.Registrator.ScopedAttribute>()));
            var ns = typeServerAPI.Namespace.GetParentNamespace();

            var codeFile = new CBCodeFile();
            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("Brimborium.CodeFlow.RequestHandler")));
            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("Brimborium.CodeFlow.Server")));
            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("Brimborium.Registrator")));

            codeFile.Imports.Add(new CBCodeImportNamespace(ns.GetSubNamespace("API")));
            codeFile.Imports.Add(new CBCodeImportNamespace(ns.GetSubNamespace("Logic")));

            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("Microsoft.Extensions.DependencyInjection")));

            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("System")));
            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("System.Collections.Generic")));
            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("System.Linq")));
            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("System.Threading")));
            codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace("System.Threading.Tasks")));

            codeFile.Namespace = typeIServerAPI.Namespace;
            codeFile.Items.Add(typeServerAPI);
            var ctor = new CBCodeDefinitionConstructor();
            typeServerAPI.Members.Add(ctor);
             ctor.AddParameterAssignToField("requestServices", CBCodeClrTypeReference.Create<System.IServiceProvider>());

            foreach (var memberInfo in typeIServerAPI.GetMembers()) {
                if (memberInfo is CBCodeClrMethodInfo methodInfo) {
                    var serverAPIMethod =new ServerAPIMethod();
                    serverAPIMethod.Name = memberInfo.Name;
                    if (methodInfo.Parameters.Length == 2) {
                        serverAPIMethod.ServerRequest = methodInfo.Parameters[0].ParameterType;
                    }
                    
                    if (methodInfo.ReturnType.TryGetGenericTypeArguments(typeof(System.Threading.Tasks.Task<>), out var taskOfArgs)) {
                        if (taskOfArgs[0].TryGetGenericTypeArguments(typeof(Brimborium.CodeFlow.RequestHandler.RequestResult<>), out var responseArgs)) {
                            serverAPIMethod.ServerResponse = responseArgs[0];
                        }
                    }
                    serverAPIMethod.Request = serverAPIMethod.ServerRequest;
                    serverAPIMethod.Response = serverAPIMethod.ServerResponse;
                    typeServerAPI.Members.Add(serverAPIMethod);
                }
            }

            /*
            typeServerAPI.Members.Add(new ServerAPIMethod() {
                Request = new CBCodeTypeNameReference(new CBCodeTypeName(new CBCodeNamespace(""), "GnaQueryRequest")),
                ServerRequest = new CBCodeTypeNameReference(new CBCodeTypeName(new CBCodeNamespace(""), "GnaServerGetRequest")),
                Response = new CBCodeTypeNameReference(new CBCodeTypeName(new CBCodeNamespace(""), "GnaQueryResponse")),
                ServerResponse = new CBCodeTypeNameReference(new CBCodeTypeName(new CBCodeNamespace(""), "GnaServerGetResponse")),
                RequestHandler = new CBCodeTypeNameReference(new CBCodeTypeName(new CBCodeNamespace(""), "IGnaQueryRequestHandler")),
            });
            */

            var sbOutput = new StringBuilder();
            var writer = new IndentedTextWriter(new StringWriter(sbOutput), "    ");
            CBRenderContext ctxt = new CBRenderContext(this._TemplateProvider, writer);
            ctxt.CallTemplateDynamic(codeFile);
            this._ToolService.SetFileContent(new CBFileContent($@"Demo.Logic\Server\{typeServerAPI.TypeName}.txt", sbOutput.ToString()));
        }
    }

    public sealed class ServerAPIMethod : CBCodeDefinitionCustomMember {
        public ServerAPIMethod() {
        }
        public ICBCodeTypeReference? ServerRequest { get; set; }
        public ICBCodeTypeReference? Request { get; set; }
        public ICBCodeTypeReference? Response { get; set; }
        public ICBCodeTypeReference? ServerResponse { get; set; }
        public ICBCodeTypeReference? RequestHandler { get; set; }


    }
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
    }
}
#endif
