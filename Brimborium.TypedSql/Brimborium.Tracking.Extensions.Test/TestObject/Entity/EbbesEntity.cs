#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.


using Brimborium.Tracking.Extensions.TestObject.API;

namespace Brimborium.Tracking.Extensions.TestObject.Entity;

public record EbbesEntity(
    Guid Id,
    string Title,
    long EntityVersion = 0
    ) : IEntityWithVersion {
    public EbbesPK GetPrimaryKey() => new EbbesPK(this.Id);
}
