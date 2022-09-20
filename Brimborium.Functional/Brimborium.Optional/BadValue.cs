namespace Brimborium.Optional;

//
public partial struct BadValue<V>
        where V : notnull {
    private V _Value;
    public V Value => _Value;
    public BadValue(V value) {
        _Value = value;
    }
    public MayBe<V, F> ToMayBeAddFailureType<F>()
        where F : notnull
        => new MayBe<V, F>(MayBeMode.Bad, this._Value, default, default);
}
