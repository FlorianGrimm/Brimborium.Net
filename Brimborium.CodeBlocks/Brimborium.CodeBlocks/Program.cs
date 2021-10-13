using System.Threading.Tasks;

namespace Brimborium.CodeBlocks {
    public static class Program {
        public static async Task<int> Main(string[] args) {
            return await Brimborium.CodeBlocks.Tool.ToolProgram.Run(args);
        }
    }
}