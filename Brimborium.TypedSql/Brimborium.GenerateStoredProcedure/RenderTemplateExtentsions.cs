namespace Brimborium.GenerateStoredProcedure {
    public static class RenderTemplateExtentsions {
#pragma warning disable IDE0060 // Remove unused parameter
        public static string GetFilename<T>(T data, string filePattern, Dictionary<string, string> boundVariables) {
#pragma warning restore IDE0060 // Remove unused parameter
            var result = new StringBuilder();
            var iPosPrev = 0;
            while (iPosPrev < filePattern.Length) {
                var iPosStart = filePattern.IndexOf('[', iPosPrev);
                if (iPosStart < 0) {
                    break;
                }
                var iPosEnd = filePattern.IndexOf(']', iPosStart);
                //
                if (iPosPrev < iPosStart) {
                    var constPart = filePattern.Substring(iPosPrev, iPosStart - iPosPrev);
                    result.Append(constPart);
                }
                //
                {
                    var namePart = filePattern.Substring(iPosStart + 1, iPosEnd - iPosStart - 1);
                    if (boundVariables.TryGetValue(namePart, out var value)) {
                        result.Append(value);
                    }
                }
                //
                iPosPrev = iPosEnd + 1;
            }
            if (iPosPrev < filePattern.Length) {
                var constPart = filePattern.Substring(iPosPrev, filePattern.Length - iPosPrev);
                result.Append(constPart);
            }
            var outputFolder = boundVariables["ProjectRoot"]!;
            return System.IO.Path.Combine(
                outputFolder,
                result.ToString()
            );
        }

        public static string GetAbsoluteFilename(string fileName, Dictionary<string, string> boundVariables) {
            var outputFolder = boundVariables["ProjectRoot"]!;
            return System.IO.Path.Combine(
                outputFolder,
                fileName
            );
        }

        public static Func<T, Dictionary<string, string>, string> GetFileNameBind<T>(string filePattern) {
            return bound;

            string bound(T data, Dictionary<string, string> boundVariables) {
                return GetFilename(data, filePattern, boundVariables);
            }
        }
    }
}
