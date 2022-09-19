namespace Brimborium.Optional;

//
public struct NoValue {
    public NoValue() {
    }
}

//public struct NoValue<V>
//        where V : notnull {
//    private V _Value;
//    public GoodValue(V value) {
//        _Value = value;
//    }
//    public MayBe2<V, F> ToMayBe2<F>()
//        where F : notnull
//        => new MayBe2<V, F>(MayBeMode.NoValue, this._Value, default, default);
//}