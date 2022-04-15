using System.Collections.Generic;

namespace Brimborium.Tracking;

public class TrackingChange {
    public TrackingChange(
        TrackingStatus status,
        TrackingObject trackingObject
        ) {
        this.Status = status;
        this.TrackingObject = trackingObject;
    }

    public TrackingStatus Status { get; }
    public TrackingObject TrackingObject { get; }
}

public class TrackingChanges {
    private TrackingContext _TrackingContext;
    private readonly List<TrackingChange> _Changes;
    public TrackingChanges(TrackingContext trackingContext) {
        this._TrackingContext = trackingContext;
        this._Changes = new List<TrackingChange>();
    }

    public void Add(TrackingChange change) {
        this._Changes.Add(change);
    }
    public void Remove(TrackingStatus status, TrackingObject trackingObject) {
        for (int idx = 0; idx < this._Changes.Count; idx++) {
            if (this._Changes[idx].Status == status) {
                if (ReferenceEquals(this._Changes[idx].TrackingObject, trackingObject)) {
                    this._Changes.RemoveAt(idx);
                    return;
                }
            }
        }
    }
}
