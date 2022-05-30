namespace Brimborium.Tracking;

public interface ITrackingSetEvent<TValue>
    where TValue : class {
    void OnAdding(
        TValue value,
        TrackingSet<TValue> trackingSet,
        ITrackingContext trackingContext
        );
    void OnUpdating(
        TValue newValue,
        TValue oldValue,
        TrackingStatus oldTrackingStatus,
        TrackingSet<TValue> trackingSet,
        ITrackingContext trackingContext
        );
    void OnDeleting(
        TValue newValue,
        TValue oldValue,
        TrackingStatus oldTrackingStatus,
        TrackingSet<TValue> trackingSet,
        ITrackingContext trackingContext
        );
}
