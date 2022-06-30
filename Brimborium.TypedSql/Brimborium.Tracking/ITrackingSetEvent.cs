namespace Brimborium.Tracking;

public interface ITrackingSetEvent<TKey, TValue>
    where TKey : notnull
    where TValue : class {
    AddingArgument<TKey, TValue> OnAdding(
       AddingArgument<TKey, TValue> argument
       );

    UpdatingArgument<TKey, TValue> OnUpdating(
       UpdatingArgument<TKey, TValue> argument
       );

    DeletingArgument<TKey, TValue> OnDeleting(
       DeletingArgument<TKey, TValue> argument
       );
}

public record struct AddingArgument<TKey, TValue>(
    TValue Value,
    ITrackingSet<TKey, TValue> TrackingSet,
    ITrackingContext TrackingContext
    )
    where TKey : notnull
    where TValue : class;

public record struct UpdatingArgument<TKey, TValue>(
    TValue NewValue,
    TrackingStatus NewTrackingStatus,
    TValue OldValue,
    TrackingStatus OldTrackingStatus,
    ITrackingSet<TKey, TValue> TrackingSet,
    ITrackingContext TrackingContext
    )
    where TKey : notnull
    where TValue : class;

public record struct DeletingArgument<TKey, TValue>(
    TValue NewValue,
    TValue OldValue,
    TrackingStatus OldTrackingStatus,
    ITrackingSet<TKey, TValue> TrackingSet,
    ITrackingContext TrackingContext
    )
    where TKey : notnull
    where TValue : class;