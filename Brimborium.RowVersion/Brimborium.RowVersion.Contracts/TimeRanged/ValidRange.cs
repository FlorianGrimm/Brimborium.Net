namespace Brimborium.RowVersion.TimeRanged;

public interface IDTValidRange {
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}

public readonly struct DTValidRange : IDTValidRange {
    private readonly DateTime _ValidFrom;
    private readonly DateTime _ValidTo;

    public DTValidRange(
        DateTime validFrom,
        DateTime validTo
        ) {
        this._ValidFrom = validFrom;
        this._ValidTo = validTo;
    }
    public DateTime ValidFrom { get => this._ValidFrom; set => throw new NotSupportedException(); }
    public DateTime ValidTo { get => this._ValidTo; set => throw new NotSupportedException(); }
}

public interface IDTValidRangeQ {
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public readonly struct DTValidRangeQ : IDTValidRangeQ {
    private readonly DateTime _ValidFrom;
    private readonly DateTime? _ValidTo;

    public DTValidRangeQ(
        DateTime validFrom,
        DateTime? validTo
        ) {
        this._ValidFrom = validFrom;
        this._ValidTo = validTo;
    }
    public DateTime ValidFrom { get => this._ValidFrom; set => throw new NotSupportedException(); }
    public DateTime? ValidTo { get => this._ValidTo; set => throw new NotSupportedException(); }
}

public interface IDTOValidRange {
    public DateTimeOffset ValidFrom { get; set; }
    public DateTimeOffset ValidTo { get; set; }
}

public readonly struct DTOValidRange : IDTOValidRange {
    private readonly DateTimeOffset _ValidFrom;
    private readonly DateTimeOffset _ValidTo;

    public DTOValidRange(
        DateTimeOffset validFrom,
        DateTimeOffset validTo
        ) {
        this._ValidFrom = validFrom;
        this._ValidTo = validTo;
    }
    public DateTimeOffset ValidFrom { get => this._ValidFrom; set => throw new NotSupportedException(); }
    public DateTimeOffset ValidTo { get => this._ValidTo; set => throw new NotSupportedException(); }
}

public interface IDTOValidRangeQ {
    public DateTimeOffset ValidFrom { get; set; }
    public DateTimeOffset? ValidTo { get; set; }
}


public readonly struct DTOValidRangeQ : IDTOValidRangeQ {
    private readonly DateTimeOffset _ValidFrom;
    private readonly DateTimeOffset? _ValidTo;

    public DTOValidRangeQ(
        DateTimeOffset validFrom,
        DateTimeOffset? validTo
        ) {
        this._ValidFrom = validFrom;
        this._ValidTo = validTo;
    }
    public DateTimeOffset ValidFrom { get => this._ValidFrom; set => throw new NotSupportedException(); }
    public DateTimeOffset? ValidTo { get => this._ValidTo; set => throw new NotSupportedException(); }
}
