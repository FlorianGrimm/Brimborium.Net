namespace Brimborium.LocalObservability;

/// <summary>
/// The code point is a point in the code where a log entry is created.
/// </summary>
[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public class CodePoint {
    public readonly EventId Event;
    
    /// <summary>
    /// Gets the name of the code point.
    /// </summary>
    public string Name => this.Event.Name ?? string.Empty;

    /// <summary>
    /// Gets the description of the code point.
    /// </summary>
    public string? Description;

    /// <summary>
    /// Gets the event id of the code point.
    /// </summary>
    public int EventId => this.Event.Id;

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
        this.Event = new EventId(eventId, name ?? string.Empty);
        this.Description = description;
    }

    public CodePoint(
        EventId eventId,
        string? description = default
        ) {
        this.Event = eventId;
        this.Description = description;
    }

    public override string ToString() => this.Event.ToString();
}
