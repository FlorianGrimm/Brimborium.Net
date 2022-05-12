namespace Brimborium.Tracking;

public class TrackingChanges {
    private readonly TrackingContext _TrackingContext;
    public readonly List<TrackingObject> Changes;

    public TrackingChanges(TrackingContext trackingContext) {
        this._TrackingContext = trackingContext;
        this.Changes = new List<TrackingObject>();
    }

    public async Task ApplyChangesAsync(
        ITrackingTransConnection transConnection,
        CancellationToken cancellationToken = default(CancellationToken)
        ) {
        if (this.Changes.Count > 0) {
            var changes = this.Changes.ToArray();
            this.Changes.Clear();
            foreach (var to in changes) {
                await to.ApplyChangesAsync(transConnection);
                cancellationToken.ThrowIfCancellationRequested();
            }
            await transConnection.CommitAsync();
        }
    }

    public void Add(TrackingObject change) {
        this.Changes.Add(change);
    }

    public void Remove(TrackingObject trackingObject) {
        var value = trackingObject.GetValue();
        for (int idx = 0; idx < this.Changes.Count; idx++) {
            if (ReferenceEquals(this.Changes[idx], trackingObject)) {
                this.Changes.RemoveAt(idx);
                return;
            }
            if (ReferenceEquals(this.Changes[idx].GetValue(), value)) {
                this.Changes.RemoveAt(idx);
                return;
            }
        }
    }

    public void Clear() {
        this.Changes.Clear();
    }

    public void Undo() {
        for (int idx = this.Changes.Count - 1; idx >= 0; idx--) {
            var to = this.Changes[idx];
            to.Undo();
        }
    }

    public void Undo(TrackingObject trackingObject) {
        var value = trackingObject.GetValue();
        for (int idx = 0; idx < this.Changes.Count; idx++) {
            if (ReferenceEquals(this.Changes[idx], trackingObject)) {
                var to = this.Changes[idx];
                to.Undo();
                return;
            }
            if (ReferenceEquals(this.Changes[idx].GetValue(), value)) {
                var to = this.Changes[idx];
                to.Undo();
                return;
            }
        }
    }
}
