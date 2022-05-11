#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.

namespace Brimborium.Tracking.Test;

public class Test1TrackingContext : TrackingContext {
    public Test1TrackingContext() {
        this.Ebbes = new TrackingSet<EbbesPK, EbbesEntity>(
            extractKey: EbbesUtiltiy.Instance,
            comparer: EbbesUtiltiy.Instance,
            trackingContext: this,
            trackingApplyChanges: null!
            );
    }
    public TrackingSet<EbbesPK, EbbesEntity> Ebbes { get; }
}
