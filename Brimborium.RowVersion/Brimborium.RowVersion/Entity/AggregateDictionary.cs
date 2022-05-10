namespace Brimborium.RowVersion.Entity;

public static class AggregateDictionary {
    public static AggregateDictionary<K, V> ToAggregateDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> src)
        where K : notnull
        where V : notnull, IEntityWithVersion {
        var result = new AggregateDictionary<K, V>();
        foreach (var item in src) {
            result.Add(item.Key, item.Value);
        }
        return result;
    }

    public static AggregateDictionary<K, V> ToAggregateDictionary<T, K, V>(this IEnumerable<T> src, Func<T, K> getKey, Func<T, V> getValue)
        where K : notnull
        where V : notnull, IEntityWithVersion {
        var result = new AggregateDictionary<K, V>();
        foreach (var item in src) {
            var key = getKey(item);
            var value = getValue(item);
            result.Add(key, value);
        }
        return result;
    }


    public static AggregateDictionary<K, V> ToAggregateDictionaryValidRange<T, K, V>(this IEnumerable<T> src, DateTime at, Func<T, K> getKey, Func<T, V> getValue)
        where K : notnull
        where V : notnull, IDTOValidRange, IEntityWithVersion {
        var result = new AggregateDictionary<K, V>();
        foreach (var item in src) {
            var value = getValue(item);
            if (value.WithinValidRange<V>(at)) {
                var key = getKey(item);
                result.Add(key, value);
            }
        }
        return result;
    }

    public static AggregateDictionary<K, V> ToAggregateDictionaryValidRangeQ<T, K, V>(this IEnumerable<T> src, DateTime at, Func<T, K> getKey, Func<T, V> getValue)
        where K : notnull
        where V : notnull, IDTOValidRangeQ, IEntityWithVersion {
        var result = new AggregateDictionary<K, V>();
        foreach (var item in src) {
            var value = getValue(item);
            if (value.WithinValidRangeQ<V>(at)) {
                var key = getKey(item);
                result.Add(key, value);
            }
        }
        return result;
    }
}

public class AggregateDictionary<K, V>
    where K : notnull
    where V : notnull, IEntityWithVersion {
    private readonly Dictionary<K, V> _Dict;
    private AggregationEntityVersion _EntityVersion;

    public AggregateDictionary() {
        this._Dict = new Dictionary<K, V>();
    }

    public Dictionary<K, V> Dict => this._Dict;

    public AggregationEntityVersion EntityVersion => this._EntityVersion;

    public void Add(K key, V item) {
        this._Dict[key] = item;
        item.EntityVersion.MaxEntityVersion(ref this._EntityVersion);
    }
}
