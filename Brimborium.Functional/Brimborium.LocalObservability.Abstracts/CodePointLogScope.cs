namespace Brimborium.LocalObservability;
//  https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Logging/src/LoggerFactoryScopeProvider.cs
public class CodePointLogScope : IReadOnlyList<KeyValuePair<string, object?>> {
    private string? _cachedToString;
    private readonly CodePoint _CodePoint;

    public int Count => 1;

    public CodePointLogScope(CodePoint codePoint) {
        Debug.Assert(codePoint != null);
        this._CodePoint = codePoint;
    }

    public KeyValuePair<string, object?> this[int index] {
        get {
            if (index >= 1) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return new KeyValuePair<string, object?>("CodePoint", this._CodePoint);
        }
    }

    public override string ToString() {
        if (this._cachedToString == null) {

            this._cachedToString = $"CodePoint: {this._CodePoint}";
        }

        return this._cachedToString;
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() {
        yield return new KeyValuePair<string, object?>("CodePoint", this._CodePoint);
    }

    IEnumerator IEnumerable.GetEnumerator()
    => this.GetEnumerator();
}

public static class CodePointLogScopeExtension {
    public static CodePointLogScope AsScope(this CodePoint codePoint) 
        => new CodePointLogScope(codePoint);
}