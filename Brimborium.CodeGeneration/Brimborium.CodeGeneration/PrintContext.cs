namespace Brimborium.CodeGeneration {
    public class PrintContext {
        public PrintOutput PrintOutput { get; }
        public StringBuilder Output { get; }

        public string Indent { get; set; }
        public int Index { get; set; }
        public int Count { get; set; }
        
        public PrintContext(
            StringBuilder Output,
            Dictionary<string, string>? BoundVariables = default
            ) {
            this.Output = Output;
            this.PrintOutput = new PrintOutput(Output, BoundVariables ?? new Dictionary<string, string>());
            this.Indent = string.Empty;
            this.Index = -1;
            this.Count = -1;
        }

        public PrintContext(
            PrintContext printContext,
            string Indent,
            int Index,
            int Count
            ) {
            this.Output = printContext.Output;
            this.PrintOutput = printContext.PrintOutput;
            this.Indent = Indent;
            this.Index = Index;
            this.Count = Count;
        }
        public PrintContext GetIndented(string addIndent = "    ") {
            return new PrintContext(this, this.Indent + addIndent, this.Index, this.Count);
        }
        public PrintContext Append(string value) {
            this.PrintOutput.Append(this.Indent, value, false);
            return this;
        }

        public void AppendLineAndError(string line) {
            this.PrintOutput.Append(this.Indent, line, true);
            System.Console.Error.WriteLine(line);
        }

        public PrintContext AppendParts(params string[] value) {
            foreach (var part in value) {
                this.PrintOutput.Append(this.Indent, part, false);
            }
            return this;
        }

        public PrintContext AppendLine(string? value = null) {
            if (string.IsNullOrEmpty(value)) {
                this.PrintOutput.AppendLine();
            } else {
                this.PrintOutput.Append(this.Indent, value, true);
            }
            return this;
        }

        public PrintContext AppendPartsLine(params string[] value) {
            foreach (var part in value) {
                this.PrintOutput.Append(this.Indent, part, false);
            }
            this.PrintOutput.AppendLine();
            return this;
        }

        //public void AppendCurlyBlock( string lineFirstBefore, string lineLastAfter, PrintContext ctxt, Action<PrintContext> inner) {
        //    if (string.IsNullOrEmpty(lineFirstBefore)) { ctxt.AppendLine($"{{"); } else { ctxt.AppendLine($"{lineFirstBefore} {{"); }
        //    inner(ctxt.GetIndented());
        //    if (string.IsNullOrEmpty(lineLastAfter)) { ctxt.AppendLine($"}}"); } else { ctxt.AppendLine($"}} {lineLastAfter}"); }
        //}

        public PrintContext SetListPosition(int index, int count) {
            return new PrintContext(this, this.Indent, index, count);
        }

        public bool IsFirst => this.Index == 0;

        public bool IsLast => this.Index + 1 == Count;
    }
}
