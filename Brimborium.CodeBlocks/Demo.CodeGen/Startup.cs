using System;

using Brimborium.CodeBlocks.Tool;

namespace Demo.CodeGen {
    public class Startup: IStartup {
        public Startup() {
        }
    }

    public class CodeGenTask1 : ICodeGenTask { }
    public class CodeGenTask2 : ICodeGenTask { }
}
