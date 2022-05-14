#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.


namespace Brimborium.Tracking.Extensions.TestObject.API;

public record Ebbes(
    Guid Id,
    string Title,
    string DataVersion = ""
    ) : RowVersion.API.IDataAPIWithVersion {
    public EbbesPK GetPrimaryKey() => new EbbesPK(this.Id);
}

public record EbbesPK(
    Guid Id
);
