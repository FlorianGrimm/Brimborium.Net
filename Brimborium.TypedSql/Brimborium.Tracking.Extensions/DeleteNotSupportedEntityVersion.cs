namespace Brimborium.Tracking.Extensions;

public sealed class DeleteNotSupportedEntityVersion<TValue> : ITrackingSetEvent<TValue>
    where TValue : class, IEntityWithVersion {
    private static DeleteNotSupportedEntityVersion<TValue>? _Instance;
    public static DeleteNotSupportedEntityVersion<TValue> Instance => _Instance ??= new DeleteNotSupportedEntityVersion<TValue>();

    private DeleteNotSupportedEntityVersion() { }

    public void OnAdding(TValue value, TrackingSet<TValue> trackingSet, TrackingContext trackingContext) {
    }

    public void OnUpdating(TValue newValue, TValue oldValue, TrackingStatus oldTrackingStatus, TrackingSet<TValue> trackingSet, TrackingContext trackingContext) {
    }

    public void OnDeleting(TValue newValue, TValue oldValue, TrackingStatus oldTrackingStatus, TrackingSet<TValue> trackingSet, TrackingContext trackingContext) {
        throw new InvalidModificationException("Deleteing is not supported");
    }
}