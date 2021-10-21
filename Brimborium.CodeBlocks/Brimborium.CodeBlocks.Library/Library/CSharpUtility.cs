namespace Brimborium.CodeBlocks.Library {
    public static class CSharpUtility {
        //public static string WihoutAttribute(ICBCodeTypeName codeTypeName) {
        //    var tn = codeTypeName.TypeName;
        //    return new CBCodeTypeName(
        //        codeTypeName.Namespace,
        //        tn.EndsWith("Attribute") ? tn.Substring(0, tn.Length - 9) : tn
        //        ).FullName;
        //}
    }
    public static class StringUtility {
        public static string FirstUpperCase(string txt) {
            return txt.Substring(0, 1).ToUpperInvariant() + txt.Substring(1);
        }
    }
}
