namespace Brimborium.Optional;

public abstract class MayBe<V> : MayBe {

    public abstract MayBeValue<V> WithMayValue(bool success, V value);
    public abstract MayBeValue<V> WithSuccessfullValue(V value);
    public abstract MayBeValue<V> WithUndecidedValue(V value);
    public abstract MayBe<V, E> WithFailure<E>(E failure);
    public abstract MayBe<V, E> AsFailure<E>();
}
