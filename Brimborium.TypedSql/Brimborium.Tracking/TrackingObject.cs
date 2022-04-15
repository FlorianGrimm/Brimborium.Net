namespace Brimborium.Tracking;
public class TrackingObject {
    private TrackingStatus _Status;
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
}
public class TrackingObject<TItem> : TrackingObject {
    private TItem _Item;
    private readonly TrackingSet<TItem> _TrackingSet;

    public TrackingObject(
        TItem item,
        TrackingStatus status,
        TrackingSet<TItem> trackingSet)
        : base(status) {
        this._Item = item;
        this._TrackingSet = trackingSet;
    }

    public TItem Item {
        get {
            return this._Item;
        }
        set {
            if (this.Status == TrackingStatus.Original) { 
                this._Item = value;
                this.TrackingSet.TrackingContext.TrackingChanges.Add(
                    new TrackingChange(Tracking.TrackingStatus.Added, this)
                    ); ;
            } else if (this.Status == TrackingStatus.Added) {
                this._Item = value;
                // TrackingChange should be there
            } else if (this.Status == TrackingStatus.Modified) {
                this._Item = value;
                // TrackingChange should be there
            } else if (this.Status == TrackingStatus.Deleted) {
                throw new System.InvalidOperationException("The object is deleted.");
            }
        }
    }

    internal TrackingSet<TItem> TrackingSet => this._TrackingSet;

    public void Delete() {
        this._TrackingSet.Delete(this);
    }
}
