namespace Brimborium.CodeBlocks.Library {
    public interface ICBTypeDefinition : ICBCodeTypeName {
        CBCodeAccessibilityLevel AccessibilityLevel { get; }
        public CBList<ICBCodeTypeDecoration> Attributes { get; }

        public ICBCodeTypeReference? BaseType { get; set; }

        public CBList<ICBCodeTypeReference> Interfaces { get; }

        public CBList<ICBCodeDefinitionMember> Members { get; }
    }

    public class CBTypeDefinition : CBCodeTypeName, ICBTypeDefinition {
        public CBTypeDefinition() : base() {
            this.Attributes = new CBList<ICBCodeTypeDecoration>(this);
            this.Interfaces = new CBList<ICBCodeTypeReference>(this);
            this.Members = new CBList<ICBCodeDefinitionMember>(this);
        }

        public CBTypeDefinition(CBCodeNamespace @namespace, string typeName) : base(@namespace, typeName) {
            this.Attributes = new CBList<ICBCodeTypeDecoration>(this);
            this.Interfaces = new CBList<ICBCodeTypeReference>(this);
            this.Members = new CBList<ICBCodeDefinitionMember>(this);
        }

        public CBCodeAccessibilityLevel AccessibilityLevel { get; set; } = CBCodeAccessibilityLevel.Public;

        public CBList<ICBCodeTypeDecoration> Attributes { get; init; }

        public ICBCodeTypeReference? BaseType { get; set; } = null;

        public CBList<ICBCodeTypeReference> Interfaces { get; init; }

        public CBList<ICBCodeDefinitionMember> Members { get; init; }
    }
}