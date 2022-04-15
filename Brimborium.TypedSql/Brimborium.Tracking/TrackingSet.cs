using System;
using System.Collections.Generic;

namespace Brimborium.Tracking;

public abstract class TrackingSet {
    protected TrackingSet(TrackingContext trackingContext) {
        this.TrackingContext = trackingContext;
        trackingContext.RegisterTrackingSet(this);
    }

    public TrackingContext TrackingContext { get; }

    internal abstract Type GetItemType();
}
public abstract class TrackingSet<TItem> : TrackingSet {
    protected TrackingSet(TrackingContext trackingContext)
        : base(trackingContext) {
    }

    internal override Type GetItemType() => typeof(TItem);
    public abstract TrackingObject<TItem> Attach(TItem item);

    public abstract void Delete(TrackingObject<TItem> trackingObject);
}

public class TrackingSet<TKey, TItem>
    : TrackingSet<TItem>
    where TKey : notnull {
    private readonly Dictionary<TKey, TrackingObject<TItem>> _Items;
    //private readonly List<TrackingObject<TItem>> _ItemsToDelete;
    private readonly Func<TItem, TKey> _ExtractKey;

    public int Count => this._Items.Count;

    public TrackingSet(
        Func<TItem, TKey> extractKey,
        IEqualityComparer<TKey> comparer,
        TrackingContext trackingContext
        ) : base(trackingContext) {
        this._ExtractKey = extractKey;
        this._Items = new Dictionary<TKey, TrackingObject<TItem>>(comparer);
    }

    public override TrackingObject<TItem> Attach(TItem item) {
        var result = new TrackingObject<TItem>(
            item: item,
            status: TrackingStatus.Original,
            trackingSet: this
            );
        var key=this._ExtractKey(item);
        this._Items.Add(key,result);
        return result;
    }

    public TrackingSet<TItem> Insert(TItem item) {
        return this;
    }

    public TrackingSet<TItem> Update(TItem item) {
        return this;
    }

    public void Delete(TItem item) {
        var key = this._ExtractKey(item);
        if (this._Items.TryGetValue(key, out var trackingObject)) {
            this.Delete(trackingObject);
        }
    }
    public override void Delete(TrackingObject<TItem> trackingObject) {
        //base.Delete(trackingObject);
        if (!ReferenceEquals(trackingObject.TrackingSet, this)) {
            throw new InvalidOperationException("wrong TrackingSet");
        } else {
            var key = this._ExtractKey(trackingObject.Item);
            if (this._Items.TryGetValue(key, out var found)) {
                if (found.Status == TrackingStatus.Deleted) {
                    // already deleted, but found???
                    this._Items.Remove(key);
                    return;
                }
                if (found.Status == TrackingStatus.Added) {
                    // created and deleted
                    found.Status = TrackingStatus.Deleted;
                    this._Items.Remove(key);
                    this.TrackingContext.TrackingChanges.Remove(TrackingStatus.Added, found);
                    return;
                }
                if (found.Status == TrackingStatus.Modified) {
                    found.Status = TrackingStatus.Deleted;
                    this._Items.Remove(key);
                    return;
                }
                if (found.Status == TrackingStatus.Deleted) {
                    found.Status = TrackingStatus.Deleted;
                    this._Items.Remove(key);
                    return;
                }
            }
        }
    }
}
