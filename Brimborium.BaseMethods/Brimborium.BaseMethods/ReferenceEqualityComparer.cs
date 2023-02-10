namespace Brimborium.BaseMethods;

public class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class {
    public static ReferenceEqualityComparer<T> Default { get; } = new ReferenceEqualityComparer<T>();

    public bool Equals(T? x, T? y) {
        return ReferenceEquals(x, y);
    }

    public int GetHashCode(T? obj) {
        return RuntimeHelpers.GetHashCode(obj);
    }
}