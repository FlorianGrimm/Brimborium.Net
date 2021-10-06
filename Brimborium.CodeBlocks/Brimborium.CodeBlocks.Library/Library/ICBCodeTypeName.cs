namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeTypeName : ICBCodeElement {
        CBCodeNamespace Namespace { get; set; }
        string TypeName { get; set; }
        string FullName { get;  }

        ICBCodeTypeReference? GenericTypeDefinition { get; set; }

        ICBCodeTypeReference[] GenericTypeArguments { get; set; }
    }
}