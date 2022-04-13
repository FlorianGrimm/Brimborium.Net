
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brimborium.GenerateStoredProcedure {
    public sealed class PrintOutput {
        public StringBuilder Output { get; }
        public Dictionary<string, string> BoundVariables { get; }
        public PrintOutput(
            StringBuilder Output,
            Dictionary<string, string> BoundVariables) {
            this.Output = Output;
            this.BoundVariables = BoundVariables;
        }
        public bool IndentWritten;

        public PrintOutput AppendIndent(string indent) {
            this.IndentWritten = true;
            this.Output.Append(indent);
            return this;
        }

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

    public class PrintContext {
        public PrintOutput PrintOutput { get; }
        public string Indent { get; set; }
        public int Index { get; set; }
        public int Count { get; set; }
        public PrintContext(
            StringBuilder Output,
            Dictionary<string, string> BoundVariables
            ) {
            this.PrintOutput = new PrintOutput(Output, BoundVariables);
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

        public PrintContext SetListPosition(int index, int count) {
            return new PrintContext(this, this.Indent, index, count);
        }

        public bool IsFirst => this.Index == 0;

        public bool IsLast => this.Index + 1 == Count;
    }
}
