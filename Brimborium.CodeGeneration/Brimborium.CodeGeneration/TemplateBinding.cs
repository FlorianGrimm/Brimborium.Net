﻿namespace Brimborium.CodeGeneration {
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public record TemplateBinding<T>(
        T Data,
        RenderTemplate<T> Template
        ) : RenderBinding<T>(Data, Template)
        where T : notnull {
        public override string GetName() {
            return this.Template.GetName(this.Data);
        }

        public override string GetCodeIdentity() {
            return this.Template.GetCodeIdentity(this.Data);
        }

        public override string GetFilename(Dictionary<string, string> boundVariables) {
            return this.Template.GetFilename(this.Data, boundVariables);
        }
    }
}
