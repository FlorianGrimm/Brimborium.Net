using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeElement {
        ICBCodeElement? Parent { get; set; }

        IEnumerable<ICBCodeElement> GetChildren();
    }
}
