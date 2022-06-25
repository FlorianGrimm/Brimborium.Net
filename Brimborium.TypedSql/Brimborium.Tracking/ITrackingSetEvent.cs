namespace Brimborium.Tracking;

public interface ITrackingSetEvent<TValue>
    where TValue : class {
    //void OnAdding(
    //    TValue value,
    //    TrackingSet<TValue> trackingSet,
    //    ITrackingContext trackingContext
    //    );
    AddingArgument<TValue> OnAdding(
       AddingArgument<TValue> argument
       );
    //void OnUpdating(
    //    TValue newValue,
    //    TValue oldValue,
    //    TrackingStatus oldTrackingStatus,
    //    TrackingSet<TValue> trackingSet,
    //    ITrackingContext trackingContext
    //    );
    UpdatingArgument<TValue> OnUpdating(
       UpdatingArgument<TValue> argument
       );
    //void OnDeleting(
    //    TValue newValue,
    //    TValue oldValue,
    //    TrackingStatus oldTrackingStatus,
    //    TrackingSet<TValue> trackingSet,
    //    ITrackingContext trackingContext
    //    );
    DeletingArgument<TValue> OnDeleting(
       DeletingArgument<TValue> argument
       );
}

public record struct AddingArgument<TValue>(
    TValue Value,
    ITrackingSet<TValue> TrackingSet,
    ITrackingContext TrackingContext
    )
    where TValue : class;

public record struct UpdatingArgument<TValue>(
    TValue NewValue,
    TValue OldValue,
    TrackingStatus OldTrackingStatus,
    ITrackingSet<TValue> TrackingSet,
    ITrackingContext TrackingContext
    )
    where TValue : class;

public record struct DeletingArgument<TValue>(
    TValue NewValue,
    TValue OldValue,
    TrackingStatus OldTrackingStatus,
    ITrackingSet<TValue> TrackingSet,
    ITrackingContext TrackingContext
    )
    where TValue : class;