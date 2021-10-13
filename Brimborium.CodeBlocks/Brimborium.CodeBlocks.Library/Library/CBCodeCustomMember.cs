using System.Collections.Generic;

namespace Brimborium.CodeBlocks.Library {
    public class CBCodeCustomMember : ICBCodeTypeMember {
        public CBCodeCustomMember() {
            this.Name = string.Empty;
        }
        public CBCodeAccessibilityLevel AccessibilityLevel { get; set; }
        public string Name { get; set; }
        public ICBCodeElement? Parent { get; set; }
        public virtual IEnumerable<ICBCodeElement> GetChildren() => new ICBCodeElement[0];
    }
}