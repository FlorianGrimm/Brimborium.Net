namespace Brimborium.Tracking;
public interface ITrackingContext {
    //void RegisterTrackingSet(TrackingSet trackingSet);
    
    TrackingChanges TrackingChanges { get; }

    Task ApplyChangesAsync(
        ITrackingTransConnection trackingTransConnection,
        CancellationToken cancellationToken = default(CancellationToken));

    TrackingObject<TValue>? Attach<TValue>(TValue item) where TValue : class;
    TrackingObject<TValue> Add<TValue>(TValue item) where TValue : class;
    TrackingObject<TValue> Update<TValue>(TValue item) where TValue : class;
    TrackingObject<TValue> Upsert<TValue>(TValue item) where TValue : class;
}
