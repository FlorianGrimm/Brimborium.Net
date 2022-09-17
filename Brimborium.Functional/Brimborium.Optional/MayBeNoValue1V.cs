namespace Brimborium.Optional;

public class MayBeNoValue<V> : MayBe<V> {
    private static MayBeNoValue<V>? _Empty;

    public static MayBeNoValue<V> Empty() {
        return (_Empty ??= new MayBeNoValue<V>());
    }

    public override bool Success => false;
    public override bool Fail => false;

    public MayBeNoValue() { }

    public static implicit operator MayBeNoValue<V>(MayBeNoValue that) => Empty();

    public override MayBeValue<V> WithMayValue(bool success, V value) => new MayBeValue<V>(success, value);
    public override MayBeValue<V> WithSuccessfullValue(V value) => new MayBeValue<V>(true, value);
    public override MayBeValue<V> WithUndecidedValue(V value) => new MayBeValue<V>(false, value);
    public override MayBe<V, E> WithFailure<E>(E failure) => new MayBeFail<V, E>(failure);
    public override MayBe<V, E> AsFailure<E>() => MayBeNoValue<V, E>.Empty();
}
