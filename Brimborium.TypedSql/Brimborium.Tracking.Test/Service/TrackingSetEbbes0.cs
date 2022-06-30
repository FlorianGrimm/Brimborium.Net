namespace Brimborium.Tracking.Service;

public sealed class TrackingSetEbbes0 : TrackingSet<EbbesPK, EbbesEntity>, ITrackingSetEbbes {
    public TrackingSetEbbes0(
        Test1TrackingContext trackingContext,
        ITrackingSetApplyChanges<EbbesPK, EbbesEntity>? trackingApplyChanges = default
        ) : base(
            extractKey: EbbesUtiltiy.Instance,
            comparer: EbbesUtiltiy.Instance,
            trackingContext: trackingContext,
            trackingApplyChanges: trackingApplyChanges ?? TrackingSetApplyChangesEbbes.Instance) {
    }
}

public sealed class TrackingSetApplyChangesEbbes
    : TrackingSetApplyChangesBase<EbbesPK, EbbesEntity> {
    private static TrackingSetApplyChangesEbbes? _Instance;
    public static TrackingSetApplyChangesEbbes Instance => _Instance ??= new TrackingSetApplyChangesEbbes();

    public long EntityVersion;
    private TrackingSetApplyChangesEbbes()
        : base(EbbesUtiltiy.Instance) {
        this.EntityVersion = 1;
    }

    public override Task<EbbesEntity> Insert(TrackingObject<EbbesPK, EbbesEntity> to, ITrackingTransConnection trackingTransaction) {
        var value = to.Value;
        if (value.Title == "fail") {
            throw new InvalidModificationException("Insert");
        }
        value = value with { EntityVersion = this.EntityVersion++ };
        return Task.FromResult(value);
    }

    public override Task<EbbesEntity> Update(TrackingObject<EbbesPK, EbbesEntity> to, ITrackingTransConnection trackingTransaction) {
        var value = to.Value;
        if (value.Title == "fail") {
            throw new InvalidModificationException("Update");
        }
        value = value with { EntityVersion = this.EntityVersion++ };
        return Task.FromResult(value);
    }

    public override Task Delete(TrackingObject<EbbesPK, EbbesEntity> to, ITrackingTransConnection trackingTransaction) {
        var value = to.Value;
        if (value.Title == "fail") {
            throw new InvalidModificationException("Delete");
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
        throw new NotImplementedException();
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

