namespace Brimborium.RowVersion.Entity;
public static class AggregateRecordList {
    public static AggregateRecordList<T> ToAggregateRecordList<T>(this IEnumerable<T> src)
        where T : IEntityWithVersion {
        var result = new AggregateRecordList<T>();
        foreach (var item in src) {
            result.Add(item);
        }
        return result;
    }

    public static AggregateRecordList<T> ToAggregateRecordList<T>(this IEnumerable<T> src, Func<T, bool> predicate)
        where T : IEntityWithVersion {
        var result = new AggregateRecordList<T>();
        foreach (var item in src) {
            if (predicate(item)) {
                result.Add(item);
            }
        }
        return result;
    }

    public static AggregateRecordList<T> ToAggregateRecordListValidRange<T>(IEnumerable<T> src, DateTime at)
        where T : IDTOValidRange, IEntityWithVersion {
        var result = new AggregateRecordList<T>();
        foreach (var item in src) {
            if (item.WithinValidRange<T>(at)) {
                result.Add(item);
            }
        }
        return result;
    }

    public static AggregateRecordList<T> ToAggregateRecordListValidRangeQ<T>(IEnumerable<T> src, DateTime at)
        where T : IDTOValidRangeQ, IEntityWithVersion {
        var result = new AggregateRecordList<T>();
        foreach (var item in src) {
            if (item.WithinValidRangeQ<T>(at)) {
                result.Add(item);
            }
        }
        return result;
    }

    public static AggregateRecordList<T> ToAggregateRecordListSorted<T>(this IEnumerable<T> src, Func<T, T, int> compare)
        where T : IEntityWithVersion {
        var result = new AggregateRecordList<T>();
        foreach (var item in src) {
            result.Add(item, compare);
        }
        return result;
    }
}

public class AggregateRecordList<T> where T : IEntityWithVersion {
    private readonly List<T> _List;
    private AggregationEntityVersion _AggregatedEntityVersion;

    public AggregateRecordList() {
        this._List = new List<T>();
    }

    public List<T> List => this._List;

    public AggregationEntityVersion AggregatedEntityVersion => this._AggregatedEntityVersion;

    public void Add(T item) {
        item.EntityVersion.MaxEntityVersion(ref this._AggregatedEntityVersion);
        this._List.Add(item);
    }

    public void Add(T item, Func<T, T, int> compare) {
        item.EntityVersion.MaxEntityVersion(ref this._AggregatedEntityVersion);
        var pos = this._List.BinarySearchByValue(item, compare);
        if (pos < 0) {
            this._List.Insert(~pos, item);
        } else {
            this._List.Insert(pos + 1, item);
        }
    }
}
