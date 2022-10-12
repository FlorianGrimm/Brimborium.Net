namespace Brimborium.Tracking;

public abstract class TrackingObject {
    protected TrackingStatus _OrginalStatus;
    protected TrackingStatus _Status;

    protected TrackingObject(
        TrackingStatus status
        ) {
        this._OrginalStatus = status;
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

    public abstract Task ApplyChangesAsync(ITrackingTransConnection trackingTransConnection);

    internal protected virtual void Undo() {
    }
}

public class TrackingObject<TKey, TValue>
    : TrackingObject
    where TKey : notnull
    where TValue : class {
    private TKey _Key;
    private TValue _OrginalValue;
    private TValue _Value;
    private readonly TrackingSet<TKey, TValue> _TrackingSet;

    public TrackingObject(
        TKey key,
        TValue value,
        TrackingStatus status,
        TrackingSet<TKey, TValue> trackingSet)
        : base(status) {
        this._Key = key;
        this._OrginalValue = value;
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

    public TKey Key { get => this._Key; }

    public TValue Value {
        get {
            return this._Value;
        }
        set {
            this._TrackingSet.ValueSet(this, value);
        }
    }

    public override object GetValue() => this._Value;

    internal TrackingSet<TKey, TValue> TrackingSet => this._TrackingSet;

    public void Delete() {
        this._TrackingSet.Delete(this);
    }

    public void Detach() {
        this._TrackingSet.Detach(this);
    }

    public override async Task ApplyChangesAsync(
        ITrackingTransConnection transConnection
        ) {
        if (this.Status == TrackingStatus.Original) {
            // all done
        } else if (this.Status == TrackingStatus.Added) {
            var nextValue = await this.TrackingSet.TrackingApplyChanges.Insert(this, transConnection);

            if (nextValue is null) {
                this._TrackingSet.Detach(this);
                this.Status = TrackingStatus.Original;
                this._Value = nextValue!;
                this._OrginalValue = nextValue!;
            } else {
                this.Status = TrackingStatus.Original;
                this._Value = nextValue;
                this._OrginalValue = nextValue;
                if (this._TrackingSet.TryExtractKey(nextValue, out var nextKey)) {
                    if (this._Key.Equals(nextKey)) {
                    } else {
                        var oldKey = this._Key;
                        this._Key = nextKey;
                        this._TrackingSet.ReAttach(oldKey, this);
                    }
                }
            }
        } else if (this.Status == TrackingStatus.Modified) {
            var nextValue = await this.TrackingSet.TrackingApplyChanges.Update(this, transConnection);
            if (nextValue is null) {
                this._TrackingSet.Detach(this);
                this.Status = TrackingStatus.Original;
                this._Value = nextValue!;
                this._OrginalValue = nextValue!;
            } else {
                this.Status = TrackingStatus.Original;
                this._Value = nextValue;
                this._OrginalValue = nextValue;
                if (this._TrackingSet.TryExtractKey(nextValue, out var nextKey)) {
                    if (this._Key.Equals(nextKey)) {
                    } else {
                        var oldKey = this._Key;
                        this._Key = nextKey;
                        this._TrackingSet.ReAttach(oldKey, this);
                    }
                }
            }
        } else if (this.Status == TrackingStatus.Deleted) {
            await this.TrackingSet.TrackingApplyChanges.Delete(this, transConnection);
        } else {
            throw new InvalidModificationException($"{this.Status} unknown.", "Status", "", typeof(TValue).Name, "");
        }
    }

    protected internal override void Undo() {
        if (this._OrginalStatus == TrackingStatus.Original) {
            if (this.Status == TrackingStatus.Modified) {
                this._Value = this._OrginalValue;
                this._Status = this._OrginalStatus;
                return;
            } else if (this.Status == TrackingStatus.Deleted) {
                this._Value = this._OrginalValue;
                this._Status = TrackingStatus.Original;
                this.TrackingSet.ReAttach(this.Key, this);
                return;
            } else if (this.Status == TrackingStatus.Original) {
                // ignore
            }
        } else if (this._OrginalStatus == TrackingStatus.Added) {
            if (this.Status == TrackingStatus.Added) {
                this.TrackingSet.Detach(this);
                this._Status = TrackingStatus.Original;
                return;
            }
        }
        throw new InvalidModificationException($"{this._OrginalStatus}-{this._Status} unknown.", "Status", "", typeof(TValue).Name, "");
    }
}
