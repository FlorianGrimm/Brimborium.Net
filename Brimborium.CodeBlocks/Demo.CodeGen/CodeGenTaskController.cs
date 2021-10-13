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
                this.ExecuteIController(CBCodeType.FromClr(typeIController));
            }
        }

        public void ExecuteIController(CBCodeType typeIController) {
            var controllerInfo = SrcIControllerInfo.Create(typeIController);
            var genControllerInfo = GenControllerInfo.Create(controllerInfo);

            var (codeFile, codeClass) = CBCodeFile.CreateFileAndType(
                    genControllerInfo.Namespace,
                    genControllerInfo.TypeName,
                    $@"Demo.WebApplication\Controllers\{genControllerInfo.TypeName}.txt"
                );
            

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

            codeClass.IsClass = true;
            codeClass.IsSealed = true;
            codeClass.Prefix.Add(new CBCodeConst($"// {typeIController}"));
            codeClass.Prefix.Add(new CBCodeConst("[ApiController]"));
            codeClass.Prefix.Add(new CBCodeConst("[Route(\"api/[controller]\")]"));
            codeClass.BaseType = CBCodeType.FromClr(typeof(Microsoft.AspNetCore.Mvc.ControllerBase));

            //codeClass.Attributes.Add(new CBCodeTypeAttribute(CBCodeClrTypeReference.Create<Brimborium.Registrator.ScopedAttribute>()));

            var ctor = new CBCodeConstructor();
            codeClass.Constructors.Add(ctor);
            
            ctor.AddParameterAssignToField("requestServices", CBCodeType.FromClr(typeof(System.IServiceProvider)));

            ctor.AddParameterAssignToField("logger",
                new CBCodeType(
                    genericTypeDefinition: CBCodeType.FromClr(typeof(Microsoft.Extensions.Logging.ILogger<>)),
                    genericTypeArguments: new CBCodeType[] { codeClass }));

            codeFile.Generate(this._ToolService, this._TemplateProvider);
        }
    }
}

/*
Brimborium.CodeFlow.RequestHandler.RequestResult
*/
