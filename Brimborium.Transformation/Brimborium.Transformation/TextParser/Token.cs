namespace Brimborium.Transformation.TextParser;

public record TextPosition(
    int Start,
    int Length,
    int Line,
    int Pos
);

public struct TextPositionMutable{
    public int Start;
    public int Length;
    public int Line;
    public int Pos;

    public TextPosition AsTextPosition()
        => new TextPosition(this.Start, this.Length, this.Line, this.Pos);
}

public interface IToken{
    TextPosition Position { get; }
}

public class Token{}