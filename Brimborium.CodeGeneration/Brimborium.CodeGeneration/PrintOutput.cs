namespace Brimborium.CodeGeneration {
    public sealed class PrintOutput {
        public StringBuilder Output { get; }
        public Dictionary<string, string> BoundVariables { get; }
        public Dictionary<string, string> Flags { get; }
        public Dictionary<string, string> Customize { get; }
        
        public PrintOutput(
            StringBuilder Output,
            Dictionary<string, string> BoundVariables,
            Dictionary<string, string> Flags,
            Dictionary<string, string> Customize
            ) {
            this.Output = Output;
            this.BoundVariables = BoundVariables;
            this.Flags = Flags;
            this.Customize = Customize;
        }
        public bool IndentWritten;

        //public PrintOutput AppendIndent(string indent) {
        //    this.IndentWritten = true;
        //    this.Output.Append(indent);
        //    return this;
        //}

        public PrintOutput Append(string indent, string value, bool newLine) {
            if (!this.IndentWritten) {
                this.IndentWritten = true;
                this.Output.Append(indent);
            }
            this.Output.Append(value);
            if (newLine) {
                this.Output.AppendLine();
                this.IndentWritten = false;
            }
            return this;
        }

        public PrintOutput AppendLine() {
            this.IndentWritten = false;
            this.Output.AppendLine();
            return this;
        }
    }
}
