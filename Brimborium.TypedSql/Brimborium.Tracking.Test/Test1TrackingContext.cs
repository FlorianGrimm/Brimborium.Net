#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.

namespace Brimborium.Tracking;

public class Test1TrackingContext : TrackingContext {
    public Test1TrackingContext(
        ITrackingSet<EbbesPK, EbbesEntity>? ebbes = default
    ) {
        this.Ebbes = ebbes ?? new TrackingSetEbbes(
            trackingContext: this
            );
    }
    public ITrackingSet<EbbesPK, EbbesEntity> Ebbes { get; }
}
