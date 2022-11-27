namespace Brimborium.Transformation.TextParser;

public class TokenizerForText {
    public TokenizerForText() {
    }

    public TokenizerContextForText Scan(string value, string filename) {
        return new TokenizerContextForText(this, value.AsMemory(), filename);
    }

    public TokenizerContextForText Scan(ReadOnlyMemory<char> value, string filename) {
        return new TokenizerContextForText(this, value, filename);
    }
}

public class TokenizerContextForText {
    public TokenizerForText TokenizerForText { get; }
    public ReadOnlyMemory<char> Value { get; }
    public string Filename { get; }

    public TextPositionMutable Pos;

    public TokenizerContextForText(
        TokenizerForText tokenizerForText,
        ReadOnlyMemory<char> value,
        string filename) {
        this.TokenizerForText = tokenizerForText;
        this.Value = value;
        this.Filename = filename;
        this.Pos = new TextPositionMutable() {
            Start = -1,
            Length = 0,
            Line = 1,
            Pos = 1
        };
    }

    public bool Parse() {
        return false;
    }

    public bool Next(out char value) {
        if (this.Pos.Start < this.Value.Length) {
            this.Pos.Start++;
            this.Pos.Length++;
            if (this.Pos.Start < this.Value.Length) {
                value = this.Value.Span[this.Pos.Start];
                if (value == '\n') {
                    this.Pos.Line++;
                    this.Pos.Pos = 1;
                }
                return true;
            }
        }
        value = '\0';
        return false;
    }

    public void X() {
        this.Pos.Length = 0;
    }

}