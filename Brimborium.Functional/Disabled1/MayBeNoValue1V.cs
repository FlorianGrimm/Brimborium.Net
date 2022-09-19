namespace Brimborium.Optional;

public class MayBeNoValue<V> : MayBe<V> {
    private static MayBeNoValue<V>? _Empty;

    public static MayBeNoValue<V> Empty() {
        return (_Empty ??= new MayBeNoValue<V>());
    }

    public override bool Success => false;
    public override bool HasValue => false;
    public override bool Fail => false;

    public MayBeNoValue() { }

    public static implicit operator MayBeNoValue<V>(MayBeNoValue that) => Empty();
}
