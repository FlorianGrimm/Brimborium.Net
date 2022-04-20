using System;
using System.Threading.Tasks;

namespace Brimborium.Tracking;

public abstract class TrackingSetApplyChanges<TValue> {
    protected TrackingSetApplyChanges() {

    }

    public abstract Task<TValue> Insert(TValue value, TrackingTransaction trackingTransaction);

    public abstract Task<TValue> Update(TValue value, TrackingTransaction trackingTransaction);

    public abstract Task Delete(TValue value, TrackingTransaction trackingTransaction);
}

public class TrackingSetApplyChangesDelegate<TValue>
    : TrackingSetApplyChanges<TValue> {
    private readonly Func<TValue, TrackingTransaction, Task<TValue>> _ActionInsert;
    private readonly Func<TValue, TrackingTransaction, Task<TValue>> _ActionUpdate;
    private readonly Func<TValue, TrackingTransaction, Task> _ActionDelete;

    public TrackingSetApplyChangesDelegate(
        Func<TValue, TrackingTransaction, Task<TValue>> actionInsert,
        Func<TValue, TrackingTransaction, Task<TValue>> actionUpdate,
        Func<TValue, TrackingTransaction, Task> actionDelete
        ) {
        this._ActionInsert = actionInsert;
        this._ActionUpdate = actionUpdate;
        this._ActionDelete = actionDelete;
    }

    public override Task<TValue> Insert(TValue value, TrackingTransaction trackingTransaction) {
        return this._ActionInsert(value, trackingTransaction);
    }

    public override Task<TValue> Update(TValue value, TrackingTransaction trackingTransaction) {
        return this._ActionUpdate(value, trackingTransaction);
    }

    public override Task Delete(TValue value, TrackingTransaction trackingTransaction) {
        return this._ActionDelete(value, trackingTransaction);
    }
}