namespace Brimborium.CodeGeneration {
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed record ReplacementTemplate<T>(
        string Name,
        RenderTemplate<T> Template
    ) : RenderTemplate();

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public record ReplacementBinding(
        string Name
    ) : RenderBinding();

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed record ReplacementBinding<T>(
        string Name,
        T Data,
        RenderTemplate<T> Template
    ) : ReplacementBinding(Name) {
        public override void Render(PrintContext printContext) {
            this.Template.Render(this.Data, printContext);
        }
    }

}
