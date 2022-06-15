namespace Brimborium.CodeGeneration {
    public static class ReplacementBindingExtension {
        // -- Replace=x --
        // -- /Replace=x --
        public const string ReplaceStart = "-- Replace=";
        public const string ReplaceFinish = "-- /Replace=";

        public static bool ContainsReplace(string content) {
            if (content.Contains(ReplaceStart) && content.Contains(ReplaceFinish)) {
                var posStart = IndexOfReplaceStart(content, 0);
                if (posStart.start < 0) { return false; }
                var posStop = IndexOfReplaceStop(content, posStart);
                return (posStop.start >= 0);
            }
            return false;
        }

        private static Regex _RegexReplaceStart = new Regex("((?:[/][*])?[-][-][ ]Replace(?:\\d*))(?:[=])((?:(?:[^ ]+)|(?:[ ][^-]))+)([ ][-][-](?:[*][/])?)((?:[ ]*(?:\\r?\\n?)))?");

        public static ReplacePositionStart IndexOfReplaceStart(string content, int startat) {
            var m = _RegexReplaceStart.Match(content, startat);
            if (m.Success) {
                var start = m.Index;
                var len = m.Value.Length;
                var prefix = m.Groups[1].Value;
                var name = m.Groups[2].Value;
                var suffix = m.Groups[3].Value;
                return new ReplacePositionStart(start: start, len: len, prefix: prefix, name: name, suffix: suffix);
            } else {
                return new ReplacePositionStart(start: -1, len: 0, prefix: string.Empty, name: string.Empty, suffix: string.Empty);
            }
        }

        public static ReplacePositionStop IndexOfReplaceStop(string content, ReplacePositionStart replacePositionStart) {
            var startat = replacePositionStart.start + replacePositionStart.len;
            // Replace.length = 7
            var tokenStop = $"{replacePositionStart.prefix.Substring(0, replacePositionStart.prefix.Length - 7)}/Replace={replacePositionStart.name}{replacePositionStart.suffix}";
            var posStop = content.IndexOf(tokenStop, startat);
            if (posStop > 0) {                
                return new ReplacePositionStop(start: posStop, tokenStop: tokenStop);
            } else {
                return new ReplacePositionStop(start: -1, tokenStop: string.Empty);
            }
        }

        private static Regex _RegexCustomizeStart = new Regex("((?:[/][*])?[-][-][ ]Customize(?:\\d*))(?:[=])((?:(?:[^ ]+)|(?:[ ][^-]))+)([ ][-][-](?:[*][/])?)((?:[ ]*(?:\\r?\\n?)))?");

        public static ReplacePositionStart IndexOfCustomizeStart(string content, int startat) {
            var m = _RegexCustomizeStart.Match(content, startat);
            if (m.Success) {
                var start = m.Index;
                var len = m.Value.Length;
                var prefix = m.Groups[1].Value;
                var name = m.Groups[2].Value;
                var suffix = m.Groups[3].Value;
                return new ReplacePositionStart(start: start, len: len, prefix: prefix, name: name, suffix: suffix);
            } else {
                return new ReplacePositionStart(start: -1, len: 0, prefix: string.Empty, name: string.Empty, suffix: string.Empty);
            }
        }

        public static ReplacePositionStop IndexOfCustomizeStop(string content, ReplacePositionStart replacePositionStart) {
            var startat = replacePositionStart.start + replacePositionStart.len;
            // Customize.length = 9
            var tokenStop = $"{replacePositionStart.prefix.Substring(0, replacePositionStart.prefix.Length - 9)}/Customize={replacePositionStart.name}{replacePositionStart.suffix}";
            var posStop = content.IndexOf(tokenStop, startat);
            if (posStop > 0) {
                return new ReplacePositionStop(start: posStop, tokenStop: tokenStop);
            } else {
                return new ReplacePositionStop(start: -1, tokenStop: string.Empty);
            }
        }

        private static Regex _RegexFlags = new Regex("(?:(?:[/][*])?[-][-][ ])([^:]{1,128})(?:[:])([^- \\r\\n]{1,128})(?:[ ][-][-](?:[*][/])?[ \\t]*(?:\\r?\\n?)*)?");
        private static char[] newline = new char[] { '\r', '\n' };

        public static Dictionary<string, string> ReadFlags(string content, Dictionary<string, string>? result=default) {
            result ??= new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            //result["FlagsContent"] = string.Empty;
            //var sbFlagsContent = new StringBuilder();
            int pos = 0;
            while (pos < content.Length) {
                var match = _RegexFlags.Match(content, pos);
                if (match.Success) {
                    pos += match.Length;
                    var key = match.Groups[1].Value;
                    var value = match.Groups[2].Value;
                    result[key] = value;
                    //sbFlagsContent.Append(match.Value);
                } else {
                    break;
                }
            }
            //result["FlagsContent"] = sbFlagsContent.ToString();
            return result;
        }

        public static bool HasFlagValue(
            Dictionary<string, string>? flags, 
            string name, string value
            ) {
            if (flags is not null) {
                if (flags.TryGetValue(name, out var v)) {
                    return string.Equals(v, value, StringComparison.Ordinal);
                }
            }
            return false;
        }

        public static Dictionary<string, string> ReadCustomize(string content) {
            var result = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            int pos = 0;
            while (pos < content.Length) {
                var positionStart = ReplacementBindingExtension.IndexOfCustomizeStart(content, pos);
                if (positionStart.start < 0) {
                    break;
                }
                var positionStop = ReplacementBindingExtension.IndexOfCustomizeStop(content, positionStart);
                if (positionStop.start < 0) {
                    break;
                }
                var contentCustomizeStart = positionStart.start + positionStart.len;
                var len = positionStop.start - contentCustomizeStart;
                while ((len > 0) && (IsSpaceOrTab(content[contentCustomizeStart + len - 1]))){
                    len--;
                }
                var contentCustomize = content.Substring(contentCustomizeStart, len);
                result[positionStart.name] = contentCustomize;
                pos = positionStop.start + positionStop.tokenStop.Length;
            }
            return result;

        }
        public static (bool changed, string content) Replace(string content, Func<string, int, string> replace) {
            var posStart = 0;
            var changed = false;
            while (posStart < content.Length) {
                var rpStart = IndexOfReplaceStart(content, posStart);
                if (rpStart.start < 0) { break; }
                //
                var rpStop = IndexOfReplaceStop(content, rpStart);
                if (rpStop.start < 0) {
                    /*
                    var startContent = content.Substring(rpStart.start, rpStart.len);
                    Console.Error.WriteLine($"stop not found: {startContent}");
                    break;
                    */
                    throw new ArgumentException(content.Substring(rpStart.start, rpStart.len), "content");

                }
                //
                var wsStart = 0;
                while (0 <= (rpStart.start - wsStart - 1) && IsSpaceOrTab(content[rpStart.start - wsStart - 1])) {
                    wsStart++;
                }

                var wsStop = 0;
                while (0 <= rpStop.start - wsStop - 1 && IsSpaceOrTab(content[rpStop.start - wsStop - 1])) {
                    wsStop++;
                }

                var rpValue = replace(rpStart.name, wsStart);
                if (string.IsNullOrEmpty(rpValue)) {
                    posStart = rpStop.start + rpStop.tokenStop.Length;
                } else {
                    var oldContent = content.Substring(rpStart.start + rpStart.len, rpStop.start - rpStart.start - rpStart.len);
                    if (string.Equals(rpValue, oldContent, StringComparison.Ordinal)) {
                        posStart = rpStart.start + rpStart.len + rpValue.Length;
                    } else {
                        var startContent = content.Substring(0, rpStart.start + rpStart.len);
                        var stopContent = content.Substring(rpStop.start - wsStop);
                        content = startContent + rpValue + stopContent;
                        posStart = rpStart.start + rpStart.len + rpValue.Length;
                        changed = true;
                    }
                }
            }
            return (changed, content);
        }

        private static bool IsSpaceOrTab(char c) {
            return c == ' ' || c == '\t';
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public sealed record ReplacePositionStart(int start, int len, string prefix, string name, string suffix);

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public sealed record ReplacePositionStop(int start, string tokenStop);
    }
}
