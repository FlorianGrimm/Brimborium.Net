using System;
using System.Collections.Generic;
namespace Brimborium.Tracking;

public class TrackingContext {
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

    public TrackingObject<TValue> Attach<TValue>(TValue item)
        where TValue : class {
        if (this._TrackingSetByType.TryGetValue(typeof(TValue), out var trackingSet)) {
            return ((TrackingSet<TValue>)trackingSet).Attach(item);
        } else {
            throw new InvalidOperationException();
        }
    }

    public TrackingObject<TValue> Insert<TValue>(TValue item)
        where TValue : class {
        if (this._TrackingSetByType.TryGetValue(typeof(TValue), out var trackingSet)) {
            return ((TrackingSet<TValue>)trackingSet).Insert(item);
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

    public void ApplyChanges(TrackingConnection trackingConnection) {
        this.TrackingChanges.ApplyChangesAsync(trackingConnection);
        
    }
}
