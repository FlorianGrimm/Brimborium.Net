
using System.Collections.Generic;

namespace Brimborium.GenerateStoredProcedure {
    public record TemplateBinding<T>(
        T Data,
        RenderTemplate<T> Template
        ) : RenderBinding<T>(Data, Template)
        where T : notnull {
        public override string GetName() {
            return this.Template.GetName(this.Data);
        }
        public override string GetFilename(Dictionary<string, string> boundVariables) {
            return this.Template.GetFilename(this.Data, boundVariables);
        }
    }
}
