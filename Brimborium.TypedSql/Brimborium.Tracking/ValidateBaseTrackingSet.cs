namespace Brimborium.Tracking;

public class ValidateBaseTrackingSet<TValue> : ITrackingSetEvent<TValue>
    where TValue : class {

    public virtual void OnAdding(TValue value, TrackingSet<TValue> trackingSet, TrackingContext trackingContext) {
        this.Validate(value);
    }
    public virtual void OnUpdating(TValue newValue, TValue oldValue, TrackingStatus oldTrackingStatus, TrackingSet<TValue> trackingSet, TrackingContext trackingContext) {
        this.Validate(newValue);
    }

    public virtual void OnDeleting(TValue newValue, TValue oldValue, TrackingStatus oldTrackingStatus, TrackingSet<TValue> trackingSet, TrackingContext trackingContext) {
        this.Validate(newValue);
    }

    protected virtual void Validate(TValue value) {
    }
}
