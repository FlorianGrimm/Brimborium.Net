namespace Brimborium.Tracking;

public class TrackingSetApplyChangesDelegate<TKey, TValue>
    : ITrackingSetApplyChanges<TKey, TValue>
    where TKey : notnull
    where TValue : class {
    private readonly Func<TrackingObject<TKey, TValue>, ITrackingTransConnection, Task<TValue>> _ActionInsert;
    private readonly Func<TrackingObject<TKey, TValue>, ITrackingTransConnection, Task<TValue>> _ActionUpdate;
    private readonly Func<TrackingObject<TKey, TValue>, ITrackingTransConnection, Task> _ActionDelete;

    public TrackingSetApplyChangesDelegate(
        Func<TrackingObject<TKey, TValue>, ITrackingTransConnection, Task<TValue>> actionInsert,
        Func<TrackingObject<TKey, TValue>, ITrackingTransConnection, Task<TValue>> actionUpdate,
        Func<TrackingObject<TKey, TValue>, ITrackingTransConnection, Task> actionDelete
        ) {
        this._ActionInsert = actionInsert;
        this._ActionUpdate = actionUpdate;
        this._ActionDelete = actionDelete;
    }

    public Task<TValue> Insert(TrackingObject<TKey, TValue> value, ITrackingTransConnection trackingTransaction) {
        return this._ActionInsert(value, trackingTransaction);
    }

    public Task<TValue> Update(TrackingObject<TKey, TValue> value, ITrackingTransConnection trackingTransaction) {
        return this._ActionUpdate(value, trackingTransaction);
    }

    public Task Delete(TrackingObject<TKey, TValue> value, ITrackingTransConnection trackingTransaction) {
        return this._ActionDelete(value, trackingTransaction);
    }
}