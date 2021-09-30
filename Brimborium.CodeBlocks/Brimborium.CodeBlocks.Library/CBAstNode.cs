using System;
using System.Collections.Generic;

namespace Brimborium.CodeBlocks {
    public sealed class CBAstNode {
        public static CBAstNode? _Empty;
        public static CBAstNode Empty => (_Empty ??= new CBAstNode());

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
        public override string ToString() {
            if (this.StartToken is null && this.ContentToken is not null && this.FinishToken is null) {
                return this.ContentToken.ToString();
            }
            var s = (this.StartToken?.ToString() ?? string.Empty) ;
            var c = (this.ContentToken?.ToString() ?? string.Empty);
            var f = (this.FinishToken?.ToString() ?? string.Empty);
            return $"#{this.Items.Count} {s}{c}{f}";
        }

        public CBAstNodeKind GetKind() {
            if ((this.StartToken is not null) && (this.ContentToken is null) && (this.FinishToken is not null) && ((this.Items.Count >= 0))) {
                return CBAstNodeKind.Replacement;
            }
            if ((this.StartToken is null) && (this.ContentToken is not null) && (this.FinishToken is null) && ((this.Items.Count == 0))) {
                return CBAstNodeKind.Content ;
            }
            if ((this.StartToken is null) && (this.ContentToken is null) && (this.FinishToken is null) && ((this.Items.Count >= 0))) {
                return CBAstNodeKind.Items;
            }
            return CBAstNodeKind.Faulted;
        }
    }

    public enum CBAstNodeKind { Replacement, Content, Items, Faulted }

    
    public sealed class CBAstNodeTextName : IComparer<CBAstNode> {
        private static CBAstNodeTextName? _Instance;
        public static CBAstNodeTextName GetInstance() => _Instance ??= new CBAstNodeTextName();

        public int Compare(CBAstNode? x, CBAstNode? y) {
            if (ReferenceEquals(x, y)) { return 0; }
            if (x?.StartToken is null && y?.StartToken is null) { return 0; }
            if (x is null || x.StartToken is null) { return -1; }
            if (y is null || y.StartToken is null) { return +1; }

            if (x.StartToken.Kind == CBParserResultKind.Replacement
                && y.StartToken.Kind == CBParserResultKind.Replacement
                ) {
                var result = StringComparer.OrdinalIgnoreCase.Compare(x.StartToken.Text, y.StartToken.Text);
                if (result == 0) { 
                    result = StringComparer.OrdinalIgnoreCase.Compare(x.StartToken.Name, y.StartToken.Name);
                }
                return result;
            } else {
                return 0;
            }
        }
    }
}
