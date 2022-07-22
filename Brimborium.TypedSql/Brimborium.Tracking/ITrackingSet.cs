namespace Brimborium.Tracking;

public interface ITrackingSet {
    /// <summary>
    /// Owner
    /// </summary>
    ITrackingContext TrackingContext { get; }
 
    int Count { get; }
    
    void Clear();

    /// <summary>
    /// ReadOnly allows only Attach and Clear.
    /// </summary>
    bool IsReadOnly { get; set; }
}

public interface ITrackingSet<TValue>
    : ITrackingSet
    where TValue : class {
    ICollection<TValue> Values { get; }
}

public interface ITrackingSet<TKey, TValue>
    : ITrackingSet<TValue>
    where TKey : notnull
    where TValue : class {
    ITrackingSetApplyChanges<TKey, TValue> TrackingApplyChanges { get; }

    TValue this[TKey key] { get; }

    ICollection<TKey> Keys { get; }

    // ICollection<TValue> Values { get; }

    [return: NotNullIfNotNull("item")]
    TrackingObject<TKey, TValue>? Attach(TValue? item);

    List<TrackingObject<TKey, TValue>> AttachRange(IEnumerable<TValue> items);

    void Detach(TrackingObject<TKey, TValue>? item);

    TrackingObject<TKey, TValue> Add(TValue item);

    TrackingObject<TKey, TValue> Update(TValue item);

    TrackingObject<TKey, TValue> Upsert(TValue item);

    void Delete(TrackingObject<TKey, TValue> trackingObject);

    void Delete(TValue item);

    IEnumerable<TrackingObject<TKey, TValue>> GetTrackingObjects();
    /*
        [return: NotNullIfNotNull("item")]
        TrackingObject<TKey, TValue>? Attach(TValue? item);
        List<TrackingObject<TKey, TValue>> AttachRange(IEnumerable<TValue> items);
        
        void Detach(TrackingObject<TKey, TValue>? item);
      
        TrackingObject<TKey, TValue> Add(TValue item);
        TrackingObject<TKey, TValue> Update(TValue item);
        TrackingObject<TKey, TValue> Upsert(TValue item);
        void Delete(TrackingObject<TKey, TValue> trackingObject);
        void Delete(TValue item);
    */

    TrackingObject<TKey, TValue> GetTrackingObject(TKey key);

    bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);

    bool TryTrackingObject(TKey key, [MaybeNullWhen(false)] out TrackingObject<TKey, TValue> value);
}
