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
        var itemType=trackingSet.GetItemType();
        this._TrackingSetByType.Add(itemType, trackingSet);
    }

    public TrackingObject<TItem> Attach<TItem>(TItem item) {
        if (this._TrackingSetByType.TryGetValue(typeof(TItem), out var trackingSet)) {
            return ((TrackingSet<TItem>)trackingSet).Attach(item);
        } else {
            throw new InvalidOperationException();
        }
    }

    //public TrackingSet<TItem> Insert<TItem>(TItem item) {
    //    return this;
    //}

    //public TrackingSet<TItem> Update<TItem>(TItem item) {
    //    return this;
    //}
}
