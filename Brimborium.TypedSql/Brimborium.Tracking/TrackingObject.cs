namespace Brimborium.Tracking;
public abstract class TrackingObject {
    protected TrackingStatus _Status;

    protected TrackingObject(
        TrackingStatus status
        ) {
        this._Status = status;
    }

    public TrackingStatus Status {
        get {
            return this._Status;
        }
        internal set {
            this._Status = value;
        }
    }

    public abstract object GetValue();

    public abstract void ApplyChanges(TrackingStatus status, TrackingTransaction trackingTransaction);
}
public class TrackingObject<TValue> 
    : TrackingObject
    where TValue : class
    {
    private TValue _Value;
    private readonly TrackingSet<TValue> _TrackingSet;

    public TrackingObject(
        TValue value,
        TrackingStatus status,
        TrackingSet<TValue> trackingSet)
        : base(status) {
        this._Value = value;
        this._TrackingSet = trackingSet;
    }

    internal void Set(
        TValue value,
        TrackingStatus status
        ) {
        this._Value = value;
        this._Status = status;
    }

    public TValue Value {
        get {
            return this._Value;
        }
        set {
            this._TrackingSet.Upsert(value, this);
        }
    }

    public override object GetValue() => this._Value;

    internal TrackingSet<TValue> TrackingSet => this._TrackingSet;

    public void Delete() {
        this._TrackingSet.Delete(this);
    }

    public override void ApplyChanges(TrackingStatus status, TrackingTransaction  trackingTransaction) {
        if (this.Status != status) {
            throw new System.InvalidOperationException($"{this.Status}!={status}");
        }
        if (this.Status == TrackingStatus.Original) {
            // all done
        } else if (this.Status == TrackingStatus.Added) {
            this.TrackingSet.TrackingApplyChanges.Insert(this.Value, trackingTransaction);
            this.Status = TrackingStatus.Original;
        } else if (this.Status == TrackingStatus.Modified) {
            this.TrackingSet.TrackingApplyChanges.Update(this.Value, trackingTransaction);
            this.Status = TrackingStatus.Original;
        } else if (this.Status == TrackingStatus.Deleted) {
            this.TrackingSet.TrackingApplyChanges.Delete(this.Value, trackingTransaction);
            this.Status = TrackingStatus.Original;
        } else {
            throw new System.InvalidOperationException($"{this.Status} unknown.");
        }
    }
}
