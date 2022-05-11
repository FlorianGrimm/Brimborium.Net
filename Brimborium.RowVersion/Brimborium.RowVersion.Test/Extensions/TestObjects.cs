namespace Brimborium.RowVersion.Extensions;

[ExcludeFromCodeCoverage]
public class TestEntityWithVersion : IEntityWithVersion {
    public TestEntityWithVersion() {
    }
    public TestEntityWithVersion(long entityVersion) {
        this.EntityVersion = entityVersion;
    }

    public long EntityVersion { get; set; }
}

[ExcludeFromCodeCoverage]
public class DTTestValidRange 
    : IDTValidRange
    , IEntityWithVersion {
    public DTTestValidRange() {
        this.ValidFrom = DateTime.MinValue;
        this.ValidTo = DateTime.MaxValue;
    }
    public DTTestValidRange(
        DateTime validFrom,
        DateTime validTo,
        long entityVersion
        ) {
        this.ValidFrom = validFrom;
        this.ValidTo = validTo;
        this.EntityVersion = entityVersion;
    }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public long EntityVersion { get; set; }
}

[ExcludeFromCodeCoverage]
public class DTTestValidRangeQ 
    : IDTValidRangeQ
    , IEntityWithVersion {
    public DTTestValidRangeQ() {
        this.ValidFrom = DateTime.MinValue;
    }

    public DTTestValidRangeQ(
        DateTime validFrom,
        DateTime? validTo,
        long entityVersion
        ) {
        this.ValidFrom = validFrom;
        this.ValidTo = validTo;
        this.EntityVersion = entityVersion;
    }

    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public long EntityVersion { get; set; }
}

[ExcludeFromCodeCoverage]
public class DTOTestValidRange
    : IDTOValidRange
    , IEntityWithVersion {
    public DTOTestValidRange() {
        this.ValidFrom = DateTimeOffset.MinValue;
        this.ValidTo = DateTimeOffset.MaxValue;
    }
    public DTOTestValidRange(
        DateTimeOffset validFrom,
        DateTimeOffset validTo,
        long entityVersion
        ) {
        this.ValidFrom = validFrom;
        this.ValidTo = validTo;
        this.EntityVersion = entityVersion;
    }
    public DateTimeOffset ValidFrom { get; set; }
    public DateTimeOffset ValidTo { get; set; }
    public long EntityVersion { get; set; }
}

[ExcludeFromCodeCoverage]
public class DTOTestValidRangeQ 
    : IDTOValidRangeQ
    , IEntityWithVersion {
    public DTOTestValidRangeQ() {
        this.ValidFrom = DateTimeOffset.MinValue;
    }

    public DTOTestValidRangeQ(
        DateTimeOffset validFrom,
        DateTimeOffset? validTo,
        long entityVersion
        ) {
        this.ValidFrom = validFrom;
        this.ValidTo = validTo;
        this.EntityVersion = entityVersion;
    }
    public DateTimeOffset ValidFrom { get; set; }
    public DateTimeOffset? ValidTo { get; set; }
    public long EntityVersion { get; set; }
}
