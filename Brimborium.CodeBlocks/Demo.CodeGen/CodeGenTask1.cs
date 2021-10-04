using Brimborium.CodeBlocks.Tool;
using System.Linq;
using Microsoft.Extensions.Logging;
using Brimborium.CodeFlow.Server;
using Brimborium.CodeBlocks.Library;

namespace Demo.CodeGen {
    public class CodeGenTask1 : ICodeGenTask {
        private readonly ToolService _ToolService;
        private readonly ILogger<CodeGenTask1> _Logger;

        public CodeGenTask1(
            ToolService toolService,
            ILogger<CodeGenTask1> logger
            ) {
            this._ToolService = toolService;
            this._Logger = logger;
        }

        public int GetStep() => 100;

        public void Execute() {
            var lstServerAPI = typeof(Demo.Server.IEbbesServerAPI).Assembly.GetTypes().Where(t => typeof(IServerAPI).IsAssignableFrom(t)).ToList();
            this._Logger.LogDebug("ServerAPI : {ServerAPIs}", string.Join(", ", lstServerAPI.Select(t=>t.FullName)));
            var b=lstServerAPI.Select(typeServerAPI => {
                //typeServerAPI.GetMethods
                return typeServerAPI;
            }).ToList();
            this._ToolService.SetFileContent(new CBFileContent($@"Demo.Logic\Server\GnaServerAPI.cs", "//GnaServerAPI"));
        }
    }
}
