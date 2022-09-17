namespace Brimborium.Optional;

public class MayBeNoValue : MayBe {
    private static MayBeNoValue? _Empty;

    public static MayBeNoValue Empty() {
        return (_Empty ??= new MayBeNoValue());
    }

    public override bool Success => false;

    public override bool Fail => false;

    public MayBeNoValue() {
    }
}
