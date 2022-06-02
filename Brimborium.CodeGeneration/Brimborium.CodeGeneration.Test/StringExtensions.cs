namespace System {
    public static class StringExtensions {
        public static string ReplaceNewLineToPipe(this string value) => value.Replace(System.Environment.NewLine, "|");
        public static string ReplacePipeToNewLine(this string value) => value.Replace("|", System.Environment.NewLine);
    }
}
