namespace Brimborium.Optional;
public interface IMayBeFactory {
    MayBe<V, F> OkValue<V, F>(V value) where V : notnull where F : notnull;
    MayBe<V, F> BadValue<V, F>(V value) where V : notnull where F : notnull;
}
//
public class MayBeFactory : IMayBeFactory {
    public MayBe<V, F> OkValue<V, F>(V value)
        where V : notnull
        where F : notnull
        => new MayBe<V, F>(MayBeMode.Good, value, default, default);
    public MayBe<V, F> BadValue<V, F>(V value)
        where V : notnull
        where F : notnull
        => new MayBe<V, F>(MayBeMode.Bad, value, default, default);
    public MayBe<V, F> NoValue<V, F>()
        where V : notnull
        where F : notnull
        => new MayBe<V, F>(MayBeMode.NoValue, default, default, default);
    public MayBe<V, F> FailValue<V, F>(F failure)
        where V : notnull
        where F : notnull
        => new MayBe<V, F>(MayBeMode.Fail, default, failure, default);
    public MayBe<V, F> ErrorValue<V, F>(V value)
        where V : notnull
        where F : notnull
        => new MayBe<V, F>(MayBeMode.Error, value, default, default);
}
