namespace Brimborium.LocalObservability;

public class CodePoint {
    public string Name;
    public string? Description;
    public int EventId;

    public CodePoint(
        string? name = default,
        string? description = default,
        int eventId = 1
        ) {
        this.Name = name ?? string.Empty;
        this.Description = description;
        this.EventId = eventId;
    }

    public override string ToString() {
        return Name ?? string.Empty;
    }
}

public interface IActualCodePoint {
    CodePoint CodePoint { get; }

    IEnumerable<KeyValuePair<string, object>> GetValues();
}

public class ActualCodePoint : IActualCodePoint {
    private readonly IEnumerable<KeyValuePair<string, object>> _Values;
    public ActualCodePoint(
        CodePoint codePoint,
        IEnumerable<KeyValuePair<string, object>> values
        ) {
        this.CodePoint = codePoint;
        this._Values = values;
    }
    public IEnumerable<KeyValuePair<string, object>> GetValues() => this._Values;

    public CodePoint CodePoint { get; }
}