namespace Brimborium.Tracking;

public interface ITrackingSet<TKey, TValue>
    where TKey : notnull
    where TValue : class {
    TValue this[TKey key] { get; }

    int Count { get; }
    ICollection<TKey> Keys { get; }
    ICollection<TValue> Values { get; }
    void Clear();

    [return: NotNullIfNotNull("item")]
    TrackingObject<TValue>? Attach(TValue? item);
    List<TrackingObject<TValue>> AttachRange(IEnumerable<TValue> items);
    void Detach(TrackingObject<TValue> item);

    TrackingObject<TValue> Add(TValue item);
    TrackingObject<TValue> Update(TValue item);
    TrackingObject<TValue> Upsert(TValue item);
    void Delete(TrackingObject<TValue> trackingObject);
    void Delete(TValue item);

    TrackingObject<TValue> GetTrackingObject(TKey key);
    IEnumerable<TrackingObject<TValue>> GetTrackingObjects();
    bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);
    bool TryTrackingObject(TKey key, [MaybeNullWhen(false)] out TrackingObject<TValue> value);
}
