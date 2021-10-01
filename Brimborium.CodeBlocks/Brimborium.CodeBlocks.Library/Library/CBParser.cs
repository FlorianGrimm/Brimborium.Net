
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Brimborium.CodeBlocks.Library {
    public sealed class CBParser {
        private static Regex? _RegexTemplate;

        public CBParser() {

        }
        public List<CBToken> Tokenize(string txtTemplate) {
            var result = new List<CBToken>();
            var regex = _RegexTemplate ??= new Regex(
                "(?:/\\*)(?:(?<s1>\\<\\<)|(?<s2>--))\\s*(?<n>[^->:]+)\\s*(?:\\:\\s*(?<m>[^->]+)\\s*)?(?:(?<f2>--)|(?<f1>\\>\\>))(?:\\*/)",
                RegexOptions.Multiline | RegexOptions.ExplicitCapture);
            var match = regex.Match(txtTemplate);
            var iPosStart = 0;
            while (match.Success) {
                var replacement = new CBToken() { Kind = CBParserResultKind.Replacement };

                if (iPosStart != match.Index) {
                    var txtMatch = txtTemplate.Substring(iPosStart, match.Index - iPosStart);
                    result.Add(CBToken.Fixed(txtMatch));
                }

                if (match.Groups[1].Success) {
                    replacement.Start = true;
                } else if (match.Groups[2].Success) {
                    replacement.Start = false;
                }
                if (match.Groups[3].Success) {
                    replacement.Text = match.Groups[3].Value.Trim();
                    if (match.Groups[4].Success) {
                        replacement.Text = match.Groups[4].Value.Trim();
                    }
                } else {
                    throw new ArgumentException("name missing");
                }
                if (match.Groups[5].Success) {
                    replacement.Finish = false;
                } else if (match.Groups[6].Success) {
                    replacement.Finish = true;
                }

                result.Add(replacement);

                iPosStart = match.Index + match.Value.Length;
                match = match.NextMatch();
            }

            if (iPosStart != txtTemplate.Length - 1) {
                var text = txtTemplate.Substring(iPosStart);
                result.Add(CBToken.Fixed(text));
            }
            for (var idx = 1; idx < result.Count - 1; idx++) {
                var replacement = result[idx];
                if (replacement.Kind == CBParserResultKind.Replacement) {
                    if (replacement.Start) {
                        var tokenBefore = result[idx - 1];
                        var txtMatch = tokenBefore.Text;
                        var iPosEnd = txtMatch.Length - 1;
                        var iPosWordEnd = iPosEnd;
                        for (var iPos = iPosEnd; iPos >= 0; iPos--) {
                            if (txtMatch[iPos] == ' ' || txtMatch[iPos] == '\t') {
                                iPosWordEnd = iPos;
                                continue;
                            }
                            break;
                        }
                        if (iPosWordEnd < iPosEnd) {
                            replacement.IndentWS = txtMatch.Substring(iPosWordEnd);
                        }
                    }
                    if (replacement.Finish) {
                        var tokenAfter = result[idx + 1];
                        var txtMatch = tokenAfter.Text;
                        var iPosEnd = txtMatch.Length;
                        var iPosWordStart = 0;
                        var iPos = 0;
                        for (iPos = 0; iPos < iPosEnd; iPos++) {
                            if (txtMatch[iPos] == ' ' || txtMatch[iPos] == '\t') {
                                iPosWordStart = iPos;
                                continue;
                            }
                            break;
                        }
                        for (iPos = 0; iPos < iPosEnd; iPos++) {
                            if (txtMatch[iPos] == '\r' || txtMatch[iPos] == '\n') {
                                iPosWordStart = iPos;
                                continue;
                            }
                            break;
                        }
                        if (0 < iPosWordStart) {
                            replacement.PrefixWS = txtMatch.Substring(0, iPosWordStart);
                        }
                    }
                }
            }
            return result;
        }

        public CBAstNode Parse(List<CBToken> lstTokens) {
            var stack = new Stack<CBAstNode>();
            var root = new CBAstNode();
            var current = root;
            stack.Push(current);
            foreach (var token in lstTokens) {
                if (token.Kind == CBParserResultKind.Fixed) {
                    var replacementNode = new CBAstNode();
                    replacementNode.ContentToken = token;
                    current.Add(replacementNode);
                    continue;
                }
                if (token.Kind == CBParserResultKind.Replacement) {
                    if (!token.Start && !token.Finish) {
                        token.Start = true;
                        token.Finish = true;
                    }
                    if (token.Start && token.Finish) {
                        var replacementNode = new CBAstNode();
                        replacementNode.StartToken = token;
                        replacementNode.FinishToken = token;
                        current.Add(replacementNode);
                        continue;
                    }
                    if (token.Start && !token.Finish) {
                        var replacementNode = new CBAstNode();
                        replacementNode.StartToken = token;
                        current.Add(replacementNode);
                        current = replacementNode;
                        stack.Push(current);
                        continue;
                    }
                    if (!token.Start && token.Finish) {
                        if (current.StartToken is null) { throw new ArgumentException($"Current token:{token.Text} - no Start token"); }
                        if (!string.Equals(current.StartToken.Text, token.Text)) { throw new ArgumentException($"Current token:{token.Text} != Start token{current.StartToken?.Text}"); }
                        current.FinishToken = token;
                        stack.Pop();
                        current = stack.Peek();
                        continue;
                    }
                }
            }
            return root;
        }
    }
}
