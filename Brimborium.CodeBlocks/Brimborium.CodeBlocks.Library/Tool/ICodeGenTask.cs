namespace Brimborium.CodeBlocks.Tool {
    public interface ICodeGenTask {
        public int GetOrder() { return 100; }

        void Execute() { }
    }
}
