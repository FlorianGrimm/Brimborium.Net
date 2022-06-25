namespace Brimborium.Tracking;

public class ValidateBaseTrackingSet<TValue> : ITrackingSetEvent<TValue>
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

    protected virtual void Validate(TValue value, ITrackingContext trackingContext, ITrackingSet<TValue> trackingSet) {
    }

    public virtual AddingArgument<TValue> OnAdding(AddingArgument<TValue> argument) {
        this.Validate(argument.Value, argument.TrackingContext, argument.TrackingSet);
        return argument;
    }

    public virtual UpdatingArgument<TValue> OnUpdating(UpdatingArgument<TValue> argument) {
        this.Validate(argument.NewValue, argument.TrackingContext, argument.TrackingSet);
        return argument;
    }

    public virtual DeletingArgument<TValue> OnDeleting(DeletingArgument<TValue> argument) {
        this.Validate(argument.NewValue, argument.TrackingContext, argument.TrackingSet);
        return argument;
    }
}
