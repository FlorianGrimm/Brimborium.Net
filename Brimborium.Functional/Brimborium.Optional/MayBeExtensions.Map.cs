using System.ComponentModel;

namespace Brimborium.Optional;

public static partial class MayBeExtensions {
    public static async Task<T> MapToAsync<V, F, T>(
        this MayBe<V, F> that,
        //Func<MayBe<V, F>, T> fallback,
        Func<NoValue, Task<T>>? onNoValue = default,
        Func<GoodValue<V>, Task<T>>? onGood = default,
        Func<BadValue<V>, Task<T>>? onBad = default,
        Func<FailValue<F>, Task<T>>? onFail = default,
        Func<ErrorValue, Task<T>>? onError = default,
        Func<MayBe<V, F>, Task<T>>? otherwise = default
    )
        where V : notnull
        where F : notnull {
        switch (that.Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return await onNoValue((NoValue)that);
                } else if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    throw new ArgumentNullException(nameof(onNoValue));
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return await onGood((GoodValue<V>)that);
                } else if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    throw new ArgumentNullException(nameof(onGood));
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return await onBad((BadValue<V>)that);
                } else if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    throw new ArgumentNullException(nameof(onBad));
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return await onFail((FailValue<F>)that);
                } else if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    throw new ArgumentNullException(nameof(onFail));
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return await onError((ErrorValue)that);
                } else if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    throw new ArgumentNullException(nameof(onError));
                }
            default:
                throw new InvalidOperationException($"{that.Mode} is unexpected.");
        }
    }
    public static async Task<T> MapToAsync<V, F, T>(
        this Task<MayBe<V, F>> taskOfThat,
        Func<NoValue, Task<T>>? onNoValue = default,
        Func<GoodValue<V>, Task<T>>? onGood = default,
        Func<BadValue<V>, Task<T>>? onBad = default,
        Func<FailValue<F>, Task<T>>? onFail = default,
        Func<ErrorValue, Task<T>>? onError = default,
        Func<MayBe<V, F>, Task<T>>? otherwise = default
    )
        where V : notnull
        where F : notnull {
        var that = await taskOfThat;
        switch (that.Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return await onNoValue((NoValue)that);
                } else if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    throw new ArgumentNullException(nameof(onNoValue));
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return await onGood((GoodValue<V>)that);
                } else if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    throw new ArgumentNullException(nameof(onGood));
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return await onBad((BadValue<V>)that);
                } else if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    throw new ArgumentNullException(nameof(onBad));
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return await onFail((FailValue<F>)that);
                } else if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    throw new ArgumentNullException(nameof(onFail));
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return await onError((ErrorValue)that);
                } else if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    throw new ArgumentNullException(nameof(onError));
                }
            default:
                throw new InvalidOperationException($"{that.Mode} is unexpected.");
        }
    }
    //
    public static async Task<MayBe<RV, RF>> MapAsync<V, F, RV, RF>(
        this MayBe<V, F> that,
        MayBe<RV, RF> value,
        Func<MayBe<V, F>, MayBe<RV, RF>, Task<MayBe<RV, RF>>>? onNoValue = default,
        Func<MayBe<V, F>, MayBe<RV, RF>, Task<MayBe<RV, RF>>>? onGood = default,
        Func<MayBe<V, F>, MayBe<RV, RF>, Task<MayBe<RV, RF>>>? onBad = default,
        Func<MayBe<V, F>, MayBe<RV, RF>, Task<MayBe<RV, RF>>>? onFail = default,
        Func<MayBe<V, F>, MayBe<RV, RF>, Task<MayBe<RV, RF>>>? onError = default,
        Func<MayBe<V, F>, MayBe<RV, RF>, Task<MayBe<RV, RF>>>? otherwise = default
    )
        where V : notnull
        where F : notnull
        where RV : notnull
        where RF : notnull {
        switch (that.Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return await onNoValue(that, value);
                }
                if (otherwise is not null) {
                    return await otherwise(that, value);
                } else {
                    return new ErrorValue(new InvalidOperationException("MayBeNoValue is null"));
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return await onGood(that, value);
                }
                if (otherwise is not null) {
                    return await otherwise(that, value);
                } else {
                    return new ErrorValue(new InvalidOperationException("Good is null"));
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return await onBad(that, value);
                }
                if (otherwise is not null) {
                    return await otherwise(that, value);
                } else {
                    return new ErrorValue(new InvalidOperationException("Bad is null"));
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return await onFail(that, value);
                }
                if (otherwise is not null) {
                    return await otherwise(that, value);
                } else {
                    return new ErrorValue(new InvalidOperationException("Fail is null"));
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return await onError(that, value);
                }
                if (otherwise is not null) {
                    return await otherwise(that, value);
                } else {
                    return new ErrorValue(new InvalidOperationException("Error is null"));
                }
            default:
                return new ErrorValue(new InvalidOperationException($"{that.Mode} is unexpected."));
        }
    }
    //
    public static async Task<MayBe<RV, RF>> MapAsync<V, F, RV, RF>(
        this Task<MayBe<V, F>> taskOfThat,
        MayBe<RV, RF> value,
        Func<MayBe<V, F>, MayBe<RV, RF>, Task<MayBe<RV, RF>>>? onNoValue = default,
        Func<MayBe<V, F>, MayBe<RV, RF>, Task<MayBe<RV, RF>>>? onGood = default,
        Func<MayBe<V, F>, MayBe<RV, RF>, Task<MayBe<RV, RF>>>? onBad = default,
        Func<MayBe<V, F>, MayBe<RV, RF>, Task<MayBe<RV, RF>>>? onFail = default,
        Func<MayBe<V, F>, MayBe<RV, RF>, Task<MayBe<RV, RF>>>? onError = default,
        Func<MayBe<V, F>, MayBe<RV, RF>, Task<MayBe<RV, RF>>>? otherwise = default
    )
        where V : notnull
        where F : notnull
        where RV : notnull
        where RF : notnull {
        MayBe<V, F> that = await taskOfThat;
        switch (that.Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return await onNoValue(that, value);
                }
                if (otherwise is not null) {
                    return await otherwise(that, value);
                } else {
                    return new ErrorValue(new InvalidOperationException("MayBeNoValue is null"));
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return await onGood(that, value);
                }
                if (otherwise is not null) {
                    return await otherwise(that, value);
                } else {
                    return new ErrorValue(new InvalidOperationException("Good is null"));
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return await onBad(that, value);
                }
                if (otherwise is not null) {
                    return await otherwise(that, value);
                } else {
                    return new ErrorValue(new InvalidOperationException("Bad is null"));
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return await onFail(that, value);
                }
                if (otherwise is not null) {
                    return await otherwise(that, value);
                } else {
                    return new ErrorValue(new InvalidOperationException("Fail is null"));
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return await onError(that, value);
                }
                if (otherwise is not null) {
                    return await otherwise(that, value);
                } else {
                    return new ErrorValue(new InvalidOperationException("Error is null"));
                }
            default:
                return new ErrorValue(new InvalidOperationException($"{that.Mode} is unexpected."));
        }
    }
    //
    public static async Task<MayBe<RV, RF>> MapValueAsync<V, F, RV, RF>(
        this MayBe<V, F> that,
        Func<NoValue, Task<MayBe<RV, RF>>>? onNoValue = default,
        Func<GoodValue<V>, Task<MayBe<RV, RF>>>? onGood = default,
        Func<BadValue<V>, Task<MayBe<RV, RF>>>? onBad = default,
        Func<FailValue<F>, Task<MayBe<RV, RF>>>? onFail = default,
        Func<ErrorValue, Task<MayBe<RV, RF>>>? onError = default,
        Func<MayBe<V, F>, Task<MayBe<RV, RF>>>? otherwise = default
    )
        where V : notnull
        where F : notnull
        where RV : notnull
        where RF : notnull {
        switch (that.Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return await onNoValue((NoValue)that);
                }
                if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    return new ErrorValue(new InvalidOperationException("MayBeNoValue is null"));
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return await onGood((GoodValue<V>)that);
                }
                if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    return new ErrorValue(new InvalidOperationException("Good is null"));
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return await onBad((BadValue<V>)that);
                }
                if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    return new ErrorValue(new InvalidOperationException("Bad is null"));
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return await onFail((FailValue<F>)that);
                }
                if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    return new ErrorValue(new InvalidOperationException("Fail is null"));
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return await onError((ErrorValue)that);
                }
                if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    return new ErrorValue(new InvalidOperationException("Error is null"));
                }
            default:
                return new ErrorValue(new InvalidOperationException($"{that.Mode} is unexpected."));
        }
    }
    //
    public static async Task<MayBe<RV, RF>> MapValueAsync<V, F, RV, RF>(
        this MayBe<V, F> that,
        MayBe<RV, RF> result,
        Func<NoValue, Task<MayBe<RV, RF>>>? onNoValue = default,
        Func<GoodValue<V>, Task<MayBe<RV, RF>>>? onGood = default,
        Func<BadValue<V>, Task<MayBe<RV, RF>>>? onBad = default,
        Func<FailValue<F>, Task<MayBe<RV, RF>>>? onFail = default,
        Func<ErrorValue, Task<MayBe<RV, RF>>>? onError = default,
        Func<MayBe<V, F>, Task<MayBe<RV, RF>>>? otherwise = default
    )
        where V : notnull
        where F : notnull
        where RV : notnull
        where RF : notnull {
        switch (that.Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return await onNoValue((NoValue)that);
                }
                if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    return result;
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return await onGood((GoodValue<V>)that);
                }
                if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    return result;
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return await onBad((BadValue<V>)that);
                }
                if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    return result;
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return await onFail((FailValue<F>)that);
                }
                if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    return result;
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return await onError((ErrorValue)that);
                }
                if (otherwise is not null) {
                    return await otherwise(that);
                } else {
                    return result;
                }
            default:
                return new ErrorValue(new InvalidOperationException($"{that.Mode} is unexpected."));
        }
    }
    //
}