namespace Brimborium.Tracking;

public sealed class TrackingSetEbbes : TrackingSet<EbbesPK, EbbesEntity> {
    public TrackingSetEbbes(
        Test1TrackingContext trackingContext,
        ITrackingSetApplyChanges<EbbesEntity>? trackingApplyChanges = default
        ) : base(
            extractKey: EbbesUtiltiy.Instance,
            comparer: EbbesUtiltiy.Instance,
            trackingContext: trackingContext,
            trackingApplyChanges: trackingApplyChanges ?? TrackingSetApplyChangesEbbes.Instance) {

    }
}

public sealed class TrackingSetApplyChangesEbbes
    : TrackingSetApplyChangesBase<EbbesEntity, EbbesPK> {
    private static TrackingSetApplyChangesEbbes? _Instance;
    public static TrackingSetApplyChangesEbbes Instance => _Instance ??= new TrackingSetApplyChangesEbbes();

    private TrackingSetApplyChangesEbbes() 
        : base(EbbesUtiltiy.Instance) { 
        }

    // protected override EbbesPK ExtractKey(EbbesEntity value) => value.GetPrimaryKey();

    public override Task<EbbesEntity> Insert(EbbesEntity value, ITrackingTransConnection trackingTransaction) {
        return this.Upsert(value, trackingTransaction);
    }

    public override Task<EbbesEntity> Update(EbbesEntity value, ITrackingTransConnection trackingTransaction) {
        return this.Upsert(value, trackingTransaction);
    }

    private async Task<EbbesEntity> Upsert(EbbesEntity value, ITrackingTransConnection trackingTransaction) {
        var sqlAccess = (ISqlAccess)trackingTransaction;
        var result = await sqlAccess.ExecuteEbbesUpsertAsync(value);
        return this.ValidateUpsertDataManipulationResult(value, result);
    }

    public override async Task Delete(EbbesEntity value, ITrackingTransConnection trackingTransaction) {
        var sqlAccess = (ISqlAccess)trackingTransaction;
        var result = await sqlAccess.ExecuteEbbesDeletePKAsync(value);
        this.ValidateDelete(value, result);
    }
}


public sealed class EbbesUtiltiy
    : IEqualityComparer<EbbesPK>
    , IExtractKey<EbbesEntity, EbbesPK> {
    private static EbbesUtiltiy? _Instance;
    public static EbbesUtiltiy Instance => _Instance ??= new EbbesUtiltiy();
    private EbbesUtiltiy() { }

    public EbbesPK ExtractKey(EbbesEntity that) => that.GetPrimaryKey();

    bool IEqualityComparer<EbbesPK>.Equals(EbbesPK? x, EbbesPK? y) {
        if (ReferenceEquals(x, y)) {
            return true;
        } else if (x is null || y is null) {
            return false;
        } else {
            return x.Equals(y);
        }
    }

    int IEqualityComparer<EbbesPK>.GetHashCode(EbbesPK obj) {
        return obj.GetHashCode();
    }
}
