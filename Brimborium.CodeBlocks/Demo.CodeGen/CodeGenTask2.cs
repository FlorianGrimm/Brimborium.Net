
using Brimborium.CodeBlocks.Tool;

using Microsoft.Extensions.Logging;

namespace Demo.CodeGen {
    public class CodeGenTask2 : ICodeGenTask {
        private readonly ToolService _ToolService;
        private readonly ILogger<CodeGenTask2> _Logger;

        public CodeGenTask2(
            ToolService toolService,
            ILogger<CodeGenTask2> logger
            ) {
            this._ToolService = toolService;
            this._Logger = logger;
        }

        public int GetStep() => 200;

        public void Execute() {
            this._Logger.LogDebug("2D");
            this._Logger.LogInformation("2I");
        }
    }
}
