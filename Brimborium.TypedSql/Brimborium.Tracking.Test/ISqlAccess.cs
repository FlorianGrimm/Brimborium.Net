namespace Brimborium.Tracking.Test;

public interface ISqlAccess {
    Task<IDataManipulationResult<EbbesEntity>> ExecuteEbbesUpsertAsync(EbbesEntity value);
    Task<List<EbbesPK>> ExecuteEbbesDeletePKAsync(EbbesEntity value);
}