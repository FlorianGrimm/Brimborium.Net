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

    public abstract Task ApplyChangesAsync(
        TrackingStatus status,
        ITrackingTransConnection trackingTransConnection);
}
public class TrackingObject<TValue>
    : TrackingObject
    where TValue : class {
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
            this._TrackingSet.ValueSet(this, value);
        }
    }

    public override object GetValue() => this._Value;

    internal TrackingSet<TValue> TrackingSet => this._TrackingSet;

    public void Delete() {
        this._TrackingSet.Delete(this);
    }

    public void Detach() {
        this._TrackingSet.Detach(this);
    }

    public override async Task ApplyChangesAsync(
        TrackingStatus status,
        ITrackingTransConnection transConnection
        ) {
        if (this.Status != status) {
            throw new System.InvalidOperationException($"{this.Status}!={status}");
        }
        if (this.Status == TrackingStatus.Original) {
            // all done
        } else if (this.Status == TrackingStatus.Added) {
            var nextValue = await this.TrackingSet.TrackingApplyChanges.Insert(this.Value, transConnection);
            this.Status = TrackingStatus.Original;
            this._Value = nextValue;
        } else if (this.Status == TrackingStatus.Modified) {
            var nextValue = await this.TrackingSet.TrackingApplyChanges.Update(this.Value, transConnection);
            this.Status = TrackingStatus.Original;
            this._Value = nextValue;
        } else if (this.Status == TrackingStatus.Deleted) {
            await this.TrackingSet.TrackingApplyChanges.Delete(this.Value, transConnection);
#warning TODO add TEST
            // this.TrackingSet.Detach(this); this should already be deleted!
        } else {
            throw new System.InvalidOperationException($"{this.Status} unknown.");
        }
    }
}
