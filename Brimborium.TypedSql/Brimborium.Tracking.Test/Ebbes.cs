#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.

namespace Brimborium.Tracking.Test;

public record Ebbes(
    Guid Id,
    string Title,
    string DataVersion = ""
    ) : Brimborium.RowVersion.API.IDataAPIWithVersion {
    public EbbesPK GetPrimaryKey() => new EbbesPK(this.Id);
}

public record EbbesPK(
    Guid Id
);

public record EbbesEntity(
    Guid Id,
    string Title,
    long EntityVersion = 0
    ) : Brimborium.RowVersion.Entity.IEntityWithVersion {
    public EbbesPK GetPrimaryKey() => new EbbesPK(this.Id);
}
