using System;

namespace Brimborium.Tracking;

public abstract class TrackingSetApplyChanges<TValue> {
    protected TrackingSetApplyChanges() {

    }

    public abstract void Insert(TValue value, TrackingTransaction trackingTransaction);

    public abstract void Update(TValue value, TrackingTransaction trackingTransaction);

    public abstract void Delete(TValue value, TrackingTransaction trackingTransaction);
}

public class TrackingSetApplyChangesDelegate<TValue> 
    : TrackingSetApplyChanges<TValue> {
    private readonly Action<TValue, TrackingTransaction> _ActionInsert;
    private readonly Action<TValue, TrackingTransaction> _ActionUpdate;
    private readonly Action<TValue, TrackingTransaction> _ActionDelete;

    public TrackingSetApplyChangesDelegate(
        Action<TValue, TrackingTransaction> actionInsert,
        Action<TValue, TrackingTransaction> actionUpdate,
        Action<TValue, TrackingTransaction> actionDelete
        ) {
        this._ActionInsert = actionInsert;
        this._ActionUpdate = actionUpdate;
        this._ActionDelete = actionDelete;
    }

    public override void Insert(TValue value, TrackingTransaction trackingTransaction) {
        this._ActionInsert(value, trackingTransaction);
    }

    public override void Update(TValue value, TrackingTransaction trackingTransaction) {
        this._ActionUpdate(value, trackingTransaction);
    }

    public override void Delete(TValue value, TrackingTransaction trackingTransaction) {
        this._ActionDelete(value, trackingTransaction);
    }
}