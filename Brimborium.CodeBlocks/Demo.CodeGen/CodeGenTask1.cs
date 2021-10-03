using Brimborium.CodeBlocks.Tool;

using Microsoft.Extensions.Logging;

namespace Demo.CodeGen {
    public class CodeGenTask1 : ICodeGenTask {
        private readonly ILogger<CodeGenTask1> _Logger;

        public CodeGenTask1(
            ILogger<CodeGenTask1> logger
            ) {
            this._Logger = logger;
        }

        public int GetStep() => 100;

        public void Execute() {
            this._Logger.LogDebug("1D");
            this._Logger.LogInformation("1I");
        }
    }
}
