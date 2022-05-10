namespace Brimborium.RowVersion.Extensions;

public static class IEnumerableExtension {
    public static IEnumerable<T> WhereP<T, P>(this IEnumerable<T> src, P p, Func<T, P, bool> predicate) {
        foreach (var item in src) {
            if (predicate(item, p)) {
                yield return item;
            }
        }
    }

    public static IEnumerable<T> WhereNP<T, P>(this IEnumerable<T?> src, P p, Func<T, P, bool> predicate)
        where T : class {
        foreach (var item in src) {
            if (item is null) {
                // skip
            } else if (predicate(item, p)) {
                yield return item;
            }
        }
    }

    public static IEnumerable<T> SelectMore<T, P>(this IEnumerable<T?> src, P p, Action<T, P, List<T>> predicateAndProject)
        where T : class {
        var list = new List<T>();
        foreach (var item in src) {
            if (item is null) {
                // skip
            } else {
                predicateAndProject(item, p, list);
                if (list.Count > 0) {
                    foreach (var resultItem in list) {
                        yield return item;
                    }
                    list.Clear();
                }
            }
        }
    }
}
