namespace Brimborium.Tracking;
public enum TrackingStatus { 
    Original,
    Added,
    Modified,
    Deleted
}
public class TrackingObject<T> {
    private TrackingStatus _Status;
    private T _Item;

    public TrackingObject(T item, TrackingStatus status) {
        this._Item = item;
        this.Status = status;
    }

    public TrackingStatus Status {
        get {
            return this._Status;
        }
        set {
            this._Status = value;
        }
    }

    public T Item {
        get {
            return this._Item;
        }
        set {
            this._Item = value;
        }
    }

    public void Delete() {
        this._Status = TrackingStatus.Deleted;
    }

}

public class TrackingSet<T> {
    public TrackingSet() {
    }

    public TrackingSet<T> Attach(T item) { 
    }

    public TrackingSet<T> Insert(T item) {
    }

    public TrackingSet<T> Update(T item) {
    }

}
