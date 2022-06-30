namespace Brimborium.Tracking.Extensions;

public sealed class DeleteNotSupportedEntityVersion<TKey, TValue> : ITrackingSetEvent<TKey, TValue>
    where TKey : notnull
    where TValue : class, IEntityWithVersion {
    private static DeleteNotSupportedEntityVersion<TKey, TValue>? _Instance;
    public static DeleteNotSupportedEntityVersion<TKey, TValue> Instance => _Instance ??= new DeleteNotSupportedEntityVersion<TKey, TValue>();

    private DeleteNotSupportedEntityVersion() { }

    //public void OnAdding(TValue value, TrackingSet<TValue> trackingSet, ITrackingContext trackingContext) {
    //}

    //public void OnUpdating(TValue newValue, TValue oldValue, TrackingStatus oldTrackingStatus, TrackingSet<TValue> trackingSet, ITrackingContext trackingContext) {
    //}

    //public void OnDeleting(TValue newValue, TValue oldValue, TrackingStatus oldTrackingStatus, TrackingSet<TValue> trackingSet, ITrackingContext trackingContext) {
    //    throw new InvalidModificationException("Deleteing is not supported");
    //}

    public AddingArgument<TKey, TValue> OnAdding(AddingArgument<TKey, TValue> argument) => argument;

    public UpdatingArgument<TKey, TValue> OnUpdating(UpdatingArgument<TKey, TValue> argument) => argument;

    public DeletingArgument<TKey, TValue> OnDeleting(DeletingArgument<TKey, TValue> argument) {
        throw new InvalidModificationException("Deleteing is not supported");
    }
}