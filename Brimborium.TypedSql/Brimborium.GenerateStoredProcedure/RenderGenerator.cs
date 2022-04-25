
using System.Collections.Generic;
using System.Text;

namespace Brimborium.GenerateStoredProcedure {
    public class RenderGenerator {
        private readonly Dictionary<string, RenderBinding> _BindingByName;
        private readonly Dictionary<string, string> _TemplateVariables;

        public RenderGenerator(
                List<RenderBinding> replacementBindings,
                Dictionary<string, string> templateVariables
            ) {
            this._TemplateVariables = templateVariables;
            this._BindingByName = new Dictionary<string, RenderBinding>();
            foreach (var replacementBinding in replacementBindings) {
                var name = replacementBinding.GetName();
                if (string.IsNullOrEmpty(name)) {
                    // skip
                } else {
                    this._BindingByName.Add(name, replacementBinding);
                }
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
            return string.Empty;
        }
    }
}
