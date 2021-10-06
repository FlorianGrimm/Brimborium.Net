namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeTypeDefinition : ICBCodeTypeName {
        CBCodeAccessibilityLevel AccessibilityLevel { get; }
        public CBList<ICBCodeTypeDecoration> Attributes { get; }

        public ICBCodeTypeReference? BaseType { get; set; }

        public CBList<ICBCodeTypeReference> Interfaces { get; }

        public CBList<ICBCodeDefinitionMember> Members { get; }
    }
}