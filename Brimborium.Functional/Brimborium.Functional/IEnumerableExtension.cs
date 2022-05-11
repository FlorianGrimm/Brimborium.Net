namespace Brimborium.Functional;

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

    public static IEnumerable<R> WhereSelect<T, R>(this IEnumerable<T> src, Func<T, R?> predicateAndProject)
        where R : class {
        foreach (var item in src) {
            var r = predicateAndProject(item);
            if (r is not null) {
                yield return r;
            }
        }
    }

    public static IEnumerable<R> WhereSelectN<T, R>(this IEnumerable<T?> src, Func<T, R?> predicateAndProject)
        where T : class
        where R : class {
        foreach (var item in src) {
            if (item is null) {
                // skip
            } else {
                var r = predicateAndProject(item);
                if (r is not null) {
                    yield return r;
                }
            }
        }
    }

    public static IEnumerable<R> WhereSelectP<T, P, R>(this IEnumerable<T> src, P p, Func<T, P, R?> predicateAndProject)
        where R : class {
        foreach (var item in src) {
            var r = predicateAndProject(item, p);
            if (r is not null) {
                yield return r;
            }
        }
    }

    public static IEnumerable<R> WhereSelectNP<T, P, R>(this IEnumerable<T?> src, P p, Func<T, P, R?> predicateAndProject)
        where T : class
        where R : class {
        foreach (var item in src) {
            if (item is null) {
                // skip
            } else {
                var r = predicateAndProject(item, p);
                if (r is not null) {
                    yield return r;
                }
            }
        }
    }

    public static IEnumerable<T> SelectMore<T>(this IEnumerable<T?> src, Action<T, List<T>> predicateAndProject)
        where T : class {
        var list = new List<T>();
        foreach (var item in src) {
            if (item is null) {
                // skip
            } else {
                predicateAndProject(item, list);
                if (list.Count > 0) {
                    foreach (var resultItem in list) {
                        yield return resultItem;
                    }
                    list.Clear();
                }
            }
        }
    }

    public static IEnumerable<T> SelectMoreP<T, P>(this IEnumerable<T?> src, P p, Action<T, P, List<T>> predicateAndProject)
        where T : class {
        var list = new List<T>();
        foreach (var item in src) {
            if (item is null) {
                // skip
            } else {
                predicateAndProject(item, p, list);
                if (list.Count > 0) {
                    foreach (var resultItem in list) {
                        yield return resultItem;
                    }
                    list.Clear();
                }
            }
        }
    }
}
