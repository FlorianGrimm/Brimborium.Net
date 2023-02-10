namespace Brimborium.BaseMethods;

public class DefaultEqualityComparer<T> : IEqualityComparer<T> {
    private static DefaultEqualityComparer<T>? _Default;

    public static DefaultEqualityComparer<T> Default
        => _Default ??= new DefaultEqualityComparer<T>();

    private static IEqualityComparer<T>? _UnderlyingEqualityComparer;

    private DefaultEqualityComparer() {
        if (_UnderlyingEqualityComparer is not null) {
            // OK
        } else if (typeof(T).IsSealed) {
            _UnderlyingEqualityComparer = EqualityComparer<T>.Default;
        } else {
            _UnderlyingEqualityComparer = new ObjectEqualityComparer();
        }
    }

    public bool Equals(T? x, T? y) => _UnderlyingEqualityComparer!.Equals(x, y);

    public int GetHashCode([DisallowNull] T obj) => _UnderlyingEqualityComparer!.GetHashCode(obj);

    class ObjectEqualityComparer : IEqualityComparer<T> {
        public bool Equals(T? x, T? y) => object.Equals(x, y);

        public int GetHashCode([DisallowNull] T obj) => obj?.GetHashCode() ?? 0;
    }
}