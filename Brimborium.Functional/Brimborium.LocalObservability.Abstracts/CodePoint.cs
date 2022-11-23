﻿namespace Brimborium.LocalObservability;

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
public class ActualCodePoint {
    public ActualCodePoint(
        CodePoint codePoint,
        IEnumerable<KeyValuePair<string, object>> values
        ) {
        this.CodePoint = codePoint;
    }

    public CodePoint CodePoint { get; }
}