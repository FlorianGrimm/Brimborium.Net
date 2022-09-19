namespace Brimborium.Optional;

//
public struct ErrorValue {
    private Exception _Error;
    public Exception Error => _Error;
    public ErrorValue(Exception error) {
        _Error = error;
    }
    public MayBe<V, F> ToMayBe<V,F>()
        where V : notnull
        where F : notnull
        => new MayBe<V, F>(MayBeMode.Error, default, default, this._Error);
}
