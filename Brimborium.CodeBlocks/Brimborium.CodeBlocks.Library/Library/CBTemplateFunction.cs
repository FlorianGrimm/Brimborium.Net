using System;

namespace Brimborium.CodeBlocks.Library {
    public class CBTemplateFunction<T> : CBTemplate<T> {
        private Action<T, CBRenderContext> _Render;

        public CBTemplateFunction(Action<T, CBRenderContext> render) {
            this._Render = render;
        }

        public override void RenderT(T value, CBRenderContext ctxt) {
            this._Render(value, ctxt);
        }
    }
}