namespace Brimborium.Tracking.Extensions;

public sealed class DeleteNotSupportedEntityVersion<TValue> : ITrackingSetEvent<TValue>
    where TValue : class, IEntityWithVersion {
    private static DeleteNotSupportedEntityVersion<TValue>? _Instance;
    public static DeleteNotSupportedEntityVersion<TValue> Instance => _Instance ??= new DeleteNotSupportedEntityVersion<TValue>();

    private DeleteNotSupportedEntityVersion() { }

    //public void OnAdding(TValue value, TrackingSet<TValue> trackingSet, ITrackingContext trackingContext) {
    //}

    //public void OnUpdating(TValue newValue, TValue oldValue, TrackingStatus oldTrackingStatus, TrackingSet<TValue> trackingSet, ITrackingContext trackingContext) {
    //}

    //public void OnDeleting(TValue newValue, TValue oldValue, TrackingStatus oldTrackingStatus, TrackingSet<TValue> trackingSet, ITrackingContext trackingContext) {
    //    throw new InvalidModificationException("Deleteing is not supported");
    //}

    public AddingArgument<TValue> OnAdding(AddingArgument<TValue> argument) => argument;

    public UpdatingArgument<TValue> OnUpdating(UpdatingArgument<TValue> argument) => argument;

    public DeletingArgument<TValue> OnDeleting(DeletingArgument<TValue> argument) {
        throw new InvalidModificationException("Deleteing is not supported");
    }
}