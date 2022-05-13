using Brimborium.Tracking.API;
using Brimborium.Tracking.Entity;

namespace Brimborium.Tracking.Service;

public interface ISqlAccess {
    Task<IDataManipulationResult<EbbesEntity>> ExecuteEbbesUpsertAsync(EbbesEntity value);
    Task<List<EbbesPK>> ExecuteEbbesDeletePKAsync(EbbesEntity value);
}