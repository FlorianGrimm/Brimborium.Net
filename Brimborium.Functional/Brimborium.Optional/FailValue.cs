namespace Brimborium.Optional;

public partial struct FailValue<F>
        where F : notnull {
    private F _Failure;
    public F Failure => _Failure;
    public FailValue(F failure) {
        _Failure = failure;
    }
    public MayBe<V, F> ToMayBeAddValueType<V>()
        where V : notnull
        => new MayBe<V, F>(MayBeMode.Fail, default, this._Failure,  default);
}
