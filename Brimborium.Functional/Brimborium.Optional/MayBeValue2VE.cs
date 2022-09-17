using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Optional;

public class MayBeValue<V, E> : MayBe<V, E> {
    public override bool Success { get; }

    public override bool Fail => false;
    public V Value { get; init; }

    public MayBeValue(bool Success, V Value) {
        this.Success = Success;
        this.Value = Value;
    }

    public void Deconstruct(out bool Success, out V Value) {
        Success = this.Success;
        Value = this.Value;
    }

    public override bool TryGetValue([MaybeNullWhen(false)] out V value) {
        value = this.Value;
        return true;
    }

    public override bool TryGetSuccessfullValue([MaybeNullWhen(false)] out V value) {
        if (this.Success) {
            value = this.Value;
            return true;
        }
        value = default;
        return false;
    }

    public override bool TryGetFailureValue([MaybeNullWhen(false)] out E value) {
        value = default;
        return false;
    }

    public MayBeValue<V, E> WithMayValue(bool success, V value) => new MayBeValue<V, E>(success, value);

    public new MayBeValue<V, E> WithSuccessfullValue(V value) => new MayBeValue<V, E>(true, value);

    public new MayBeValue<V, E> WithUndecidedValue(V value) => new MayBeValue<V, E>(false, value);

    public override MayBe<OV, OE> Map<OV, OE>(
        Func<MayBeValue<V, E>, MayBe<OV, OE>, MayBe<OV, OE>>? onSuccessfull = default,
        Func<MayBeValue<V, E>, MayBe<OV, OE>, MayBe<OV, OE>>? onUndecided = default,
        Func<MayBe<OV, OE>, MayBe<OV, OE>>? onNoValue = default,
        Func<MayBeFail<V, E>, MayBe<OV, OE>, MayBe<OV, OE>>? onFail = default
        ) {
        var r = MayBeNoValue<OV, OE>.Empty();
        if (this.Success) {
            if (onSuccessfull is null) {
                return r;
            } else {
                return onSuccessfull(this, r);
            }
        } else {
            if (onUndecided is null) {
                return r;
            } else {
                return onUndecided(this, r);
            }
        }
    }

    public override T MapTo<T>(
       Func<MayBeValue<V, E>, T>? onSuccessfull = default,
        Func<MayBeValue<V, E>, T>? onUndecided = default,
        Func<T>? onNoValue = default,
        Func<MayBeFail<V, E>, T>? onFail = default
        ) {
        if (this.Success) {
            if (onSuccessfull is null) {
                throw new ArgumentNullException("onSuccessfull");
            } else { 
            return onSuccessfull(this);
            }
        } else {
            if (onUndecided is null) {
                throw new ArgumentNullException("onUndecided");
            } else {
                return onUndecided(this);
            }
        }
    }

    public static implicit operator MayBeValue<V, E>(MayBeValue<V> that) {
        return new MayBeValue<V, E>(that.Success, that.Value);
    }
}
