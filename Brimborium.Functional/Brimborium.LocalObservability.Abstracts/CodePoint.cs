namespace Brimborium.LocalObservability;

/// <summary>
/// The code point is a point in the code where a log entry is created.
/// </summary>
public class CodePoint {
    /// <summary>
    /// Gets the name of the code point.
    /// </summary>
    public string Name;

    /// <summary>
    /// Gets the description of the code point.
    /// </summary>
    public string? Description;

    /// <summary>
    /// Gets the event id of the code point.
    /// </summary>
    public int EventId;

/// <summary>
/// Creates a new instance of the <see cref="CodePoint"/> class.
/// </summary>
/// <param name="name">The name can be derived from a log entry.</param>
/// <param name="description"></param>
/// <param name="eventId">If derived from a log entry it's EventId</param>
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

