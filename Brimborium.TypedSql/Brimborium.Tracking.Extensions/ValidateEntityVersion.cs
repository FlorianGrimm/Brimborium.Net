namespace Brimborium.Tracking.Extensions;

public sealed class ValidateEntityVersion<TValue> : ITrackingSetEvent<TValue>
    where TValue : class, IEntityWithVersion {
    private static ValidateEntityVersion<TValue>? _Instance;
    public static ValidateEntityVersion<TValue> Instance => _Instance ??= new ValidateEntityVersion<TValue>();

    private ValidateEntityVersion() { }

    public void OnAdding(TValue value, TrackingSet<TValue> trackingSet, TrackingContext trackingContext) {
        if (value.EntityVersion != 0) {
            throw new InvalidModificationException("EntityVersion!=0");
        }
    }

    public void OnUpdating(TValue newValue, TValue oldValue, TrackingStatus oldTrackingStatus, TrackingSet<TValue> trackingSet, TrackingContext trackingContext) {
        if (!oldValue.EntityVersion.EntityVersionDoesMatch(newValue.EntityVersion)) {
            throw new InvalidModificationException("EntityVersion does not match");
        }
    }

    public void OnDeleting(TValue newValue, TValue oldValue, TrackingStatus oldTrackingStatus, TrackingSet<TValue> trackingSet, TrackingContext trackingContext) {
        if (!oldValue.EntityVersion.EntityVersionDoesMatch(newValue.EntityVersion)) {
            throw new InvalidModificationException("EntityVersion does not match");
        }
    }
}
