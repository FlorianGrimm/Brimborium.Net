namespace Brimborium.Tracking;

public interface ITrackingSet {
    ITrackingContext TrackingContext { get; }
}


public interface ITrackingSet<TKey, TValue>
    : ITrackingSet
    where TKey : notnull
    where TValue : class {
    ITrackingSetApplyChanges<TKey, TValue> TrackingApplyChanges { get; }

    TValue this[TKey key] { get; }

    ICollection<TKey> Keys { get; }

    int Count { get; }
    ICollection<TValue> Values { get; }

    void Clear();

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
