
using System.Collections.Generic;

namespace Brimborium.GenerateStoredProcedure {
    public record RenderBinding(
        ) {
        public virtual void Render(PrintContext printContext) {
        }
        public virtual string GetName()
            => string.Empty;

        public virtual string GetFilename(Dictionary<string, string> boundVariables)
            => string.Empty;

        public static string GetFilenameFromTableInfo(
            TableInfo data,
            Dictionary<string, string> boundVariables,
            RenderTemplate template
            ) {
            boundVariables["Schema"] = data.Table.Schema;
            boundVariables["Name"] = data.Table.Name;
            return template.GetFilename(data, boundVariables);
        }
    }

    public record RenderBinding<T>(
       T Data,
       RenderTemplate<T> Template
    ) : RenderBinding() {
        public override void Render(PrintContext printContext) {
            this.Template.Render(this.Data, printContext);
        }
    }
}
