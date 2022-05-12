namespace Brimborium.Tracking;

public interface ITrackingSetEvent<TValue>
    where TValue : class {
    void Adding(
        TValue item,
        TrackingSet<TValue> trackingSet,
        TrackingContext trackingContext
        );
    void Updating(
        TValue item,
        TrackingSet<TValue> trackingSet,
        TrackingContext trackingContext
        );
    void Deleting(
        TValue item,
        TrackingSet<TValue> trackingSet,
        TrackingContext trackingContext
        );
}