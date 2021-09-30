using System;

namespace Brimborium.CodeBlocks {
    public sealed class CBToken : IEquatable<CBToken> {
        public static CBToken Fixed(string value) => new CBToken() { Kind = CBParserResultKind.Fixed, Text = value };
        public static CBToken Replacement(string value) => new CBToken() { Kind = CBParserResultKind.Replacement, Text = value };
        public CBToken() {
            this.PrefixWS = string.Empty;
            this.IndentWS = string.Empty;
            this.Text = string.Empty;
            this.Name = string.Empty;
        }
        public CBParserResultKind Kind { get; set; }
        public string PrefixWS { get; set; }
        public string IndentWS { get; set; }
        public string Text { get; set; }
        public string Name { get; set; }
        public bool Start { get; set; }
        public bool Finish { get; set; }

        public override bool Equals(object? obj) {
            return base.Equals(obj as CBToken);
        }

        public bool Equals(CBToken? other) {
            if (ReferenceEquals(this, other)) { return true; }
            if (other is null) { return false; }

            return (string.Equals(this.Text, other.Text))
                && (string.Equals(this.Name, other.Name))
                && (this.Start == other.Start)
                && (this.Finish == other.Finish)
                ;
        }
        public override int GetHashCode() {
            return this.Text.ToLower().GetHashCode()
                ^ this.Name.ToLower().GetHashCode();
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

    public enum CBParserResultKind { Fixed, Replacement }
}
