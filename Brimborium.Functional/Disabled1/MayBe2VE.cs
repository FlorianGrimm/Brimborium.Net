using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Optional;

public abstract class MayBe<V, E> : MayBe {
    protected MayBe() { }

    public abstract bool TryGetValue([MaybeNullWhen(false)] out V value);
    public abstract bool TryGetSuccessfullValue([MaybeNullWhen(false)] out V value);
    public abstract bool TryGetFailureValue([MaybeNullWhen(false)] out E value);

    public MayBe<V, E> WithSuccessfullValue(V value) => new MayBeValue<V, E>(true, value);

    public MayBe<V, E> WithUndecidedValue(V value) => new MayBeValue<V, E>(false, value);

    public MayBe<V, E> WithFailure(E value) => new MayBeFail<V, E>(value);

    public abstract MayBe<OV, OE> Map<OV, OE>(
        Func<MayBeValue<V, E>, MayBe<OV, OE>, MayBe<OV, OE>>? onSuccessfull = default,
        Func<MayBeValue<V, E>, MayBe<OV, OE>, MayBe<OV, OE>>? onUndecided = default,
        Func<MayBe<OV, OE>, MayBe<OV, OE>>? onNoValue = default,
        Func<MayBeFail<V, E>, MayBe<OV, OE>, MayBe<OV, OE>>? onFail = default
        );

    public abstract T MapTo<T>(
        Func<MayBeValue<V, E>, T>? onSuccessfull = default,
        Func<MayBeValue<V, E>, T>? onUndecided = default,
        Func<T>? onNoValue = default,
        Func<MayBeFail<V, E>, T>? onFail = default
        );

    public MayBe<V, E> GetSuccessOrFailOrDefault(MayBe<V, E> maybeDefault)
        => (this.Success || this.Fail) ? this : maybeDefault;

    public static implicit operator MayBe<V, E>(MayBeValue<V> mayBeYes) {
        return new MayBeValue<V, E>(mayBeYes.Success, mayBeYes.Value);
    }

    public static implicit operator MayBe<V, E>(MayBeFail<E> mayBeNo) {
        return new MayBeFail<V, E>(mayBeNo.Error);
    }
}
