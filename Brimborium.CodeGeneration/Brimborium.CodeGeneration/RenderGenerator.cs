namespace Brimborium.CodeGeneration {
    public class RenderGenerator {
        private readonly Dictionary<string, RenderBinding> _BindingByName;
        private readonly Dictionary<string, string> _TemplateVariables;

        public RenderGenerator(
                List<RenderBinding> replacementBindings,
                Dictionary<string, string> templateVariables
            ) {
            this._TemplateVariables = templateVariables;
            this._BindingByName = new Dictionary<string, RenderBinding>();
            this.AddRange(replacementBindings);
        }
        public RenderGenerator AddRange(List<RenderBinding> replacementBindings) {
            foreach (var replacementBinding in replacementBindings) {
                var name = replacementBinding.GetName();
                if (string.IsNullOrEmpty(name)) {
                    // skip
                } else {
                    this._BindingByName.Add(name, replacementBinding);
                }
            }
            return this;
        }
        public bool Add(RenderBinding replacementBinding) {
            var name = replacementBinding.GetName();
            if (string.IsNullOrEmpty(name)) {
                // skip
                return false;
            } else {
                this._BindingByName.Add(name, replacementBinding);
                return true;
            }
        }

        public string GetValue(
                string name,
                int ws = 0
            ) {
            if (this._BindingByName.TryGetValue(name, out var replacementBinding)) {
                var sbOutput = new StringBuilder();
                var printContext = new PrintContext(sbOutput, this._TemplateVariables);
                if (0 < ws) {
                    printContext = printContext.GetIndented(new string(' ', ws));
                }
                replacementBinding.Render(printContext);
                return sbOutput.ToString();
            }
            if (name == "HELP") {
                var sbOutput = new StringBuilder();
                var printContext = new PrintContext(sbOutput, this._TemplateVariables);
                printContext = printContext.GetIndented(new string(' ', ws + 4));
                var names = this._BindingByName.Keys.OrderBy(k => k, StringComparer.InvariantCultureIgnoreCase);
                foreach (var n in names) {
                    printContext.AppendLine($"-- {n}");
                }
                return sbOutput.ToString();
            }
            if (name == "SNIPPETS") {
                var sbOutput = new StringBuilder();
                var printContext = new PrintContext(sbOutput, this._TemplateVariables);
                printContext = printContext.GetIndented(new string(' ', ws + 4));
                var names = this._BindingByName.Keys.OrderBy(k => k, StringComparer.InvariantCultureIgnoreCase);
                foreach (var n in names) {
                    printContext.AppendLine($"-- Replace={n} --");
                    this._BindingByName[n].Render(printContext);
                    printContext.AppendLine($"-- Replace#{n} --");
                    printContext.AppendLine("");
                }
                return sbOutput.ToString();
            }
            return string.Empty;
        }
    }
}
