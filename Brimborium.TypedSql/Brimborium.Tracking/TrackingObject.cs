﻿namespace Brimborium.Tracking;

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

public class TrackingObject<TValue>
    : TrackingObject
    where TValue : class {
    private TValue _OrginalValue;
    private TValue _Value;
    private readonly TrackingSet<TValue> _TrackingSet;

    public TrackingObject(
        TValue value,
        TrackingStatus status,
        TrackingSet<TValue> trackingSet)
        : base(status) {
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
        ITrackingTransConnection transConnection
        ) {
        if (this.Status == TrackingStatus.Original) {
            // all done
        } else if (this.Status == TrackingStatus.Added) {
            var nextValue = await this.TrackingSet.TrackingApplyChanges.Insert(this.Value, transConnection);
            this.Status = TrackingStatus.Original;
            this._Value = nextValue;
            this._OrginalValue = nextValue;
        } else if (this.Status == TrackingStatus.Modified) {
            var nextValue = await this.TrackingSet.TrackingApplyChanges.Update(this.Value, transConnection);
            this.Status = TrackingStatus.Original;
            this._Value = nextValue;
            this._OrginalValue = nextValue;
        } else if (this.Status == TrackingStatus.Deleted) {
            await this.TrackingSet.TrackingApplyChanges.Delete(this.Value, transConnection);
        } else {
            throw new System.InvalidOperationException($"{this.Status} unknown.");
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
                this.TrackingSet.ReAttach(this);
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
        throw new System.InvalidOperationException($"{this._OrginalStatus}-{this._Status} unknown.");

    }
}
