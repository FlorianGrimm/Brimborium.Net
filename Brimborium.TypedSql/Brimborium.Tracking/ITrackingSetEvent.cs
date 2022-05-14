namespace Brimborium.Tracking;

public interface ITrackingSetEvent<TValue>
    where TValue : class {
    void OnAdding(
        TValue value,
        TrackingSet<TValue> trackingSet,
        TrackingContext trackingContext
        );
    void OnUpdating(
        TValue newValue,
        TValue oldValue,
        TrackingStatus oldTrackingStatus,
        TrackingSet<TValue> trackingSet,
        TrackingContext trackingContext
        );
    void OnDeleting(
        TValue newValue,
        TValue oldValue,
        TrackingStatus oldTrackingStatus,
        TrackingSet<TValue> trackingSet,
        TrackingContext trackingContext
        );
}
