namespace Brimborium.CodeBlocks.Tool {
    public interface ICodeGenTask {
        int GetStepOrder();

        void Execute();
    }
}
