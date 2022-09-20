namespace Brimborium.Optional;
//
public static class MayBe {
    public static MayBe<V, F> MayBeGood<V, F>(V value)
        where V : notnull
        where F : notnull
        => new MayBe<V, F>(MayBeMode.Good, value, default, default);
    public static MayBe<V, F> MayBeBad<V, F>(V value)
        where V : notnull
        where F : notnull
        => new MayBe<V, F>(MayBeMode.Bad, value, default, default);
    public static MayBe<V, F> MayBeNoValue<V, F>()
        where V : notnull
        where F : notnull
        => new MayBe<V, F>(MayBeMode.NoValue, default, default, default);
    public static MayBe<V, F> MayBeFail<V, F>(F failure)
        where V : notnull
        where F : notnull
        => new MayBe<V, F>(MayBeMode.Fail, default, failure, default);
    public static MayBe<V, F> MayBeError<V, F>(Exception error)
        where V : notnull
        where F : notnull
        => new MayBe<V, F>(MayBeMode.Error, default, default, error);

    public static NoValue NoValue()
        => new NoValue();
    public static GoodValue<V> GoodValue<V>(V value)
        where V : notnull
        => new GoodValue<V>(value);
    public static BadValue<V> BadValue<V>(V value)
        where V : notnull
        => new BadValue<V>(value);
    public static FailValue<F> FailValue<F>(F failure)
        where F : notnull
        => new FailValue<F>(failure);
    public static ErrorValue ErrorValue(Exception error)
        => new ErrorValue(error);

}
