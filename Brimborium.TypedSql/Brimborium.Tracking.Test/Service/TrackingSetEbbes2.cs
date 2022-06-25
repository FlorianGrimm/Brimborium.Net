using Brimborium.Tracking.API;
using Brimborium.Tracking.Entity;

namespace Brimborium.Tracking.Service;

public sealed class TrackingSetEbbes2 : TrackingSet<EbbesPK, EbbesEntity>, ITrackingSetEbbes {
    public TrackingSetEbbes2(
        Test1TrackingContext trackingContext,
        ITrackingSetApplyChanges<EbbesEntity>? trackingApplyChanges = default
        ) : base(
            extractKey: EbbesUtiltiy.Instance,
            comparer: EbbesUtiltiy.Instance,
            trackingContext: trackingContext,
            trackingApplyChanges: trackingApplyChanges ?? TrackingSetApplyChangesEbbes2.Instance) {
        this.TrackingSetEvents.Add(ValidateTrackingSetEbbes2.Instance);
    }
}

public sealed class TrackingSetApplyChangesEbbes2
    : TrackingSetApplyChangesBase<EbbesEntity, EbbesPK> {
    private static TrackingSetApplyChangesEbbes2? _Instance;
    public static TrackingSetApplyChangesEbbes2 Instance => _Instance ??= new TrackingSetApplyChangesEbbes2();

    public long EntityVersion;
    private TrackingSetApplyChangesEbbes2()
        : base(EbbesUtiltiy.Instance) {
        this.EntityVersion = 1;
    }

    public override Task<EbbesEntity> Insert(EbbesEntity value, ITrackingTransConnection trackingTransaction) {
        if (value.Title == "fail") {
            throw new InvalidModificationException("Insert");
        }
        value = value with { EntityVersion = this.EntityVersion++ };
        return Task.FromResult(value);
    }

    public override Task<EbbesEntity> Update(EbbesEntity value, ITrackingTransConnection trackingTransaction) {
        if (value.Title == "fail") {
            throw new InvalidModificationException("Update");
        }
        value = value with { EntityVersion = this.EntityVersion++ };
        return Task.FromResult(value);
    }

    public override Task Delete(EbbesEntity value, ITrackingTransConnection trackingTransaction) {
        if (value.Title == "fail") {
            throw new InvalidModificationException("Delete");
        }
        return Task.CompletedTask;
    }
}

public class ValidateTrackingSetEbbes2 : ValidateBaseTrackingSet<EbbesEntity> {
    private static ValidateTrackingSetEbbes2? _Instance;
    public static ValidateTrackingSetEbbes2 Instance => _Instance ??= new ValidateTrackingSetEbbes2();

    protected override void Validate(EbbesEntity value, ITrackingContext trackingContext, ITrackingSet<EbbesEntity> trackingSet) {
        if (string.IsNullOrEmpty(value.Title)) {
            throw new ValidationFailedException(nameof(value.Title));
        }
        if (value.Id == Guid.Empty) {
            throw new ValidationFailedException(nameof(value.Id));
        }
    }

    public override AddingArgument<EbbesEntity> OnAdding(AddingArgument<EbbesEntity> argument) {
        var (value, trackingSetBase, _) = argument;
        var trackingSet = (ITrackingSetEbbes) trackingSetBase;
        if (value.Id == Guid.Empty) {
            argument = argument with { Value = value with { Id = Guid.NewGuid() } };
        }
        return base.OnAdding(argument);
    }
}