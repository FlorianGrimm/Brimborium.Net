using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Optional;

public class MayBeNoValue<V, E> : MayBe<V, E> {
    private static MayBeNoValue<V, E>? _Empty;
    public static MayBeNoValue<V, E> Empty() {
        return (_Empty??=new MayBeNoValue<V, E>());
    }

    public override bool Success => false;

    public override bool Fail => false;

    public MayBeNoValue() {
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
        value = default;
        return false;
    }

    public override MayBe<OV, OE> Map<OV, OE>(
        Func<MayBeValue<V, E>, MayBe<OV, OE>, MayBe<OV, OE>>? onSuccessfull = default,
        Func<MayBeValue<V, E>, MayBe<OV, OE>, MayBe<OV, OE>>? onUndecided = default,
        Func<MayBe<OV, OE>, MayBe<OV, OE>>? onNoValue = default,
        Func<MayBeFail<V, E>, MayBe<OV, OE>, MayBe<OV, OE>>? onFail = default
        ) {
        var r = MayBeNoValue<OV, OE>.Empty();
        if (onNoValue is null) {
            return r;
        } else {
            return onNoValue(MayBeNoValue<OV, OE>.Empty());
        }
    }

    public override T MapTo<T>(
        Func<MayBeValue<V, E>, T>? onSuccessfull = default,
        Func<MayBeValue<V, E>, T>? onUndecided = default,
        Func<T>? onNoValue = default,
        Func<MayBeFail<V, E>, T>? onFail = default
        ) {
        if (onNoValue is null) {
            throw new ArgumentNullException("onNoValue");
        } else {
            return onNoValue();
        }
    }

    public static implicit operator MayBeNoValue<V, E>(MayBeNoValue<V> that) => Empty();
}
