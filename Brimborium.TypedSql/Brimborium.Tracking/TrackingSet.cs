using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Tracking;

public abstract class TrackingSet {
    protected TrackingSet(TrackingContext trackingContext) {
        this.TrackingContext = trackingContext;
        trackingContext.RegisterTrackingSet(this);
    }

    public TrackingContext TrackingContext { get; }

    internal abstract Type GetItemType();
}
public abstract class TrackingSet<TValue> 
    : TrackingSet
    where TValue : class {

    protected TrackingSet(
        TrackingContext trackingContext,
        TrackingSetApplyChanges<TValue> trackingApplyChanges
        ) : base(trackingContext) {
        this.TrackingApplyChanges = trackingApplyChanges;
    }

    public TrackingSetApplyChanges<TValue> TrackingApplyChanges { get; }

    internal override Type GetItemType() => typeof(TValue);

    public abstract TrackingObject<TValue> Attach(TValue item);

    public abstract TrackingObject<TValue> Insert(TValue item);
    
    public abstract TrackingObject<TValue> Update(TValue item);

    public abstract TrackingObject<TValue> Upsert(TValue item);

    public abstract void Detach(TrackingObject<TValue> item);

    public abstract void Delete(TrackingObject<TValue> trackingObject);

    internal protected abstract void Upsert(TValue value, TrackingObject<TValue> trackingObject);

}

public class TrackingSet<TKey, TValue>
    : TrackingSet<TValue>
    where TKey : notnull
    where TValue : class {
    private readonly Dictionary<TKey, TrackingObject<TValue>> _Items;
    //private readonly List<TrackingObject<TItem>> _ItemsToDelete;
    private readonly Func<TValue, TKey> _ExtractKey;

    public int Count => this._Items.Count;


    public TrackingSet(
        Func<TValue, TKey> extractKey,
        IEqualityComparer<TKey> comparer,
        TrackingContext trackingContext,
        TrackingSetApplyChanges<TValue> trackingApplyChanges
        ) : base(trackingContext, trackingApplyChanges) {
        this._ExtractKey = extractKey;
        this._Items = new Dictionary<TKey, TrackingObject<TValue>>(comparer);
    }

    /// <summary>
    /// register the item to the dataset
    /// </summary>
    /// <param name="item">the item to add</param>
    /// <returns>the TrackingObject containing the item.</returns>
    /// <exception cref="System.ArgumentException">
    /// a item with the same already exists
    /// </exception>
    public override TrackingObject<TValue> Attach(TValue item) {
        var result = new TrackingObject<TValue>(
            value: item,
            status: TrackingStatus.Original,
            trackingSet: this
            );
        var key = this._ExtractKey(item);
        this._Items.Add(key, result);
        return result;
    }

    public override void Detach(TrackingObject<TValue> item) {
        var key = this._ExtractKey(item.Value);
        this._Items.Remove(key);
    }

    public override TrackingObject<TValue> Insert(TValue item) {
        var key = this._ExtractKey(item);
        if (this._Items.TryGetValue(key, out var found)) {
            if (found.Status == TrackingStatus.Original) {
                found.Set(item, TrackingStatus.Added);
                return found;
            }
            if (found.Status == TrackingStatus.Added) {
                found.Set(item, TrackingStatus.Added);
                return found;
            }
            if (found.Status == TrackingStatus.Modified) {
                throw new InvalidOperationException("item is already modified.");
            }
            if (found.Status == TrackingStatus.Deleted) {
                throw new InvalidOperationException("item is already deleted.");
            }
            throw new InvalidOperationException("unknown");
        } else {
            var result = new TrackingObject<TValue>(
               value: item,
               status: TrackingStatus.Added,
               trackingSet: this
               );
            this._Items.Add(key, result);
            return result;
        }
    }

    public override TrackingObject<TValue> Update(TValue item) {
        var key = this._ExtractKey(item);
        if (this._Items.TryGetValue(key, out var found)) {
            if (found.Status == TrackingStatus.Original) {
                found.Set(item, TrackingStatus.Modified);
                return found;
            }
            if (found.Status == TrackingStatus.Added) {
                found.Set(item, TrackingStatus.Added);
                return found;
            }
            if (found.Status == TrackingStatus.Modified) {
                found.Set(item, TrackingStatus.Modified);
                return found;
            }
            if (found.Status == TrackingStatus.Deleted) {
                throw new InvalidOperationException("item is already deleted.");
            }
            throw new InvalidOperationException("unknown");
        } else {
            var result = new TrackingObject<TValue>(
               value: item,
               status: TrackingStatus.Modified,
               trackingSet: this
               );
            this._Items.Add(key, result);
            return result;
        }
    }

    public override TrackingObject<TValue> Upsert(TValue item) {
        var key = this._ExtractKey(item);
        if (this._Items.TryGetValue(key, out var found)) {
            if (found.Status == TrackingStatus.Original) {
                found.Set(item, TrackingStatus.Modified);
                return found;
            }
            if (found.Status == TrackingStatus.Added) {
                found.Set(item, TrackingStatus.Added);
                return found;
            }
            if (found.Status == TrackingStatus.Modified) {
                found.Set(item, TrackingStatus.Modified);
                return found;
            }
            if (found.Status == TrackingStatus.Deleted) {
                throw new InvalidOperationException("item is already deleted.");
            }
            throw new InvalidOperationException("unknown");
        } else {
            var result = new TrackingObject<TValue>(
               value: item,
               status: TrackingStatus.Added,
               trackingSet: this
               );
            this._Items.Add(key, result);
            return result;
        }
    }

    public void Delete(TValue item) {
        var key = this._ExtractKey(item);
        if (this._Items.TryGetValue(key, out var trackingObject)) {
            this.Delete(trackingObject);
        }
    }

    protected internal override void Upsert(TValue value, TrackingObject<TValue> trackingObject) {
        if (trackingObject.Status == TrackingStatus.Original) {
            trackingObject.Set(value, TrackingStatus.Modified);
            this.TrackingContext.TrackingChanges.Add(
                new TrackingChange(Tracking.TrackingStatus.Modified, trackingObject)
                ); ;
        } else if (trackingObject.Status == TrackingStatus.Added) {
            trackingObject.Set(value, TrackingStatus.Added);
            // TrackingChange should be there
        } else if (trackingObject.Status == TrackingStatus.Modified) {
            trackingObject.Set(value, TrackingStatus.Modified);
            // TrackingChange should be there
        } else if (trackingObject.Status == TrackingStatus.Deleted) {
            throw new System.InvalidOperationException("The object is deleted.");
        }
    }

    public override void Delete(TrackingObject<TValue> trackingObject) {
        //base.Delete(trackingObject);
        if (!ReferenceEquals(trackingObject.TrackingSet, this)) {
            throw new InvalidOperationException("wrong TrackingSet");
        } else {
            var key = this._ExtractKey(trackingObject.Value);
            if (this._Items.TryGetValue(key, out var found)) {
                if (found.Status == TrackingStatus.Deleted) {
                    // already deleted, but found???
                    this._Items.Remove(key);
                    return;
                }
                if (found.Status == TrackingStatus.Added) {
                    // created and deleted
                    found.Set(trackingObject.Value, TrackingStatus.Deleted);
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
            } else {
                throw new InvalidOperationException("item not found.");
            }
        }
    }
    
    public TrackingObject<TValue> this[TKey key] => this._Items[key];

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)]out TrackingObject<TValue> value) {
        return this._Items.TryGetValue(key, out value);
    }

    public IEnumerable<TrackingObject<TValue>> GetValues()
        => this._Items.Values;
}
