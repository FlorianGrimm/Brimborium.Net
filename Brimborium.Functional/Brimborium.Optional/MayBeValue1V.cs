using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Optional;

public class MayBeValue<V> : MayBe<V> {
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

    public bool TryGetValue([MaybeNullWhen(false)] out V value) {
        value = this.Value;
        return true;
    }

    public bool TryGetSuccessfullValue([MaybeNullWhen(false)] out V value) {
        if (this.Success) {
            value = this.Value;
            return true;
        }
        value = default;
        return false;
    }

    public override MayBeValue<V> WithMayValue(bool success, V value) => new MayBeValue<V>(success, value);
    public override MayBeValue<V> WithSuccessfullValue(V value) => new MayBeValue<V>(true, value);
    public override MayBeValue<V> WithUndecidedValue(V value) => new MayBeValue<V>(false, value);
    public override MayBe<V, E> WithFailure<E>(E failure) => new MayBeFail<V, E>(failure);
    public override MayBe<V, E> AsFailure<E>() => MayBeNoValue<V, E>.Empty();
}
