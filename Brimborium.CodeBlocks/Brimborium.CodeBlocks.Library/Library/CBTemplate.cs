using System;

namespace Brimborium.CodeBlocks.Library {
    public abstract class CBTemplate {

        public CBTemplate() {
        }

        public abstract Type GetRenderType();

        public virtual bool CanRenderType(Type type, string name) {
            return (type == this.GetRenderType());
        }

        public abstract void Render(object? value, CBRenderContext ctxt);
    }

    public abstract class CBTemplate<T> : CBTemplate {
        public CBTemplate() {
        }

        public override Type GetRenderType() => typeof(T);

        public override void Render(object? value, CBRenderContext ctxt) {
            if (value is T valueT) {
                this.RenderT(valueT, ctxt);
            } else {
                throw new InvalidCastException(typeof(T).FullName);
            }
        }

        public abstract void RenderT(T value, CBRenderContext ctxt);
    }

    public sealed record CBNamedTemplate(string Name, CBTemplate Template) {
        public bool CanRenderType(Type type, string name) {
            if (this.Template.CanRenderType(type, name)) {
                return StringComparer.Ordinal.Equals(this.Name, name);
            } else {
                return false;
            }
        }
    }
}
