namespace Brimborium.Tracking.Extensions.TestObject.Service;

public sealed class TrackingSetEbbes : TrackingSet<EbbesPK, EbbesEntity> {
    public TrackingSetEbbes(
        DBContext trackingContext,
        ITrackingSetApplyChanges<EbbesEntity>? trackingApplyChanges = default
        ) : base(
            extractKey: EbbesUtiltiy.Instance,
            comparer: EbbesUtiltiy.Instance,
            trackingContext: trackingContext,
            trackingApplyChanges: trackingApplyChanges ?? TrackingSetApplyChangesEbbes.Instance) {
        this.TrackingSetEvents.Add(ValidateEntityVersion<EbbesEntity>.Instance);
    }
}

public sealed class TrackingSetApplyChangesEbbes
    : ITrackingSetApplyChanges<EbbesEntity> {
    private static TrackingSetApplyChangesEbbes? _Instance;
    public static TrackingSetApplyChangesEbbes Instance => _Instance ??= new TrackingSetApplyChangesEbbes();

    public long EntityVersion;
    private TrackingSetApplyChangesEbbes(){
        this.EntityVersion = 1;
    }

    public Task<EbbesEntity> Insert(EbbesEntity value, ITrackingTransConnection trackingTransaction) {
        if (value.Title == "fail") {
            throw new InvalidModificationException("fail");
        }
        value = value with { EntityVersion = this.EntityVersion++ };
        return Task.FromResult(value);
    }

    public Task<EbbesEntity> Update(EbbesEntity value, ITrackingTransConnection trackingTransaction) {
        if (value.Title == "fail") {
            throw new InvalidModificationException("fail");
        }
        value = value with { EntityVersion = this.EntityVersion++ };
        return Task.FromResult(value);
    }

    public Task Delete(EbbesEntity value, ITrackingTransConnection trackingTransaction) {
        if (value.Title == "fail") {
            throw new InvalidModificationException("fail");
        }
        return Task.CompletedTask;
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

