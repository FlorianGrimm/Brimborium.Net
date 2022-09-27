namespace Brimborium.Optional;

//
public partial struct ErrorValue {
    private Exception _Error;
    public Exception Error => _Error;
    public ErrorValue(Exception error) {
        _Error = error;
    }

    [System.Diagnostics.CodeAnalysis.DoesNotReturn]
    public Exception Throw() {
        System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(this.Error).Throw();
        return this.Error;
    }
    
    public MayBe<V, F> ToMayBe<V,F>()
        where V : notnull
        where F : notnull
        => new MayBe<V, F>(MayBeMode.Error, default, default, this._Error);
}
