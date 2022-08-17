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

    /// <summary>
    /// Called by Adding, Updating, Deleting
    /// </summary>
    protected virtual void ValidateCore(TValue value, ITrackingContext trackingContext, ITrackingSet<TKey, TValue> trackingSet) {
        // no need to call this
    }

    /// <summary>
    /// Called by Adding, Updating
    /// </summary>
    protected virtual void Validate(TValue value, ITrackingContext trackingContext, ITrackingSet<TKey, TValue> trackingSet) {
        // no need to call this
    }


    public virtual AddingArgument<TKey, TValue> OnAdding(AddingArgument<TKey, TValue> argument) {
        this.ValidateCore(argument.Value, argument.TrackingContext, argument.TrackingSet);
        this.Validate(argument.Value, argument.TrackingContext, argument.TrackingSet);
        return argument;
    }

    public virtual UpdatingArgument<TKey, TValue> OnUpdating(UpdatingArgument<TKey, TValue> argument) {
        this.ValidateCore(argument.NewValue, argument.TrackingContext, argument.TrackingSet);
        this.Validate(argument.NewValue, argument.TrackingContext, argument.TrackingSet);
        return argument;
    }

    public virtual DeletingArgument<TKey, TValue> OnDeleting(DeletingArgument<TKey, TValue> argument) {
        this.ValidateCore(argument.OldValue, argument.TrackingContext, argument.TrackingSet);
        return argument;
    }
}
