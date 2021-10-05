namespace Brimborium.CodeBlocks.Library {
    public static class CSharpUtility {
        public static string WihoutAttribute(ICBCodeTypeName codeTypeName) {
            var tn = codeTypeName.TypeName;
            return new CBCodeTypeName(
                codeTypeName.Namespace,
                tn.EndsWith("Attribute") ? tn.Substring(0, tn.Length - 9) : tn
                ).FullName;
        }
    }
}
