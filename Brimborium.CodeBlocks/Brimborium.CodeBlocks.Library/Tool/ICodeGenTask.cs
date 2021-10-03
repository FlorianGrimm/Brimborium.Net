namespace Brimborium.CodeBlocks.Tool {
    public interface ICodeGenTask {
        int GetStep();

        void Execute();
    }
}
