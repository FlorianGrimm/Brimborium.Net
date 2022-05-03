
using System;
using System.Text.RegularExpressions;

namespace Brimborium.GenerateStoredProcedure {
    public sealed record ReplacementTemplate<T>(
        string Name,
        RenderTemplate<T> Template
    ) : RenderTemplate();

    public record ReplacementBinding(
        string Name
    ) : RenderBinding();

    public sealed record ReplacementBinding<T>(
        string Name,
        T Data,
        RenderTemplate<T> Template
    ) : ReplacementBinding(Name) {
        public override void Render(PrintContext printContext) {
            this.Template.Render(this.Data, printContext);
        }
    }

    public static class ReplacementBindingExtension {
        // -- Replace=x --
        // -- Replace#x --
        public const string ReplaceStart = "-- Replace=";
        public const string ReplaceFinish = "-- Replace#";

        public static bool ContainsReplace(string content) {
            return content.Contains(ReplaceStart)
                && content.Contains(ReplaceFinish)
                ;
        }

        private static Regex _RegexStart = new Regex("([-][-][ ]Replace(?:\\d*))(?:[=])((?:(?:[^ ]+)|(?:[ ][^-]))+)([ ][-][-])((?:[ ]*(?:\\r?\\n?)))?");
        // private static Regex _RegexStop = new Regex("([-][-][ ]Replace(?:\\d*))(?:[#])((?:(?:[^ ]+)|(?:[ ][^-]))+)([ ][-][-])");

        public static ReplacePositionStart IndexOfReplaceStart(string content, int startat) {
            var m = _RegexStart.Match(content, startat);
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
            var tokenStop = $"{replacePositionStart.prefix}#{replacePositionStart.name}{replacePositionStart.suffix}";
            var posStop = content.IndexOf(tokenStop, startat);
            if (posStop > 0) {
                var len = tokenStop.Length;
                while (posStop > 0 && (content[posStop] == ' ' || content[posStop] == '\t')) {
                    posStop--;
                    len++;
                }
                if (startat == posStop) { 
                    //
                } else if (posStop > 2 && content.Substring(posStop - 2, 2) == "\r\n") {
                    posStop -= 2;
                    len += 2;
                } else if (posStop > 2 && (content[posStop - 1] == '\r' || content[posStop - 1] == '\n')) {
                    posStop -= 1;
                    len++;
                }
                return new ReplacePositionStop(start: posStop, prefixTokenLen: len, tokenStop: tokenStop);
            } else {
                return new ReplacePositionStop(start: -1, prefixTokenLen: 0, tokenStop: string.Empty);
            }
        }

        public static (bool changed, string content) Replace(string content, Func<string, int, string> replace) {
            if (content.StartsWith("-- Replace=SNIPPETS --")) { 
            }
            var posStart = 0;
            var changed = false;
            while (posStart < content.Length) {
                var rpStart = IndexOfReplaceStart(content, posStart);
                if (rpStart.start < 0) { break; }
                //
                var rpStop = IndexOfReplaceStop(content, rpStart);
                if (rpStop.start < 0) {
                    var startContent = content.Substring(rpStart.start, rpStart.len);
                    System.Console.Error.WriteLine($"stop not found: {startContent}");
                    break; 
                }
                //
                var wsStart = 0;
                while ((0 <= (rpStart.start - wsStart - 1)) && IsSpaceOrTab(content[(rpStart.start - wsStart - 1)])) {
                    wsStart++;
                }

                var wsStop = 0;
                while ((0 <= (rpStop.start - wsStop - 1)) && IsSpaceOrTab(content[(rpStop.start - wsStop - 1)])) {
                    wsStop++;
                }

                var rpValue = replace(rpStart.name, wsStart);
                if (string.IsNullOrEmpty(rpValue)) {
                    posStart = rpStop.start + rpStop.tokenStop.Length;
                } else {
                    try {
                        content.Substring(rpStart.start + rpStart.len, rpStop.start - rpStart.start - rpStart.len);
                    } catch { 
                    }
                    var oldContent = content.Substring(rpStart.start + rpStart.len, rpStop.start - rpStart.start - rpStart.len);
                    if (string.Equals(rpValue, oldContent, StringComparison.Ordinal)) {
                        posStart = rpStart.start + rpStart.len + rpValue.Length + rpStop.prefixTokenLen;
                    } else {
                        var startContent = content.Substring(0, rpStart.start + rpStart.len);
                        var stopContent = content.Substring(rpStop.start - wsStop);
                        content = startContent + rpValue + stopContent;
                        posStart = rpStart.start + rpStart.len + rpValue.Length + rpStop.prefixTokenLen;
                        changed = true;
                    }
                }
            }
            return (changed, content);
        }

        private static bool IsSpaceOrTab(char c) {
            return c == ' ' || c == '\t';
        }
    }

    public sealed record ReplacePositionStart(int start, int len, string prefix, string name, string suffix);
    public sealed record ReplacePositionStop(int start, int prefixTokenLen, string tokenStop);
}
