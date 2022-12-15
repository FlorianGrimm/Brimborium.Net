namespace Brimborium.LocalObservability;

/// <summary>
/// Contains the actual code point and the values.
/// </summary>
public interface IActualCodePoint {
    CodePoint CodePoint { get; }

    IEnumerable<KeyValuePair<string, object>> Values { get; }
    ActualPolymorphCodePoint? Container { get; set; }
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
    public ActualPolymorphCodePoint? Container { get; set; }
}