namespace Brimborium.Tracking;

public abstract class TrackingSet : ITrackingSet {
    protected TrackingSet(ITrackingContext trackingContext) {
        this.TrackingContext = trackingContext;
        if (trackingContext is TrackingContext aTrackingContext) {
            aTrackingContext.RegisterTrackingSet(this);
        }
    }

    public ITrackingContext TrackingContext { get; }

    public abstract int Count { get; }

    public abstract void Clear();

    /// <summary>
    /// ReadOnly allows only Attach and Clear.
    /// </summary>
    public bool IsReadOnly { get; set; }

    internal abstract Type GetItemType();
}

//public abstract class TrackingSet< TValue>
//    : TrackingSet
//    //, ITrackingSet<TValue>
//    where TValue : class {

//    protected TrackingSet(
//        ITrackingContext trackingContext
//        //ITrackingSetApplyChanges<TValue> trackingApplyChanges
//        ) : base(trackingContext) {
//        //this.TrackingApplyChanges = trackingApplyChanges;
//    }


//    //public ITrackingSetApplyChanges<TValue> TrackingApplyChanges { get; }

//    internal override Type GetItemType() => typeof(TValue);

//    public abstract int Count { get; }

//    public abstract ICollection<TValue> Values { get; }

//    public abstract void Clear();

//    //public abstract TrackingObject<TKey, TValue>? Attach(TValue? item);

//    //public abstract List<TrackingObject<TKey, TValue>> AttachRange(IEnumerable<TValue> items);

//    //public abstract TrackingObject<TKey, TValue> Add(TValue item);

//    //public abstract TrackingObject<TKey, TValue> Update(TValue item);

//    //public abstract TrackingObject<TKey, TValue> Upsert(TValue item);

//    //public abstract void Detach(TrackingObject<TKey, TValue>? item);

//    //public abstract void Delete(TrackingObject<TKey, TValue> trackingObject);

//    //public abstract void Delete(TValue item);

//    //public abstract IEnumerable<TrackingObject<TKey, TValue>> GetTrackingObjects();

//    //internal protected abstract void ValueSet(TrackingObject<TKey, TValue> trackingObject, TValue value);

//    //internal protected abstract void ReAttach(TrackingObject<TKey, TValue> trackingObject);
//}

public class TrackingSet<TKey, TValue>
    : TrackingSet
    , ITrackingSet<TValue>
    , ITrackingSet<TKey, TValue>
    where TKey : notnull
    where TValue : class {
    protected readonly Dictionary<TKey, TrackingObject<TKey, TValue>> _Items;
    protected readonly IExtractKey<TKey, TValue> _ExtractKey;
    protected readonly List<ITrackingSetEvent<TKey, TValue>> _TrackingSetEvents;

    internal override Type GetItemType() => typeof(TValue);

    public override int Count => this._Items.Count;

    public ICollection<TKey> Keys {
        get {
            var result = new List<TKey>(this._Items.Count);
            foreach (var kv in this._Items) {
                result.Add(kv.Key);
            }
            return result;
        }
    }

    public virtual ICollection<TValue> Values {
        get {
            var result = new List<TValue>(this._Items.Count);
            foreach (var kv in this._Items) {
                result.Add(kv.Value.Value);
            }
            return result;
        }
    }

    public override void Clear() {
        this._Items.Clear();
        this.ItemsChanged();
    }

    public TValue this[TKey key] {
        get {
            return this._Items[key].Value;
        }
    }

    public TrackingSet(
        IExtractKey<TKey, TValue> extractKey,
        IEqualityComparer<TKey> comparer,
        ITrackingContext trackingContext,
        ITrackingSetApplyChanges<TKey, TValue> trackingApplyChanges,
        List<ITrackingSetEvent<TKey, TValue>>? trackingSetEvents = default
        ) : base(trackingContext) {
        this._Items = new Dictionary<TKey, TrackingObject<TKey, TValue>>(comparer);
        this._TrackingSetEvents = new List<ITrackingSetEvent<TKey, TValue>>();
        this._ExtractKey = extractKey;
        this.TrackingApplyChanges = trackingApplyChanges;
        if (trackingSetEvents is not null) {
            foreach (var trackingSetEvent in trackingSetEvents) {
                this._TrackingSetEvents.Add(trackingSetEvent);
            }
        }
    }

    public List<ITrackingSetEvent<TKey, TValue>> TrackingSetEvents => this._TrackingSetEvents;

    public ITrackingSetApplyChanges<TKey, TValue> TrackingApplyChanges { get; }

    public bool TryExtractKey(TValue value, [MaybeNullWhen(false)] out TKey key) {
        return this._ExtractKey.TryExtractKey(value, out key);
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
    public virtual TrackingObject<TKey, TValue>? Attach(TValue? item) {
        if (item is null) {
            return default;
        } else {
            if (!this._ExtractKey.TryExtractKey(item, out var key)) {
                throw new InvalidModificationException("Invalid primary key", "PrimaryKey", "{}", nameof(TValue));
            } else {
                if (this._Items.TryGetValue(key, out var found)) {
                    this.AttachValidate(item);
                    var replace = ((found.Status == TrackingStatus.Original) || this.AttachConflictReplace(item, found));
                    if (replace) {
                        found.Set(item, TrackingStatus.Original);
                        this.ItemsChanged();
                    } else {
                        // TODO remove changes ?
                    }
                    return found;
                } else {
                    this.AttachValidate(item);
                    var result = new TrackingObject<TKey, TValue>(
                        key: key,
                        value: item,
                        status: TrackingStatus.Original,
                        trackingSet: this
                    );
                    this.ItemsChanged();
                    this._Items.Add(result.Key, result);
                    return result;
                }
            }
        }
    }

    public virtual List<TrackingObject<TKey, TValue>> AttachRange(IEnumerable<TValue>? items) {
        var result = new List<TrackingObject<TKey, TValue>>();
        if (items is not null) {
            foreach (var item in items) {
                var to = this.Attach(item);
                result.Add(to);
            }
        }
        return result;
    }

    public virtual void ClearAndAttachRange(IEnumerable<TValue>? items, Func<TValue, TValue, bool>? itemEqual = default) {
        if (items is not null) {
            if (itemEqual is not null) {
                var bEq = (this.Count == items.Count());
                if (bEq) {
                    foreach (var item in items) {
                        if (this.TryExtractKey(item, out var foundKey)) {
                            if (this.TryGetValue(foundKey, out var foundValue)) {
                                if (itemEqual(foundValue, item)) {
                                    //
                                } else {
                                    bEq = false;
                                    break;
                                }
                            } else {
                                bEq = false;
                                break;
                            }
                        } else {
                            break;
                        }
                    }
                }
                if (bEq) {
                    return;
                }
            }

            this.Clear();
            foreach (var item in items) {
                this.Attach(item);
            }
        } else {
            this.Clear();
        }
    }

    public virtual void Detach(TrackingObject<TKey, TValue>? trackingObject) {
        if (trackingObject is not null) {
            if (this._ExtractKey.TryExtractKey(trackingObject.Value, out var key)) {
                this._Items.Remove(key);
                this.ItemsChanged();
            }
        }
    }

    protected internal virtual void ReAttach(TKey oldKey, TrackingObject<TKey, TValue> trackingObject) {
        //if (!this._ExtractKey.TryExtractKey(trackingObject.Value, out var key)) {
        //    throw new InvalidModificationException("Invalid primary key", "PrimaryKey", "{}");
        //} else {
        if (this._Items.TryGetValue(oldKey, out var toOld)) {
            if (ReferenceEquals(toOld, trackingObject) && oldKey.Equals(trackingObject.Key)) {
                // do nothing
            } else {
                this._Items.Remove(oldKey);
                this._Items.Add(trackingObject.Key, trackingObject);
                this.ItemsChanged();
            }
        }
        // this._Items[key] = trackingObject;
        //}
    }

    public virtual TrackingObject<TKey, TValue> Add(TValue value) {
        if (this.IsReadOnly) { throw new InvalidOperationException("Not allowed it's IsReadOnly"); }

        var isValidKey = this._ExtractKey.TryExtractKey(value, out var key);
        if (isValidKey
            && (key is not null)
            && this._Items.TryGetValue(key, out _)) {
            throw new InvalidModificationException($"dupplicate key:{key}");
        } else {
            value = this.OnAdding(value);
            /*
            var keyFinally = this._ExtractKey.ExtractKey(value);
            */
            isValidKey = this._ExtractKey.TryExtractKey(value, out var keyFinally);
            if (!isValidKey
                || (keyFinally is null)) {
                throw new InvalidModificationException($"Invalid primary key", "PrimaryKey", "{}", typeof(TValue).Name);
            }

            if ((key is not null)
                && keyFinally.Equals(key)) {
                // no change -> no need test again
            } else {
                if (this._Items.TryGetValue(keyFinally, out _)) {
                    throw new InvalidModificationException($"dupplicate key:{key}", "PrimaryKey", keyFinally.ToString() ?? "{}", typeof(TValue).Name);
                }
            }
            var result = new TrackingObject<TKey, TValue>(
                key: keyFinally,
                value: value,
                status: TrackingStatus.Added,
                trackingSet: this
                );
            this._Items.Add(result.Key, result);
            this.TrackingContext.TrackingChanges.Add(result);
            this.ItemsChanged();
            return result;
        }
    }

    public virtual TrackingObject<TKey, TValue> Update(TValue value) {
        if (this.IsReadOnly) { throw new InvalidOperationException("Not allowed it's IsReadOnly"); }

        if (!this._ExtractKey.TryExtractKey(value, out var key)) {
            throw new InvalidModificationException("Invalid primary key", "PrimaryKey", "{}", typeof(TValue).Name);
        } else {
            if (this._Items.TryGetValue(key, out var result)) {
                if (result.Status == TrackingStatus.Original) {
                    value = this.OnUpdating(newValue: value, oldValue: result.Value, TrackingStatus.Original, TrackingStatus.Modified);
                    result.Set(value, TrackingStatus.Modified);
                    this.TrackingContext.TrackingChanges.Add(result);
                    this.ItemsChanged();
                    return result;
                }
                if (result.Status == TrackingStatus.Added) {
                    value = this.OnUpdating(newValue: value, oldValue: result.Value, TrackingStatus.Added, TrackingStatus.Added);
                    result.Set(value, TrackingStatus.Added);
                    // skip this.TrackingContext.TrackingChanges
                    this.ItemsChanged();
                    return result;
                }
                if (result.Status == TrackingStatus.Modified) {
                    value = this.OnUpdating(newValue: value, oldValue: result.Value, TrackingStatus.Modified, TrackingStatus.Modified);
                    result.Set(value, TrackingStatus.Modified);
                    // skip this.TrackingContext.TrackingChanges
                    this.ItemsChanged();
                    return result;
                }
                if (result.Status == TrackingStatus.Deleted) {
                    throw new InvalidModificationException("item is already deleted.");
                }
                throw new InvalidModificationException($"unknown status {result.Status}");
            } else {
                throw new InvalidModificationException($"item:{key} does not exists.");
            }
        }
    }

    public virtual TrackingObject<TKey, TValue> Upsert(TValue value) {
        if (this.IsReadOnly) { throw new InvalidOperationException("Not allowed it's IsReadOnly"); }

        TrackingObject<TKey, TValue>? result = default;

        var isValidKey = this._ExtractKey.TryExtractKey(value, out var key);

        bool isValidKeyFinally;

        int mode;
        if (!isValidKey) {
            value = this.OnAdding(value);
            mode = 1; // adding
            isValidKeyFinally = this._ExtractKey.TryExtractKey(value, out var keyFinally);
            if (!isValidKeyFinally) {
                throw new InvalidModificationException("Invalid primary key", "PrimaryKey", "{}", typeof(TValue).Name);
            } else {
                key = keyFinally;
            }
        } else {
            if (!this._Items.TryGetValue(key!, out result)) {
                value = this.OnAdding(value);
                isValidKeyFinally = this._ExtractKey.TryExtractKey(value, out var keyFinally);
                if (!isValidKeyFinally) {
                    throw new InvalidModificationException("Invalid primary key", "PrimaryKey", "{}", typeof(TValue).Name);
                } else {
                    if ((key is not null)
                        && (keyFinally is not null)
                        && key.Equals(keyFinally)) {
                        // OK
                        mode = 1; // adding
                    } else if ((keyFinally is not null)
                        && this._Items.TryGetValue(keyFinally, out result)) {
                        mode = 2; // updateing
                    } else {
                        key = keyFinally;
                        mode = 1; // adding
                    }
                }
            } else {
                mode = 2; // updateing
            }
        }
        if (mode == 0) {
            throw new InvalidOperationException("unexpected mode:0;");
        } else if (mode == 1) {
            if (key is null) {
                throw new InvalidOperationException("mode:1; key is null");
            }
            result = new TrackingObject<TKey, TValue>(
                key: key,
                value: value,
                status: TrackingStatus.Added,
                trackingSet: this
                );
            this.TrackingContext.TrackingChanges.Add(result);
            this._Items.Add(result.Key, result);
            this.ItemsChanged();
            return result;
        } else if (mode == 2) {
            if (key is null) {
                throw new InvalidOperationException("mode:1; key is null");
            }
            if (result is null) {
                throw new InvalidOperationException("mode:1; result is null");
            }
            if (result.Status == TrackingStatus.Original) {
                value = this.OnUpdating(newValue: value, oldValue: result.Value, TrackingStatus.Original, TrackingStatus.Modified);
                result.Set(value, TrackingStatus.Modified);
                this.TrackingContext.TrackingChanges.Add(result);
                this.ItemsChanged();
                return result;
            }
            if (result.Status == TrackingStatus.Added) {
                value = this.OnUpdating(newValue: value, oldValue: result.Value, TrackingStatus.Added, TrackingStatus.Added);
                result.Set(value, TrackingStatus.Added);
                // skip this.TrackingContext.TrackingChanges
                this.ItemsChanged();
                return result;
            }
            if (result.Status == TrackingStatus.Modified) {
                value = this.OnUpdating(newValue: value, oldValue: result.Value, TrackingStatus.Modified, TrackingStatus.Modified);
                result.Set(value, TrackingStatus.Modified);
                // skip this.TrackingContext.TrackingChanges
                this.ItemsChanged();
                return result;
            }
            if (result.Status == TrackingStatus.Deleted) {
                throw new InvalidModificationException("item is already deleted.");
            }
            throw new InvalidModificationException($"unknown state:{result.Status}");
        } else {
            throw new InvalidOperationException($"unexpected mode:{mode};");
        }

    }

    public virtual void Delete(TValue value) {
        if (this.IsReadOnly) { throw new InvalidOperationException("Not allowed it's IsReadOnly"); }

        if (!this._ExtractKey.TryExtractKey(value, out var key)) {
            throw new InvalidModificationException("Invalid primary key", "PrimaryKey", "{}", typeof(TValue).Name);
        } else {
            if (this._Items.TryGetValue(key, out var result)) {
                //if (ReferenceEquals(result.GetValue(), trackingObject)) {
                if (result.Status == TrackingStatus.Original) {
                    value = this.OnDeleting(newValue: value, oldValue: result.Value, TrackingStatus.Original);
                    result.Set(value, TrackingStatus.Deleted);
                    this._Items.Remove(key);
                    this.TrackingContext.TrackingChanges.Add(result);
                    this.ItemsChanged();
                    return;
                }
                if (result.Status == TrackingStatus.Deleted) {
                    // already deleted, but found???
                    throw new InvalidModificationException("item not found.");
                }
                if (result.Status == TrackingStatus.Added) {
                    // created and deleted
                    value = this.OnDeleting(newValue: value, oldValue: result.Value, TrackingStatus.Added);
                    this.TrackingContext.TrackingChanges.Remove(result);
                    result.Set(value, TrackingStatus.Deleted);
                    this._Items.Remove(key);
                    this.ItemsChanged();
                    return;
                }
                if (result.Status == TrackingStatus.Modified) {
                    value = this.OnDeleting(newValue: value, oldValue: result.Value, TrackingStatus.Modified);
                    this.TrackingContext.TrackingChanges.Remove(result);
                    result.Set(value, TrackingStatus.Deleted);
                    this._Items.Remove(key);
                    this.TrackingContext.TrackingChanges.Add(result);
                    this.ItemsChanged();
                    return;
                }
                if (result.Status == TrackingStatus.Deleted) {
                    throw new InvalidModificationException("item Delete found.");
                }
                throw new InvalidModificationException($"unknown state:{result.Status}");
            } else {
                throw new InvalidModificationException("item not found.");
            }
        }
    }

    protected internal virtual void ValueSet(TrackingObject<TKey, TValue> trackingObject, TValue value) {
        if (this.IsReadOnly) { throw new InvalidOperationException("Not allowed it's IsReadOnly"); }

        if (trackingObject.Status == TrackingStatus.Original) {
            value = this.OnUpdating(newValue: value, oldValue: trackingObject.Value, TrackingStatus.Original, TrackingStatus.Modified);
            trackingObject.Set(value, TrackingStatus.Modified);
            this.TrackingContext.TrackingChanges.Add(trackingObject);
            this.ItemsChanged();
        } else if (trackingObject.Status == TrackingStatus.Added) {
            value = this.OnUpdating(newValue: value, oldValue: trackingObject.Value, TrackingStatus.Added, TrackingStatus.Added);
            trackingObject.Set(value, TrackingStatus.Added);
            // TrackingChange should be there
            this.ItemsChanged();
        } else if (trackingObject.Status == TrackingStatus.Modified) {
            value = this.OnUpdating(newValue: value, oldValue: trackingObject.Value, TrackingStatus.Modified, TrackingStatus.Modified);
            trackingObject.Set(value, TrackingStatus.Modified);
            // TrackingChange should be there
            this.ItemsChanged();
        } else if (trackingObject.Status == TrackingStatus.Deleted) {
            throw new System.InvalidOperationException("The object is deleted.");
        }
    }

    public virtual void Delete(TrackingObject<TKey, TValue> trackingObject) {
        if (this.IsReadOnly) { throw new InvalidOperationException("Not allowed it's IsReadOnly"); }

        //base.Delete(trackingObject);
        if (!ReferenceEquals(trackingObject.TrackingSet, this)) {
            throw new InvalidModificationException("wrong TrackingSet");
        } else {
            if (!this._ExtractKey.TryExtractKey(trackingObject.Value, out var key)) {
                throw new InvalidModificationException("Invalid primary key", "PrimaryKey", "{}", typeof(TValue).Name);
            } else {
                if (this._Items.TryGetValue(key, out var found)) {
                    if (found.Status == TrackingStatus.Original) {
                        var value = this.OnDeleting(newValue: trackingObject.Value, oldValue: found.Value, TrackingStatus.Original);
                        found.Set(value, TrackingStatus.Deleted);
                        this._Items.Remove(key);
                        this.TrackingContext.TrackingChanges.Add(found);
                        this.ItemsChanged();
                        return;
                    }
                    if (found.Status == TrackingStatus.Added) {
                        // created and deleted
                        var value = this.OnDeleting(newValue: trackingObject.Value, oldValue: found.Value, TrackingStatus.Original);
                        found.Set(value, TrackingStatus.Deleted);
                        this._Items.Remove(key);
                        this.TrackingContext.TrackingChanges.Remove(found);
                        this.ItemsChanged();
                        return;
                    }
                    if (found.Status == TrackingStatus.Modified) {
                        var value = this.OnDeleting(newValue: trackingObject.Value, oldValue: found.Value, TrackingStatus.Original);
                        //found.Status = TrackingStatus.Deleted;
                        found.Set(value, TrackingStatus.Deleted);
                        this._Items.Remove(key);
                        this.ItemsChanged();
                        return;
                    }
                    if (found.Status == TrackingStatus.Deleted) {
                        // already deleted, but found???
                        this._Items.Remove(key);
                        this.ItemsChanged();
                        return;
                    }
                } else {
                    throw new InvalidModificationException("item not found.");
                }
            }
        }
    }

    public virtual bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) {
        if (this._Items.TryGetValue(key, out var found)) {
            value = found.Value;
            return true;
        } else {
            value = default;
            return false;
        }
    }

    public virtual IEnumerable<TrackingObject<TKey, TValue>> GetTrackingObjects()
        => this._Items.Values;

    public TrackingObject<TKey, TValue> GetTrackingObject(TKey key) => this._Items[key];

    public virtual bool TryTrackingObject(TKey key, [MaybeNullWhen(false)] out TrackingObject<TKey, TValue> value) {
        return this._Items.TryGetValue(key, out value);
    }

    public virtual void AttachValidate(TValue item) {
    }

    public virtual bool AttachConflictReplace(TValue item, TrackingObject<TKey, TValue> found) {
        return true;
    }

    public virtual TValue OnAdding(TValue value) {
        var argument = new AddingArgument<TKey, TValue>(
                Value: value,
                TrackingSet: this,
                TrackingContext: this.TrackingContext
            );
        for (int idx = 0; idx < this._TrackingSetEvents.Count; idx++) {
            var trackingSetEvent = this._TrackingSetEvents[idx];
            argument = trackingSetEvent.OnAdding(argument);
        }
        return argument.Value;
    }

    public virtual TValue OnUpdating(TValue newValue, TValue oldValue, TrackingStatus oldTrackingStatus, TrackingStatus newTrackingStatus) {
        var argument = new UpdatingArgument<TKey, TValue>(
                NewValue: newValue,
                NewTrackingStatus: newTrackingStatus,
                OldValue: oldValue,
                OldTrackingStatus: oldTrackingStatus,
                TrackingSet: this,
                TrackingContext: this.TrackingContext
            );
        for (int idx = 0; idx < this._TrackingSetEvents.Count; idx++) {
            var trackingSetEvent = this._TrackingSetEvents[idx];
            argument = trackingSetEvent.OnUpdating(argument);
        }
        return argument.NewValue;
    }

    public virtual TValue OnDeleting(TValue newValue, TValue oldValue, TrackingStatus oldTrackingStatus) {
        var argument = new DeletingArgument<TKey, TValue>(
                NewValue: newValue,
                OldValue: oldValue,
                OldTrackingStatus: oldTrackingStatus,
                TrackingSet: this,
                TrackingContext: this.TrackingContext
            );
        for (int idx = 0; idx < this._TrackingSetEvents.Count; idx++) {
            var trackingSetEvent = this._TrackingSetEvents[idx];
            argument = trackingSetEvent.OnDeleting(argument);
        }
        return argument.NewValue;
    }

    protected virtual void ItemsChanged() {
    }
}
