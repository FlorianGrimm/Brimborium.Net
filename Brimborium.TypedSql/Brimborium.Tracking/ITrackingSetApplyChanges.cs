namespace Brimborium.Tracking;

public interface ITrackingSetApplyChanges<TKey, TValue>
    where TKey : notnull
    where TValue : class {

    Task<TValue> Insert(TrackingObject<TKey, TValue> to, ITrackingTransConnection trackingTransaction);

    Task<TValue> Update(TrackingObject<TKey, TValue> to, ITrackingTransConnection trackingTransaction);

    Task Delete(TrackingObject<TKey, TValue> to, ITrackingTransConnection trackingTransaction);
}