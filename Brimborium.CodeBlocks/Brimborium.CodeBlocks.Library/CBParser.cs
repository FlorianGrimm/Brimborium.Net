using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Brimborium.CodeBlocks {
    public class CBParser {
        private static Regex? _RegexTemplate;

        public CBParser() {

        }
        public List<CBToken> Tokenize(string txtTemplate) {
            var result = new List<CBToken>();
            var regex = (_RegexTemplate ??= new Regex(
                "(?:/\\*)(?:(?<s1>\\<\\<)|(?<s2>--))\\s*(?<n>[^/*<>]+)\\s*(?:(?<f2>--)|(?<f1>\\>\\>))(?:\\*/)",
                RegexOptions.Multiline | RegexOptions.ExplicitCapture));
            var match = regex.Match(txtTemplate);
            var iPosStart = 0;
            while (match.Success) {
                var replacement = new CBToken() { Kind = CBParserResultKind.Replacement };

                if (iPosStart != match.Index) {
                    var txtMatch = txtTemplate.Substring(iPosStart, match.Index - iPosStart);
                    int iPosEnd = txtMatch.Length - 1;
                    int iPosLineEnd = iPosEnd;
                    int iPosWordEnd = iPosEnd;
                    for (int iPos = iPosEnd; iPos >= 0; iPos--) {
                        if ((txtMatch[iPos] == ' ') || (txtMatch[iPos] == '\t')) {
                            iPosWordEnd = iPos;
                            continue;
                        }
                        if ((txtMatch[iPos] == '\r') || (txtMatch[iPos] == '\n')) {
                            iPosLineEnd = iPos;
                            continue;
                        }
                        break;
                    }
                    if (iPosLineEnd < iPosEnd) {
                        replacement.PrefixWS = txtMatch.Substring(iPosLineEnd);
                        txtMatch = txtMatch.Substring(0, iPosLineEnd);
                    } else if (iPosWordEnd < iPosEnd) {
                        replacement.PrefixWS = txtMatch.Substring(iPosWordEnd);
                        txtMatch = txtMatch.Substring(0, iPosWordEnd);
                    }

                    result.Add(CBToken.Fixed(txtMatch));
                }


                if (match.Groups[1].Success) {
                    replacement.Start = true;
                } else if (match.Groups[2].Success) {
                    replacement.Start = false;
                }
                if (match.Groups[3].Success) {
                    replacement.Text = match.Groups[3].Value;
                }
                if (match.Groups[4].Success) {
                    replacement.Finish = false;
                } else if (match.Groups[5].Success) {
                    replacement.Finish = true;
                }

                result.Add(replacement);

                iPosStart = match.Index + match.Value.Length;
                match = match.NextMatch();
            }

            if (iPosStart != (txtTemplate.Length - 1)) {
                var text = txtTemplate.Substring(iPosStart);
                result.Add(CBToken.Fixed(text));
            }
            return result;
        }
    }
    public enum CBParserResultKind { Fixed, Replacement }
    public class CBToken {
        public static CBToken Fixed(string value) => new CBToken() { Kind = CBParserResultKind.Fixed, Text = value };
        public static CBToken Replacement(string value) => new CBToken() { Kind = CBParserResultKind.Replacement, Text = value };
        public CBToken() {
            this.PrefixWS = string.Empty;
            this.Text = string.Empty;
        }
        public CBParserResultKind Kind { get; set; }
        public string PrefixWS { get; set; }
        public string Text { get; set; }
        public bool Start { get; set; }
        public bool Finish { get; set; }
    }
}
