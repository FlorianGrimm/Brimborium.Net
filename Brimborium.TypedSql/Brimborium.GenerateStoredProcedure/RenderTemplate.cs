
using System;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.GenerateStoredProcedure {
    public record RenderTemplate(
        ) {
        public virtual string GetName(object data) {
            return string.Empty;
        }
        public virtual string GetFilename(object data, Dictionary<string, string> boundVariables) {
            return string.Empty;
        }
    }

    public sealed record RenderTemplate<T>(
        Action<T, PrintContext> Render,
        Func<T, string>? NameFn = default,
        Func<T, Dictionary<string, string>, string>? FileNameFn = default
        ) : RenderTemplate() {
        //private static void NullRender(T data, PrintContext ctxt) { }
        public override string GetName(object data) {
            if (this.NameFn is null) {
                return string.Empty;
            } else {
                return this.NameFn((T)data);
            }
        }
        public override string GetFilename(object data, Dictionary<string, string> boundVariables) {
            if (this.FileNameFn is null) {
                return string.Empty;
            } else {
                return this.FileNameFn((T)data, boundVariables);
            }
        }
    }

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
