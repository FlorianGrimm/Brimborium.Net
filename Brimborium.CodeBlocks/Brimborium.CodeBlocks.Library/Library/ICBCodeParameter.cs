namespace Brimborium.CodeBlocks.Library {
    public interface ICBCodeParameter : ICBCodeElement {
        bool IsOut { get; set; }

        string Name { get; set; }

        ICBCodeTypeReference Type { get; set; }
    }
}