namespace Brimborium.Tracking;

public class TrackingContext
    : ITrackingContext {
    private readonly Dictionary<Type, TrackingSet> _TrackingSetByType;
    public TrackingChanges TrackingChanges { get; }

    public TrackingContext() {
        this._TrackingSetByType = new Dictionary<Type, TrackingSet>();
        this.TrackingChanges = new TrackingChanges(this);
    }

    public void RegisterTrackingSet(TrackingSet trackingSet) {
        var itemType = trackingSet.GetItemType();
        this._TrackingSetByType.Add(itemType, trackingSet);
    }

    public TrackingObject<TValue>? Attach<TValue>(TValue item)
        where TValue : class {
        if (this._TrackingSetByType.TryGetValue(typeof(TValue), out var trackingSet)) {
            return ((TrackingSet<TValue>)trackingSet).Attach(item);
        } else {
            throw new InvalidOperationException();
        }
    }

    public TrackingObject<TValue> Add<TValue>(TValue item)
        where TValue : class {
        if (this._TrackingSetByType.TryGetValue(typeof(TValue), out var trackingSet)) {
            return ((TrackingSet<TValue>)trackingSet).Add(item);
        } else {
            throw new InvalidOperationException();
        }
    }

    public TrackingObject<TValue> Update<TValue>(TValue item)
        where TValue : class {
        if (this._TrackingSetByType.TryGetValue(typeof(TValue), out var trackingSet)) {
            return ((TrackingSet<TValue>)trackingSet).Update(item);
        } else {
            throw new InvalidOperationException();
        }
    }

    public TrackingObject<TValue> Upsert<TValue>(TValue item)
        where TValue : class {
        if (this._TrackingSetByType.TryGetValue(typeof(TValue), out var trackingSet)) {
            return ((TrackingSet<TValue>)trackingSet).Upsert(item);
        } else {
            throw new InvalidOperationException();
        }
    }

    public async Task ApplyChangesAsync(
        ITrackingTransConnection trackingTransConnection,
        CancellationToken cancellationToken = default(CancellationToken)) {
        await this.TrackingChanges.ApplyChangesAsync(trackingTransConnection, cancellationToken);
    }

#if no
    public object SaveSate() {
        if (this.TrackingChanges.Changes.Count > 0) {
            throw new NotSupportedException("TrackingChanges.Changes is not empty");
        }
        var result = new Dictionary<Type, TrackingSet>();
        foreach (var (type, trackingSet) in this._TrackingSetByType) {
            result.Add(type, trackingSet.SaveState());
        }
        return result;
    }
    public void RestoreSate(object state) {
        var stateDict = (Dictionary<Type, TrackingSet>) state;
        foreach (var (type, trackingSet) in stateDict) {
            this._TrackingSetByType[type].RestoreState(trackingSet);
        }
    }
#endif
}
