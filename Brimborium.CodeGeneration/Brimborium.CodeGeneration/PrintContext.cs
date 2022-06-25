namespace Brimborium.CodeGeneration {
    public class PrintContext {
        public PrintOutput PrintOutput { get; }
        public StringBuilder Output { get; }

        public string Indent { get; set; }
        public int Index { get; set; }
        public int Count { get; set; }
        
        public PrintContext(
            StringBuilder Output,
            Dictionary<string, string>? BoundVariables = default,
            Dictionary<string, string>? Flags = default,
            Dictionary<string, string>? Customize = default
            ) {
            this.Output = Output;
            this.PrintOutput = new PrintOutput(Output, BoundVariables ?? new Dictionary<string, string>(), Flags ?? new Dictionary<string, string>(), Customize ?? new Dictionary<string, string>());
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

        public void AppendHeader() {
            foreach (var flag in this.PrintOutput.Flags.OrderBy(kv=>kv.Key)) { 
                this.AppendPartsLine("/*-- ",flag.Key,":",flag.Value," --*/");
            }
        }

        public void AppendCustomize(string name) {
            if (this.PrintOutput.Customize.TryGetValue(name, out var content)) {
                this.AppendPartsLine("/*-- Customize=", name, " --*/");
                this.PrintOutput.Output.Append(content);
                this.AppendPartsLine("/*-- /Customize=", name, " --*/");
            } else if (this.PrintOutput.Flags.TryGetValue("Customize", out var customize)) { 
                if (string.Equals(customize, "on", StringComparison.Ordinal)) {
                    this.AppendPartsLine("/*-- Customize=", name, " --*/");
                    this.AppendPartsLine("/*-- /Customize=", name, " --*/");
                }
            }
        }

        public void AppendCustomizeWithDefaultContent<T>(
            string name,
            T Data,
            Action<T, PrintContext> Render) {            
            if (this.PrintOutput.Customize.TryGetValue($"Custom{name}", out var content)
                && !string.IsNullOrWhiteSpace(content)) {
                // this.AppendPartsLine("/*-- Customize=Default", name, " --*/");
                // this.AppendPartsLine("/*-- /Customize=Default", name, " --*/");
                this.AppendPartsLine("/*-- Customize=Custom", name, " --*/");
                this.PrintOutput.Output.Append(content);
                this.AppendPartsLine("/*-- /Customize=Custom", name, " --*/");
            } else if (this.PrintOutput.Flags.TryGetValue("Customize", out var customize)
                && (string.Equals(customize, "on", StringComparison.Ordinal))) {
                this.AppendPartsLine("/*-- Customize=Default", name, " --*/");
                Render(Data, this);
                this.AppendPartsLine("/*-- /Customize=Default", name, " --*/");
                this.AppendPartsLine("/*-- Customize=Custom", name, " --*/");
                this.AppendPartsLine("/*-- /Customize=Custom", name, " --*/");
            } else {
                Render(Data, this);
            }
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
