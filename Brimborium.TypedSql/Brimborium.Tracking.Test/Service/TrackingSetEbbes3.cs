using Brimborium.Tracking.API;
using Brimborium.Tracking.Entity;

namespace Brimborium.Tracking.Service;

public sealed class TrackingSetEbbes3 : TrackingSet<EbbesPK, EbbesEntity>, ITrackingSetEbbes {
    public TrackingSetEbbes3(
        Test1TrackingContext trackingContext,
        ITrackingSetApplyChanges<EbbesPK, EbbesEntity>? trackingApplyChanges = default
        ) : base(
            extractKey: EbbesUtiltiy.Instance,
            comparer: EbbesUtiltiy.Instance,
            trackingContext: trackingContext,
            trackingApplyChanges: trackingApplyChanges ?? TrackingSetApplyChangesEbbes3.Instance) {
        this.TrackingSetEvents.Add(ValidateTrackingSetEbbes3.Instance);
    }

    public int CalledAttachConflict = 0;

    public override void AttachValidate(EbbesEntity item) {
        base.AttachValidate(item);
        this.CalledAttachConflict++;
    }

    public int CalledAttachConflictReplace = 0;

    public override bool AttachConflictReplace(EbbesEntity item, TrackingObject<EbbesPK, EbbesEntity> found) {
        this.CalledAttachConflictReplace++;
        return true;
    }
}

public sealed class TrackingSetApplyChangesEbbes3
    : TrackingSetApplyChangesBase<EbbesPK, EbbesEntity> {
    private static TrackingSetApplyChangesEbbes3? _Instance;
    public static TrackingSetApplyChangesEbbes3 Instance => _Instance ??= new TrackingSetApplyChangesEbbes3();

    public long EntityVersion;
    private TrackingSetApplyChangesEbbes3()
        : base(EbbesUtiltiy.Instance) {
        this.EntityVersion = 1;
    }

    public override Task<EbbesEntity> Insert(TrackingObject<EbbesPK, EbbesEntity> to, ITrackingTransConnection trackingTransaction) {
        var value = to.Value;
        if (value.Title == "fail") {
            throw new InvalidModificationException("Insert");
        }
        if (value.Id == new Guid("00000000-0000-0000-0000-000000000001")) {
            value = value with { Id = Guid.NewGuid() };
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

public class ValidateTrackingSetEbbes3 : ValidateBaseTrackingSet<EbbesPK, EbbesEntity> {
    private static ValidateTrackingSetEbbes3? _Instance;
    public static ValidateTrackingSetEbbes3 Instance => _Instance ??= new ValidateTrackingSetEbbes3();

    protected override void Validate(EbbesEntity value, ITrackingContext trackingContext, ITrackingSet<EbbesPK, EbbesEntity> trackingSet) {
        if (string.IsNullOrEmpty(value.Title)) {
            throw new ValidationFailedException(nameof(value.Title));
        }
        //if (value.Id == Guid.Empty) {
        //    throw new ValidationFailedException(nameof(value.Id));
        //}
    }
}