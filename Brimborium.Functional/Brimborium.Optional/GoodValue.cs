namespace Brimborium.Optional;

//
public struct GoodValue<V>
        where V : notnull {
    private V _Value;
    public V Value=> _Value;

    public GoodValue(V value) {
        _Value = value;
    }

    public MayBe<V, F> ToMayBeAddFailureType<F>()
        where F : notnull
        => new MayBe<V, F>(MayBeMode.Good, this._Value, default, default);
}
