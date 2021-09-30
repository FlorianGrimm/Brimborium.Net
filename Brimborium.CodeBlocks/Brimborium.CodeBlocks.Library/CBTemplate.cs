using System.Threading.Tasks;

namespace Brimborium.CodeBlocks.Library {
    public class CBTemplate {
        public CBTemplate() {
        }

        public virtual async ValueTask Render(
            CBValue context,
            CBRenderer cbRender
            ) {
            await cbRender.BeginTemplate(this);
            cbRender.EnsureNewLine();
            cbRender.WriteText("");
            await cbRender.EndTemplate(this);
            return;
        }
    }
}
