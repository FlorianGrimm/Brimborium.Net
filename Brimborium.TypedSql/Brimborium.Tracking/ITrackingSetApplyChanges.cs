namespace Brimborium.Tracking;

public interface ITrackingSetApplyChanges<TValue> {

    Task<TValue> Insert(TValue value, ITrackingTransConnection trackingTransaction);

    Task<TValue> Update(TValue value, ITrackingTransConnection trackingTransaction);

    Task Delete(TValue value, ITrackingTransConnection trackingTransaction);
}

public class TrackingSetApplyChangesDelegate<TValue>
    : ITrackingSetApplyChanges<TValue> {
    private readonly Func<TValue, ITrackingTransConnection, Task<TValue>> _ActionInsert;
    private readonly Func<TValue, ITrackingTransConnection, Task<TValue>> _ActionUpdate;
    private readonly Func<TValue, ITrackingTransConnection, Task> _ActionDelete;

    public TrackingSetApplyChangesDelegate(
        Func<TValue, ITrackingTransConnection, Task<TValue>> actionInsert,
        Func<TValue, ITrackingTransConnection, Task<TValue>> actionUpdate,
        Func<TValue, ITrackingTransConnection, Task> actionDelete
        ) {
        this._ActionInsert = actionInsert;
        this._ActionUpdate = actionUpdate;
        this._ActionDelete = actionDelete;
    }

    public Task<TValue> Insert(TValue value, ITrackingTransConnection trackingTransaction) {
        return this._ActionInsert(value, trackingTransaction);
    }

    public Task<TValue> Update(TValue value, ITrackingTransConnection trackingTransaction) {
        return this._ActionUpdate(value, trackingTransaction);
    }

    public Task Delete(TValue value, ITrackingTransConnection trackingTransaction) {
        return this._ActionDelete(value, trackingTransaction);
    }
}