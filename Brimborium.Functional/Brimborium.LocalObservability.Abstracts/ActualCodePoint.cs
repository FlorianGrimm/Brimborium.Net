namespace Brimborium.LocalObservability;

public class PolymorphCodePoint {
    public PolymorphCodePoint(LogEntryData entryData) {
        this.ListActualCodePoint = new List<IActualCodePoint>();
        this.EntryData = entryData;
    }

    public List<IActualCodePoint> ListActualCodePoint { get; }
    public LogEntryData EntryData { get; }
}

/// <summary>
/// Contains the actual code point and the values.
/// </summary>
public interface IActualCodePoint {
    CodePoint CodePoint { get; }

    IEnumerable<KeyValuePair<string, object>> Values { get; }
}

/// <summary>
/// Simple implementation of <see cref="IActualCodePoint"/>.
/// </summary>
public class ActualCodePoint : IActualCodePoint {
    public ActualCodePoint(
        CodePoint codePoint,
        IEnumerable<KeyValuePair<string, object>>? values
        ) {
        this.CodePoint = codePoint;
        this.Values = values ?? Array.Empty<KeyValuePair<string, object>>();
    }

    public CodePoint CodePoint { get; }
    public IEnumerable<KeyValuePair<string, object>> Values { get; }
}