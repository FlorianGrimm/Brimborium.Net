using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;

namespace Brimborium.CodeBlocks {
    public class CBParser {
        private static Regex? _RegexTemplate;

        public CBParser() {

        }
        public List<CBToken> Tokenize(string txtTemplate) {
            var result = new List<CBToken>();
            var regex = (_RegexTemplate ??= new Regex(
                "(?:/\\*)(?:(?<s1>\\<\\<)|(?<s2>--))\\s*(?<n>[^-/*<>]+)\\s*(?:(?<f2>--)|(?<f1>\\>\\>))(?:\\*/)",
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
                        current = stack.Pop();
                        continue;
                    }
                }
            }
            return root;
        }
    }

    public enum CBParserResultKind { Fixed, Replacement }
    public class CBToken : IEquatable<CBToken> {
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

        public override bool Equals(object? obj) {
            return base.Equals(obj as CBToken);
        }

        public bool Equals(CBToken? other) {
            if (ReferenceEquals(this, other)) { return true; }
            if (ReferenceEquals(null, other)) { return false; }
            return string.Equals(this.Text, other.Text)
                && this.Start == other.Start
                && this.Finish == other.Finish
                ;
        }
        public override int GetHashCode() {
            return this.Text.ToLower().GetHashCode();
        }
        public override string ToString() {
            if (this.Kind == CBParserResultKind.Fixed) {
                return this.Text;
            }
            if (this.Kind == CBParserResultKind.Replacement) {
                return $"/*{(this.Start ? "<<" : "--")} {this.Text} {(this.Finish ? ">>" : "--")}*/";
            }
            return "";
        }

    }

    public class CBAstNode {
        public CBAstNode() {
            this.Items = new List<CBAstNode>();
        }

        public List<CBAstNode> Items { get; }
        public CBToken? StartToken { get; set; }
        public CBToken? ContentToken { get; set; }
        public CBToken? FinishToken { get; set; }

        public void Add(CBAstNode node) {
            this.Items.Add(node);
        }
    }
}
