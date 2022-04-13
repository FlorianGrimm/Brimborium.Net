
using System.Text;

namespace Brimborium.TypedStoredProcedure {
    public sealed record PrintContext(StringBuilder Output, string Indent = "", string AddIndent = "    ") {
        public PrintContext Indented() => this with { Indent = Indent + AddIndent };
        public PrintContext AppendLine(string line) {
            this.Output.Append(this.Indent);
            this.Output.AppendLine(line);
            return this;
        }

        public override string ToString() => this.Output.ToString();
    }
}
