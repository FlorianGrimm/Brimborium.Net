namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeTypeMember : ICBCodeElement {
        CBCodeAccessibilityLevel AccessibilityLevel { get; set; }
        string Name { get; set; }
    }
}