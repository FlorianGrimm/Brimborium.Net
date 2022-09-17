namespace Brimborium.Optional;

public abstract class MayBe<V> : MayBe {

    //public abstract MayBeValue<V> WithMayValue(bool success, V value);
    //public abstract MayBeValue<V> WithSuccessfullValue(V value);
    //public abstract MayBeValue<V> WithUndecidedValue(V value);
    //public abstract MayBe<V, E> WithFailure<E>(E failure);
    //public abstract MayBe<V, E> AsFailure<E>();

    public virtual MayBeValue<V> WithMayValue(bool success, V value) => new MayBeValue<V>(success, value);
    public virtual MayBeValue<V> WithSuccessfullValue(V value) => new MayBeValue<V>(true, value);
    public virtual MayBeValue<V> WithUndecidedValue(V value) => new MayBeValue<V>(false, value);
    public virtual MayBe<V, E> WithFailure<E>(E failure) => new MayBeFail<V, E>(failure);
    public virtual MayBe<V, E> AddFailureType<E>() => MayBeNoValue<V, E>.Empty();
}
