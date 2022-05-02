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
        ITrackingSetApplyChanges<TValue> trackingApplyChanges
        ) : base(trackingContext) {
        this.TrackingApplyChanges = trackingApplyChanges;
    }

    public ITrackingSetApplyChanges<TValue> TrackingApplyChanges { get; }

    internal override Type GetItemType() => typeof(TValue);

    public abstract TrackingObject<TValue>? Attach(TValue? item);
    
    public abstract List<TrackingObject<TValue>> AttachRange(IEnumerable<TValue> items);

    public abstract TrackingObject<TValue> Add(TValue item);

    public abstract TrackingObject<TValue> Update(TValue item);

    public abstract TrackingObject<TValue> Upsert(TValue item);

    public abstract void Detach(TrackingObject<TValue> item);

    public abstract void Delete(TrackingObject<TValue> trackingObject);

    internal protected abstract void ValueSet(TrackingObject<TValue> trackingObject, TValue value);

}

public class TrackingSet<TKey, TValue>
    : TrackingSet<TValue>
    , ITrackingSet<TKey, TValue>
    where TKey : notnull
    where TValue : class {
    private readonly Dictionary<TKey, TrackingObject<TValue>> _Items;
    private readonly IExtractKey<TValue, TKey> _ExtractKey;

    public int Count => this._Items.Count;

    public ICollection<TKey> Keys {
        get {
            var result = new List<TKey>(this._Items.Count);
            foreach (var kv in this._Items) {
                result.Add(kv.Key);
            }
            return result;
        }
    }

    public ICollection<TValue> Values {
        get {
            var result = new List<TValue>(this._Items.Count);
            foreach (var kv in this._Items) {
                result.Add(kv.Value.Value);
            }
            return result;
        }
    }

    public void Clear() {
        this._Items.Clear();
    }

    public TValue this[TKey key] {
        get {
            return this._Items[key].Value;
        }
    }

    public TrackingSet(
        IExtractKey<TValue, TKey> extractKey,
        IEqualityComparer<TKey> comparer,
        TrackingContext trackingContext,
        ITrackingSetApplyChanges<TValue> trackingApplyChanges
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
    [return: NotNullIfNotNull("item")]
    public override TrackingObject<TValue>? Attach(TValue? item) {
        if (item is null) {
            return default;
        } else {
            var key = this._ExtractKey.ExtractKey(item);
            if (this._Items.TryGetValue(key, out var found)) {
                this.AttachValidate(item);
                var replace = ((found.Status == TrackingStatus.Original) || this.AttachConflictReplace(item, found));
                if (replace) {
                    found.Set(item, TrackingStatus.Original);
                } else { 
                    // TODO remove changes ?
                }
                return found;
            } else {
                this.AttachValidate(item);
                var result = new TrackingObject<TValue>(
                    value: item,
                    status: TrackingStatus.Original,
                    trackingSet: this
                );
                this._Items.Add(key, result);
                return result;
            }
        }
    }
    
    public override List<TrackingObject<TValue>> AttachRange(IEnumerable<TValue> items) {
        var result = new List<TrackingObject<TValue>>();
        foreach (var item in items) {
            var to=this.Attach(item);
            result.Add(to);
        }
        return result;
    }

    public override void Detach(TrackingObject<TValue> value) {
        var key = this._ExtractKey.ExtractKey(value.Value);
        this._Items.Remove(key);
    }

    public override TrackingObject<TValue> Add(TValue value) {
        var key = this._ExtractKey.ExtractKey(value);
        if (this._Items.TryGetValue(key, out var result)) {
            throw new InvalidOperationException($"dupplicate key:{key}");
        } else {
            result = new TrackingObject<TValue>(
               value: value,
               status: TrackingStatus.Added,
               trackingSet: this
               );
            this._Items.Add(key, result);
            this.TrackingContext.TrackingChanges.Add(new TrackingChange(TrackingStatus.Added, result));
            return result;
        }
    }

    public override TrackingObject<TValue> Update(TValue value) {
        var key = this._ExtractKey.ExtractKey(value);
        if (this._Items.TryGetValue(key, out var result)) {
            if (result.Status == TrackingStatus.Original) {
                result.Set(value, TrackingStatus.Modified);
                this.Updating(value);
                this.TrackingContext.TrackingChanges.Add(new TrackingChange(TrackingStatus.Modified, result));
                return result;
            }
            if (result.Status == TrackingStatus.Added) {
                this.Updating(value);
                result.Set(value, TrackingStatus.Added);
                // skip this.TrackingContext.TrackingChanges
                return result;
            }
            if (result.Status == TrackingStatus.Modified) {
                this.Updating(value);
                result.Set(value, TrackingStatus.Modified);
                // skip this.TrackingContext.TrackingChanges
                return result;
            }
            if (result.Status == TrackingStatus.Deleted) {
                throw new InvalidOperationException("item is already deleted.");
            }
            throw new InvalidOperationException($"unknown status {result.Status}");
        } else {
            throw new InvalidOperationException($"item:{key} does not exists.");
        }
    }

    public override TrackingObject<TValue> Upsert(TValue value) {
        var key = this._ExtractKey.ExtractKey(value);
        if (this._Items.TryGetValue(key, out var result)) {
            if (result.Status == TrackingStatus.Original) {
                this.Updating(value);
                result.Set(value, TrackingStatus.Modified);
                this.TrackingContext.TrackingChanges.Add(new TrackingChange(TrackingStatus.Modified, result));
                return result;
            }
            if (result.Status == TrackingStatus.Added) {
                this.Updating(value);
                result.Set(value, TrackingStatus.Added);
                // skip this.TrackingContext.TrackingChanges
                return result;
            }
            if (result.Status == TrackingStatus.Modified) {
                this.Updating(value);
                result.Set(value, TrackingStatus.Modified);
                // skip this.TrackingContext.TrackingChanges
                return result;
            }
            if (result.Status == TrackingStatus.Deleted) {
                throw new InvalidOperationException("item is already deleted.");
            }
            throw new InvalidOperationException($"unknown state:{result.Status}");
        } else {
            this.Adding(value);
            result = new TrackingObject<TValue>(
               value: value,
               status: TrackingStatus.Added,
               trackingSet: this
               );
            this._Items.Add(key, result);
            this.TrackingContext.TrackingChanges.Add(new TrackingChange(TrackingStatus.Added, result));
            return result;
        }
    }

    public void Delete(TValue value) {
        var key = this._ExtractKey.ExtractKey(value);

        if (this._Items.TryGetValue(key, out var result)) {
            if (ReferenceEquals(result.GetValue(), value)) {
                if (result.Status == TrackingStatus.Original) {
                    this.Deleting(value);
                    result.Set(value, TrackingStatus.Deleted);
                    this._Items.Remove(key);
                    this.TrackingContext.TrackingChanges.Add(new TrackingChange(TrackingStatus.Deleted, result));
                    return;
                }
                if (result.Status == TrackingStatus.Deleted) {
                    // already deleted, but found???
                    throw new InvalidOperationException("item not found.");
                }
                if (result.Status == TrackingStatus.Added) {
                    // created and deleted
                    this.Deleting(value);
                    this.TrackingContext.TrackingChanges.Remove(TrackingStatus.Added, result);
                    result.Set(value, TrackingStatus.Deleted);
                    this._Items.Remove(key);
                    return;
                }
                if (result.Status == TrackingStatus.Modified) {
                    this.Deleting(value);
                    this.TrackingContext.TrackingChanges.Remove(TrackingStatus.Modified, result);
                    result.Set(value, TrackingStatus.Deleted);
                    this._Items.Remove(key);
                    this.TrackingContext.TrackingChanges.Add(new TrackingChange(TrackingStatus.Deleted, result));
                    return;
                }
                if (result.Status == TrackingStatus.Deleted) {
                    throw new InvalidOperationException("item Delete found.");
                }
                throw new InvalidOperationException($"unknown state:{result.Status}");
            } else {
                throw new InvalidOperationException("item not found.");
            }
        } else {
            throw new InvalidOperationException("item not found.");
        }
    }

    protected internal override void ValueSet(TrackingObject<TValue> trackingObject, TValue value) {
        if (trackingObject.Status == TrackingStatus.Original) {
            this.Updating(value);
            trackingObject.Set(value, TrackingStatus.Modified);
            this.TrackingContext.TrackingChanges.Add(
                new TrackingChange(Tracking.TrackingStatus.Modified, trackingObject)
                ); ;
        } else if (trackingObject.Status == TrackingStatus.Added) {
            this.Updating(value);
            trackingObject.Set(value, TrackingStatus.Added);
            // TrackingChange should be there
        } else if (trackingObject.Status == TrackingStatus.Modified) {
            trackingObject.Set(value, TrackingStatus.Modified);
            this.Updating(value);
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
            var key = this._ExtractKey.ExtractKey(trackingObject.Value);
            if (this._Items.TryGetValue(key, out var found)) {
                if (found.Status == TrackingStatus.Original) {
                    this.Deleting(trackingObject.Value);
                    found.Set(trackingObject.Value, TrackingStatus.Deleted);
                    this._Items.Remove(key);
                    this.TrackingContext.TrackingChanges.Add(new TrackingChange(TrackingStatus.Deleted, found));
                    return;
                }
                if (found.Status == TrackingStatus.Added) {
                    // created and deleted
                    this.Deleting(trackingObject.Value);
                    found.Set(trackingObject.Value, TrackingStatus.Deleted);
                    this._Items.Remove(key);
                    this.TrackingContext.TrackingChanges.Remove(TrackingStatus.Added, found);
                    return;
                }
                if (found.Status == TrackingStatus.Modified) {
                    this.Deleting(trackingObject.Value);
                    found.Status = TrackingStatus.Deleted;
                    this._Items.Remove(key);
                    return;
                }
                if (found.Status == TrackingStatus.Deleted) {
                    // already deleted, but found???
                    this._Items.Remove(key);
                    return;
                }
            } else {
                throw new InvalidOperationException("item not found.");
            }
        }
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) {
        if (this._Items.TryGetValue(key, out var found)) {
            value = found.Value;
            return true;
        } else {
            value = default;
            return false;
        }
    }

    public IEnumerable<TrackingObject<TValue>> GetTrackingObjects()
        => this._Items.Values;

    public TrackingObject<TValue> GetTrackingObject(TKey key) => this._Items[key];

    public bool TryTrackingObject(TKey key, [MaybeNullWhen(false)] out TrackingObject<TValue> value) {
        return this._Items.TryGetValue(key, out value);
    }

    public virtual void AttachValidate(TValue item) {
    }

    public virtual bool AttachConflictReplace(TValue item, TrackingObject<TValue> found) {
        return true;
    }
    public virtual void Adding(TValue item) {
    }
    public virtual void Updating(TValue item) {
    }
    public virtual void Deleting(TValue item) {
    }
}
