namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeExpression : ICBCodeElement {
        CBCodeType? ResultType { get; set; }
    }
}