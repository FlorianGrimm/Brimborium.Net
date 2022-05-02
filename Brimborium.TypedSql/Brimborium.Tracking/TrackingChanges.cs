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
    private readonly TrackingContext _TrackingContext;
    public readonly List<TrackingChange> Changes;

    public TrackingChanges(TrackingContext trackingContext) {
        this._TrackingContext = trackingContext;
        this.Changes = new List<TrackingChange>();
    }

    public async Task ApplyChangesAsync(
        ITrackingTransConnection transConnection,
        CancellationToken cancellationToken = default(CancellationToken)
        ) {
        if (this.Changes.Count > 0) {
            var changes = this.Changes.ToArray();
            this.Changes.Clear();
            foreach (var chg in changes) {
                await chg.TrackingObject.ApplyChangesAsync(chg.Status, transConnection);
                cancellationToken.ThrowIfCancellationRequested();
            }
            await transConnection.CommitAsync();
        }
    }

    public void Add(TrackingChange change) {
        this.Changes.Add(change);
    }

    public void Remove(TrackingStatus status, TrackingObject trackingObject) {
        var value = trackingObject.GetValue();
        for (int idx = 0; idx < this.Changes.Count; idx++) {
            if (this.Changes[idx].Status == status) {
                if (ReferenceEquals(this.Changes[idx].TrackingObject.GetValue(), value)) {
                    this.Changes.RemoveAt(idx);
                    return;
                }
            }
        }
    }

    public void Clear() {
        this.Changes.Clear();
    }
}
