namespace Brimborium.CodeGeneration {
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public record RenderBinding(
        ) {
        public virtual void Render(PrintContext printContext) {
        }
        public virtual string GetName()
            => string.Empty;

        public virtual string GetCodeIdentity()
            => string.Empty;

        public virtual string GetFilename(Dictionary<string, string> boundVariables)
            => string.Empty;
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public record RenderBinding<T>(
       T Data,
       RenderTemplate<T> Template
    ) : RenderBinding() {
        public override string GetName() {
            return this.Template.GetName(this.Data!);
        }

        public override string GetCodeIdentity() {
            return this.Template.GetCodeIdentity(this.Data!);
        }

        public override string GetFilename(Dictionary<string, string> boundVariables) {
            return base.GetFilename(boundVariables);
        }

        public override void Render(PrintContext printContext) {
            this.Template.Render(this.Data, printContext);
        }
    }
}
