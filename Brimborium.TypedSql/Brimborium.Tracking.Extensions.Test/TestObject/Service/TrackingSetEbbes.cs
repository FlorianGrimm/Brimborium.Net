namespace Brimborium.Tracking.Extensions.TestObject.Service;

public sealed class TrackingSetEbbes : TrackingSet<EbbesPK, EbbesEntity> {
    public TrackingSetEbbes(
        DBContext trackingContext,
        ITrackingSetApplyChanges<EbbesPK, EbbesEntity>? trackingApplyChanges = default
        ) : base(
            extractKey: EbbesUtiltiy.Instance,
            comparer: EbbesUtiltiy.Instance,
            trackingContext: trackingContext,
            trackingApplyChanges: trackingApplyChanges ?? TrackingSetApplyChangesEbbes.Instance) {
        this.TrackingSetEvents.Add(ValidateEntityVersion<EbbesPK, EbbesEntity>.Instance);
    }
}

public sealed class TrackingSetApplyChangesEbbes
    : ITrackingSetApplyChanges<EbbesPK, EbbesEntity> {
    private static TrackingSetApplyChangesEbbes? _Instance;
    public static TrackingSetApplyChangesEbbes Instance => _Instance ??= new TrackingSetApplyChangesEbbes();

    public long EntityVersion;
    private TrackingSetApplyChangesEbbes(){
        this.EntityVersion = 1;
    }

    public Task<EbbesEntity> Insert(TrackingObject<EbbesPK, EbbesEntity> to, ITrackingTransConnection trackingTransaction) {
        var value = to.Value;
        if (value.Title == "fail") {
            throw new InvalidModificationException("fail");
        }
        value = value with { EntityVersion = this.EntityVersion++ };
        return Task.FromResult(value);
    }

    public Task<EbbesEntity> Update(TrackingObject<EbbesPK, EbbesEntity> to, ITrackingTransConnection trackingTransaction) {
        var value = to.Value;
        if (value.Title == "fail") {
            throw new InvalidModificationException("fail");
        }
        value = value with { EntityVersion = this.EntityVersion++ };
        return Task.FromResult(value);
    }

    public Task Delete(TrackingObject<EbbesPK, EbbesEntity> to, ITrackingTransConnection trackingTransaction) {
        var value = to.Value;
        if (value.Title == "fail") {
            throw new InvalidModificationException("fail");
        }
        return Task.CompletedTask;
    }
}


public sealed class EbbesUtiltiy
    : IEqualityComparer<EbbesPK>
    , IExtractKey<EbbesPK, EbbesEntity> {
    private static EbbesUtiltiy? _Instance;
    public static EbbesUtiltiy Instance => _Instance ??= new EbbesUtiltiy();
    private EbbesUtiltiy() { }

    public EbbesPK ExtractKey(EbbesEntity that) => that.GetPrimaryKey();
    
    public bool TryExtractKey(EbbesEntity value, [MaybeNullWhen(false)] out EbbesPK key) {
        key = value.GetPrimaryKey();
        return key.Id != Guid.Empty;
    }

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

