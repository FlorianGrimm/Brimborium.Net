namespace Brimborium.Optional;

public abstract class MayBe {
    public abstract bool Success { get; }
    public abstract bool Fail { get; }
    protected MayBe() { }

    public static MayBeValue<V> MayValue<V>(bool success, V value) => new MayBeValue<V>(success, value);
    public static MayBeValue<V> SuccessfullValue<V>(V value) => new MayBeValue<V>(true, value);
    public static MayBeValue<V> UndecidedValue<V>(V value) => new MayBeValue<V>(false, value);
    public static MayBeNoValue<V> NoValue<V>(V? value = default) => new MayBeNoValue<V>();
    public static MayBeFail<E> FailError<E>(E value) => new MayBeFail<E>(value);
}
