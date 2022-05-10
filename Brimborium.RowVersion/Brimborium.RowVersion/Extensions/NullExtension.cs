namespace Brimborium.RowVersion.Extensions;

public static class NullExtension {
    public static bool TryGetNotNull<T>(this T? value, [MaybeNullWhen(false)] out T result)
        where T : class {
        if (value is T) {
            result = value;
            return true;
        }
        {
            result = null;
            return false;
        }
    }

    public static bool TryGetNotNull<T>(this T? value, Func<T, bool> condition, [MaybeNullWhen(false)] out T result)
        where T : class {
        if (value is T) {
            if (condition(value)) {
                result = value;
                return true;
            }
        }
        {
            result = null;
            return false;
        }
    }


    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static string GetValueOrDefault(this string? value, string defaultValue) {
        if (string.IsNullOrEmpty(value)) {
            return defaultValue;
        } else {
            return value;
        }
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static string GetValueOrDefault(this string? value, Func<string> defaultValue) {
        if (string.IsNullOrEmpty(value)) {
            return defaultValue();
        } else {
            return value;
        }
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static T GetValueNotNullOrDefault<T>(this T? value, T defaultValue)
        where T : class {
        if (value is null) {
            return defaultValue;
        } else {
            return value;
        }
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static T GetValueNotNullOrDefault<T>(this T? value, Func<T> fnDefaultValue)
        where T : class {
        if (value is null) {
            return fnDefaultValue();
        } else {
            return value;
        }
    }
}
