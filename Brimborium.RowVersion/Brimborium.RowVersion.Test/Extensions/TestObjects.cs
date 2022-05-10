namespace Brimborium.RowVersion.Extensions;

class TestEntityWithVersion : IEntityWithVersion {
    public TestEntityWithVersion() {
    }
    public TestEntityWithVersion(long entityVersion) {
        this.EntityVersion = entityVersion;
    }

    public long EntityVersion { get; set; }
}
class DTTestValidRange : IDTValidRange {
    public DTTestValidRange() {
        this.ValidFrom = DateTime.MinValue;
        this.ValidTo = DateTime.MaxValue;
    }
    public DTTestValidRange(
        DateTime validFrom,
        DateTime validTo
        ) {
        this.ValidFrom = validFrom;
        this.ValidTo = validTo;
    }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}

class DTTestValidRangeQ : IDTValidRangeQ {
    public DTTestValidRangeQ() {
        this.ValidFrom = DateTime.MinValue;
    }

    public DTTestValidRangeQ(
        DateTime validFrom,
        DateTime? validTo
        ) {
        this.ValidFrom = validFrom;
        this.ValidTo = validTo;
    }

    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}


class DTOTestValidRange : IDTOValidRange {
    public DTOTestValidRange() {
        this.ValidFrom = DateTimeOffset.MinValue;
        this.ValidTo = DateTimeOffset.MaxValue;
    }
    public DTOTestValidRange(
        DateTimeOffset validFrom,
        DateTimeOffset validTo
        ) {
        this.ValidFrom = validFrom;
        this.ValidTo = validTo;
    }
    public DateTimeOffset ValidFrom { get; set; }
    public DateTimeOffset ValidTo { get; set; }
}

class DTOTestValidRangeQ : IDTOValidRangeQ {
    public DTOTestValidRangeQ() {
        this.ValidFrom = DateTimeOffset.MinValue;
    }

    public DTOTestValidRangeQ(
        DateTimeOffset validFrom,
        DateTimeOffset? validTo
        ) {
        this.ValidFrom = validFrom;
        this.ValidTo = validTo;
    }
    public DateTimeOffset ValidFrom { get; set; }
    public DateTimeOffset? ValidTo { get; set; }
}
