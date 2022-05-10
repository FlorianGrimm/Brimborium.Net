namespace Brimborium.RowVersion.Extensions;

public static class ValidRangeExtension {
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool WithinValidRange<T>(this T item, DateTime at)
        where T : IDTValidRange {
        return item.ValidFrom <= at && at < item.ValidTo;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool WithinValidRange<T>(this T item, DateTimeOffset at)
    where T : IDTOValidRange {
        return item.ValidFrom <= at && at < item.ValidTo;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool WithinValidRangeQ<T>(this T item, DateTime at)
        where T : IDTValidRangeQ {
        return item.ValidFrom <= at
                && (item.ValidTo.HasValue
                    ? at < item.ValidTo.Value
                    : true);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static bool WithinValidRangeQ<T>(this T item, DateTimeOffset at)
        where T : IDTOValidRangeQ {
        return item.ValidFrom <= at
                && (item.ValidTo.HasValue
                    ? at < item.ValidTo.Value
                    : true);
    }

    public static Func<T, bool> Bind<T>(this DateTime at, Func<T, DateTime, bool> createPredicate) {
        return predicate;
        bool predicate(T item) => createPredicate(item, at);
    }

    public static Func<T, bool> Bind<T>(this DateTimeOffset at, Func<T, DateTimeOffset, bool> createPredicate) {
        return predicate;
        bool predicate(T item) => createPredicate(item, at);
    }

    public static IEnumerable<T> WhereAtValidRange<T>(this IEnumerable<T> list, DateTime at)
       where T : IDTValidRange {
        foreach (var item in list) {
            if (item.WithinValidRange(at)) {
                yield return item;
            }
        }
    }

    public static IEnumerable<T> WhereAtValidRange<T>(this IEnumerable<T> list, DateTimeOffset at)
       where T : IDTOValidRange {
        foreach (var item in list) {
            if (item.WithinValidRange(at)) {
                yield return item;
            }
        }
    }

    public static IEnumerable<T> WhereAtValidRangeQ<T>(this IEnumerable<T> list, DateTime at)
        where T : IDTValidRangeQ {
        foreach (var item in list) {
            if (item.WithinValidRangeQ(at)) {
                yield return item;
            }
        }
    }

    public static IEnumerable<T> WhereAtValidRangeQ<T>(this IEnumerable<T> list, DateTimeOffset at)
        where T : IDTOValidRangeQ {
        foreach (var item in list) {
            if (item.WithinValidRangeQ(at)) {
                yield return item;
            }
        }
    }
}
