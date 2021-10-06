namespace Brimborium.CodeBlocks.Library {
    public sealed class CBCodeFileClass : CBCodeFileGeneration {
        public static CBCodeFileClass Create(CBCodeNamespace @namespace, string typeName, string fileName) {
            var codeFile = new CBCodeFile();
            var codeClass = new CBCodeClass(@namespace, typeName);
            codeFile.Namespace = @namespace;
            codeFile.Items.Add(codeClass);
            return new CBCodeFileClass(codeFile, codeClass, fileName);
        }

        public CBCodeFileClass(CBCodeFile codeFile, CBCodeClass codeClass, string fileName) : base(codeFile, fileName) {
            this.CodeClass = codeClass;
        }

        public void Deconstruct(out CBCodeFile codeFile, out CBCodeClass codeClass) {
            codeFile = this.CodeFile;
            codeClass = this.CodeClass;
        }

        public CBCodeClass CodeClass { get; }
    }
    public sealed class CBCodeFileInterface : CBCodeFileGeneration {
        public static CBCodeFileInterface Create(CBCodeNamespace @namespace, string typeName, string fileName) {
            var codeFile = new CBCodeFile();
            var codeInterface = new CBCodeInterface(@namespace, typeName);
            codeFile.Namespace = @namespace;
            codeFile.Items.Add(codeInterface);
            return new CBCodeFileInterface(codeFile, codeInterface, fileName);
        }

        public CBCodeFileInterface(CBCodeFile codeFile, CBCodeInterface codeInterface, string fileName) : base(codeFile, fileName) {
            this.CodeInterface = codeInterface;
        }

        public void Deconstruct(out CBCodeFile codeFile, out CBCodeInterface codeInterface) {
            codeFile = this.CodeFile;
            codeInterface = this.CodeInterface;
        }

        public CBCodeInterface CodeInterface { get; }
    }
}
