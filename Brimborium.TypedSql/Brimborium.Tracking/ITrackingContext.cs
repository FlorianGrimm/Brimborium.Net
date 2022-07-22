namespace Brimborium.Tracking;
public interface ITrackingContext {
    //void RegisterTrackingSet(TrackingSet trackingSet);
    
    TrackingChanges TrackingChanges { get; }

    Task ApplyChangesAsync(
        ITrackingTransConnection trackingTransConnection,
        CancellationToken cancellationToken = default);

    //TrackingObject<TKey, TValue>? Attach<TKey, TValue>(TValue item) where TValue : class;
    //TrackingObject<TKey, TValue> Add<TKey, TValue>(TValue item) where TValue : class;
    //TrackingObject<TKey, TValue> Update<TKey, TValue>(TValue item) where TValue : class;
    //TrackingObject<TKey, TValue> Upsert<TKey, TValue>(TValue item) where TValue : class;
}
