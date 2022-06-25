namespace Brimborium.Tracking;

public interface ITrackingSet {
    ITrackingContext TrackingContext { get; }
}

public interface ITrackingSet<TValue>
    : ITrackingSet
    where TValue : class {
    ITrackingSetApplyChanges<TValue> TrackingApplyChanges { get; }
    int Count { get; }
    ICollection<TValue> Values { get; }
    
    void Clear();

    [return: NotNullIfNotNull("item")]
    TrackingObject<TValue>? Attach(TValue? item);

    List<TrackingObject<TValue>> AttachRange(IEnumerable<TValue> items);

    void Detach(TrackingObject<TValue>? item);

    TrackingObject<TValue> Add(TValue item);
    TrackingObject<TValue> Update(TValue item);
    TrackingObject<TValue> Upsert(TValue item);
    void Delete(TrackingObject<TValue> trackingObject);
    void Delete(TValue item);

    IEnumerable<TrackingObject<TValue>> GetTrackingObjects();
}

public interface ITrackingSet<TKey, TValue>
    : ITrackingSet<TValue>
    where TKey : notnull
    where TValue : class {
    TValue this[TKey key] { get; }

    ICollection<TKey> Keys { get; }

    /*
        [return: NotNullIfNotNull("item")]
        TrackingObject<TValue>? Attach(TValue? item);
        List<TrackingObject<TValue>> AttachRange(IEnumerable<TValue> items);
        
        void Detach(TrackingObject<TValue>? item);
      
        TrackingObject<TValue> Add(TValue item);
        TrackingObject<TValue> Update(TValue item);
        TrackingObject<TValue> Upsert(TValue item);
        void Delete(TrackingObject<TValue> trackingObject);
        void Delete(TValue item);
    */

    TrackingObject<TValue> GetTrackingObject(TKey key);
    bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);
    bool TryTrackingObject(TKey key, [MaybeNullWhen(false)] out TrackingObject<TValue> value);
}
