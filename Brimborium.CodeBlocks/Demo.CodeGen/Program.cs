using System.Threading.Tasks;

namespace Demo.CodeGen {
    public static class Program {
        public static async Task<int> Main(string[] args) {
            return await Brimborium.CodeBlocks.Inplace.InplaceProgram.Run(args, typeof(Program).Assembly);
        }
    }
}
