namespace Brimborium.Tracking;

public class ValidateBaseTrackingSet<TKey, TValue> : ITrackingSetEvent<TKey, TValue>
    where TKey : notnull
    where TValue : class {

    //public virtual void OnAdding(TValue value, TrackingSet<TValue> trackingSet, ITrackingContext trackingContext) {
    //    this.Validate(value);
    //}
    //public virtual void OnUpdating(TValue newValue, TValue oldValue, TrackingStatus oldTrackingStatus, TrackingSet<TValue> trackingSet, ITrackingContext trackingContext) {
    //    this.Validate(newValue);
    //}

    //public virtual void OnDeleting(TValue newValue, TValue oldValue, TrackingStatus oldTrackingStatus, TrackingSet<TValue> trackingSet, ITrackingContext trackingContext) {
    //    this.Validate(newValue);
    //}

    protected virtual void Validate(TValue value, ITrackingContext trackingContext, ITrackingSet<TKey, TValue> trackingSet) {
        // no need to call this
    }

    public virtual AddingArgument<TKey, TValue> OnAdding(AddingArgument<TKey, TValue> argument) {
        this.Validate(argument.Value, argument.TrackingContext, argument.TrackingSet);
        return argument;
    }

    public virtual UpdatingArgument<TKey, TValue> OnUpdating(UpdatingArgument<TKey, TValue> argument) {
        this.Validate(argument.NewValue, argument.TrackingContext, argument.TrackingSet);
        return argument;
    }

    public virtual DeletingArgument<TKey, TValue> OnDeleting(DeletingArgument<TKey, TValue> argument) {
        this.Validate(argument.NewValue, argument.TrackingContext, argument.TrackingSet);
        return argument;
    }
}
