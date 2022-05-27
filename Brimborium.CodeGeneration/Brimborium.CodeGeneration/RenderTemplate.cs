namespace Brimborium.CodeGeneration {
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public record RenderTemplate(
        ) {
        public virtual string GetName(object data) {
            return string.Empty;
        }
        public virtual string GetCodeIdentity(object data) {
            return string.Empty;
        }
        public virtual string GetFilename(object data, Dictionary<string, string> boundVariables) {
            return string.Empty;
        }
    }

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed record RenderTemplate<T>(
        Action<T, PrintContext> Render,
        Func<T, string>? NameFn = default,
        Func<T, string>? CodeIdentityFn = default,
        Func<T, Dictionary<string, string>, string>? FileNameFn = default
        ) : RenderTemplate() {

        public override string GetName(object data) {
            if (this.NameFn is null) {
                return string.Empty;
            } else {
                return this.NameFn((T)data);
            }
        }

        public override string GetCodeIdentity(object data) {
            if (this.CodeIdentityFn is null) {
                return string.Empty;
            } else {
                return this.CodeIdentityFn((T)data);
            }
        }

        public override string GetFilename(object data, Dictionary<string, string> boundVariables) {
            if (this.FileNameFn is null) {
                return string.Empty;
            } else {
                return this.FileNameFn((T)data, boundVariables);
            }
        }
    }
}
