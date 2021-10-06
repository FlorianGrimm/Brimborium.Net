using Brimborium.CodeBlocks.Library;
using Brimborium.CodeBlocks.Tool;

using Microsoft.Extensions.Logging;

using System.Linq;

namespace Demo.CodeGen {
    public class CodeGenTaskController : ICodeGenTask {
        private readonly ToolService _ToolService;
        private readonly CBTemplateProvider _TemplateProvider;
        private readonly ILogger<CodeGenTaskController> _Logger;

        public CodeGenTaskController(
            ToolService toolService,
            CBTemplateProvider templateProvider,
            ILogger<CodeGenTaskController> logger
            ) {
            this._ToolService = toolService;
            this._TemplateProvider = templateProvider.GetTemplateByLanguage(CBTemplateProvider.CSharp);
            this._Logger = logger;
        }

        public int GetStep() => 200;

        public void Execute() {
            var lstTypeIController = this.GetType().Assembly.GetTypes().Where(t => t.Namespace == "Brimborium.WebFlow.Controllers").ToList();
            this._Logger.LogDebug("IController  : {lstTypeIController}", string.Join(", ", lstTypeIController.Select(t => t.FullName)));
            foreach (var typeIController in lstTypeIController) {
                this.ExecuteIController(CBCodeClrTypeReference.CreateFrom(typeIController));
            }
        }

        public void ExecuteIController(CBCodeClrTypeReference typeIController) {
            var controllerInfo = SrcIControllerInfo.Create(typeIController);
            var genControllerInfo = GenControllerInfo.Create(controllerInfo);

            var codeFileClass = CBCodeFileClass.Create(
                    genControllerInfo.Namespace,
                    genControllerInfo.TypeName,
                    $@"Demo.WebApplication\Controllers\{genControllerInfo.TypeName}.txt"
                );

            var (codeFile, codeClass) = codeFileClass;
            foreach (var ns in new string[]{
                "System",
                "System.Collections.Generic",
                "System.Linq",
                "System.Threading",
                "System.Threading.Tasks"
            }) {
                codeFile.Imports.Add(new CBCodeImportNamespace(new CBCodeNamespace(ns)));
            }
            codeFile.Imports.Add(new CBCodeImportNamespace(codeClass.Namespace));

            codeClass.Prefix.Add(new CBCodeConst("[ApiController]"));
            codeClass.Prefix.Add(new CBCodeConst("[Route(\"api/[controller]\")]"));



            //codeClass.Attributes.Add(new CBCodeTypeAttribute(CBCodeClrTypeReference.Create<Brimborium.Registrator.ScopedAttribute>()));

            var ctor = new CBCodeDefinitionConstructor();

            //var typeServerAPI = new CBCodeClass(typeIServerAPI.Namespace, typeIServerAPI.TypeName.Substring(1));
            //codeClass.Interfaces.Add(typeIServerAPI);

            codeClass.Members.Add(ctor);
            ctor.AddParameterAssignToField("requestServices", CBCodeClrTypeReference.Create<System.IServiceProvider>());
            ctor.AddParameterAssignToField("logger",
                new CBCodeTypeNameReference(
                    new CBCodeTypeName() {
                        GenericTypeDefinition = CBCodeClrTypeReference.CreateFrom(typeof(Microsoft.Extensions.Logging.ILogger<>)).GetCBCodeTypeNameReference(),
                        GenericTypeArguments = new ICBCodeTypeReference[] {
                            new CBCodeTypeNameReference(codeClass)
                        }
                    }));

            codeFileClass.Generate(this._ToolService, this._TemplateProvider);
        }
    }
}

/*
Brimborium.CodeFlow.RequestHandler.RequestResult
*/
