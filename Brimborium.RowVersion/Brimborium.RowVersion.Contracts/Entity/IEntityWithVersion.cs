namespace Brimborium.RowVersion.Entity;

public interface IEntityWithVersion {
    long EntityVersion { get; }
}
