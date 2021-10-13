using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeElement {
        ICBCodeElement? Parent { get; set; }

        IEnumerable<ICBCodeElement> GetChildren();
    }

    public static class CBCodeElementExtensions {
        public static void SetOwnerIfNeeded(this ICBCodeElement that, ICBCodeElement parent) {
            if (that.Parent is null) {
                that.Parent = parent;
                var children = that.GetChildren();
                List<ICBCodeElement> lstCBCodeElement;
                if (children is List<ICBCodeElement> lst) {
                    lstCBCodeElement = lst;
                } else {
                    lstCBCodeElement = new List<ICBCodeElement>(children);
                }
                foreach (var child in lstCBCodeElement) {
                    child.SetOwnerIfNeeded(that);
                }
            }
        }
    }
}
