namespace Brimborium.LocalObservability;
//  https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Logging/src/LoggerFactoryScopeProvider.cs
#warning TODO Wrong
public class CodePointLogState : IReadOnlyList<KeyValuePair<string, object?>> {
    private string? _cachedToString;
    private readonly CodePoint _CodePoint;
    private readonly string? _Name;
    private readonly object? _Value;

    public int Count => 2;

    public CodePointLogState(CodePoint codePoint, object? value, string? name = default) {
        Debug.Assert(codePoint != null);
        this._CodePoint = codePoint;
        this._Value = value;
        this._Name = name;
    }

    public KeyValuePair<string, object?> this[int index] {
        get {
            if (index == 0) {
                return new KeyValuePair<string, object?>("CodePoint", this._CodePoint);
            }
            if (index == 1) {
                return new KeyValuePair<string, object?>(this._Name ?? this._CodePoint.Name, this._Value);
            }
            throw new ArgumentOutOfRangeException(nameof(index));
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
        yield return new KeyValuePair<string, object?>(this._Name ?? this._CodePoint.Name, this._Value);
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
public static class CodePointLogStateExtension {
    public static void LogCodepoint(
        this ILogger logger,
        LogLevel logLevel,
        CodePoint codePoint,
        object? value, string? name = default
        ) {
        var state = new CodePointLogState(codePoint, value, name);
        logger.Log<CodePointLogState>(logLevel, codePoint.EventId, state, null, EmptyFormater);
    }

    private static string EmptyFormater(CodePointLogState state, Exception? exception) {
        if (exception is not null) {
            return exception.ToString();
        } else {

            return null!;
        }
    }

    public static void LogCodepoint(
        this ILogger logger,
        LogLevel logLevel,
        CodePoint codePoint,
        string message,
        object? value, string? name = default
        ) {
        var state = new CodePointLogState(codePoint, value, name);
        logger.Log<CodePointLogState>(logLevel, codePoint.EventId, state, null, (state, exception) => message);
    }
}
