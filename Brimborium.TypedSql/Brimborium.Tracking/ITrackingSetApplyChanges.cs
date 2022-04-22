using System;
using System.Threading.Tasks;

namespace Brimborium.Tracking;

public interface ITrackingSetApplyChanges<TValue> {

    Task<TValue> Insert(TValue value, TrackingTransConnection trackingTransaction);

    Task<TValue> Update(TValue value, TrackingTransConnection trackingTransaction);

    Task Delete(TValue value, TrackingTransConnection trackingTransaction);
}

public class TrackingSetApplyChangesDelegate<TValue>
    : ITrackingSetApplyChanges<TValue> {
    private readonly Func<TValue, TrackingTransConnection, Task<TValue>> _ActionInsert;
    private readonly Func<TValue, TrackingTransConnection, Task<TValue>> _ActionUpdate;
    private readonly Func<TValue, TrackingTransConnection, Task> _ActionDelete;

    public TrackingSetApplyChangesDelegate(
        Func<TValue, TrackingTransConnection, Task<TValue>> actionInsert,
        Func<TValue, TrackingTransConnection, Task<TValue>> actionUpdate,
        Func<TValue, TrackingTransConnection, Task> actionDelete
        ) {
        this._ActionInsert = actionInsert;
        this._ActionUpdate = actionUpdate;
        this._ActionDelete = actionDelete;
    }

    public Task<TValue> Insert(TValue value, TrackingTransConnection trackingTransaction) {
        return this._ActionInsert(value, trackingTransaction);
    }

    public Task<TValue> Update(TValue value, TrackingTransConnection trackingTransaction) {
        return this._ActionUpdate(value, trackingTransaction);
    }

    public Task Delete(TValue value, TrackingTransConnection trackingTransaction) {
        return this._ActionDelete(value, trackingTransaction);
    }
}