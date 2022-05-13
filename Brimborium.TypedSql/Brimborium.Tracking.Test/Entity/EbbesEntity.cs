#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.


using Brimborium.Tracking.API;

namespace Brimborium.Tracking.Entity;

public record EbbesEntity(
    Guid Id,
    string Title,
    long EntityVersion = 0
    ) : RowVersion.Entity.IEntityWithVersion {
    public EbbesPK GetPrimaryKey() => new EbbesPK(this.Id);
}
