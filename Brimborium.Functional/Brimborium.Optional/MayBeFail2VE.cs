using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Optional;

public class MayBeFail<V, E> : MayBe<V, E> {
    private readonly E _Value;

    public override bool Success => false;
    public override bool Fail => true;

    public MayBeFail(E value) {
        this._Value = value;
    }

    public override bool TryGetValue([MaybeNullWhen(false)] out V value) {
        value = default;
        return false;
    }

    public override bool TryGetSuccessfullValue([MaybeNullWhen(false)] out V value) {
        value = default;
        return false;
    }

    public override bool TryGetFailureValue([MaybeNullWhen(false)] out E value) {
        value = this._Value;
        return true;
    }


    public override MayBe<OV, OE> Map<OV, OE>(
        Func<MayBeValue<V, E>, MayBe<OV, OE>, MayBe<OV, OE>>? onSuccessfull = default,
        Func<MayBeValue<V, E>, MayBe<OV, OE>, MayBe<OV, OE>>? onUndecided = default,
        Func<MayBe<OV, OE>, MayBe<OV, OE>>? onNoValue = default,
        Func<MayBeFail<V, E>, MayBe<OV, OE>, MayBe<OV, OE>>? onFail = default
        ) {
        var r = MayBeNoValue<OV, OE>.Empty();
        if (onFail is null) {
            return r;
        } else { 
            return onFail(this, r);
        }
    }

    public override T MapTo<T>(
        Func<MayBeValue<V, E>, T>? onSuccessfull = default,
        Func<MayBeValue<V, E>, T>? onUndecided = default,
        Func<T>? onNoValue = default,
        Func<MayBeFail<V, E>, T>? onFail = default
        ) {
        if (onFail is null) {
            throw new ArgumentNullException("onFail");
        } else {
            return onFail(this);
        }
    }

    public static implicit operator MayBeFail<V, E>(MayBeFail<E> that) {
        return new MayBeFail<V, E>(that.Error);
    }
}
