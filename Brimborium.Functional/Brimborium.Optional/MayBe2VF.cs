using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Optional;

public partial struct MayBe<V, F>
    where V : notnull
    where F : notnull {

    private readonly MayBeMode _Mode;
    private readonly V? _Value;
    private readonly F? _Failure;
    private readonly Exception? _Error;

    public MayBe(
        MayBeMode mode,
        V? value,
        F? failure,
        Exception? error
        ) {
        if (mode == MayBeMode.NoValue) {
            _Mode = MayBeMode.NoValue;
            _Value = default;
            _Failure = default;
            _Error = default;
        } else if (mode == MayBeMode.Good || mode == MayBeMode.Bad) {
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }
            _Mode = mode;
            _Value = value;
            _Failure = default;
            _Error = default;
        } else if (mode == MayBeMode.Fail) {
            if (failure == null) {
                throw new ArgumentNullException(nameof(failure));
            }
            _Mode = mode;
            _Value = default;
            _Failure = failure;
            _Error = default;
        } else if (mode == MayBeMode.Error) {
            if (error == null) {
                throw new ArgumentNullException(nameof(error));
            }
            _Mode = mode;
            _Value = default;
            _Failure = default;
            _Error = error;
        } else {
            throw new ArgumentException("unknown mode", nameof(mode));
        }
    }

    public bool TryGetGoodValue([MaybeNullWhen(false)] out V value) {
        if (this._Mode == MayBeMode.Good) {
            value = this._Value!;
            return true;
        }
        value = default;
        return false;
    }
    public bool TryGetAsGoodValue([MaybeNullWhen(false)] out GoodValue<V> value) {
        if (this._Mode == MayBeMode.Good) {
            value = new GoodValue<V>(this._Value!);
            return true;
        }
        value = default;
        return false;
    }

    public bool TryGetBadValue([MaybeNullWhen(false)] out V value) {
        if (this._Mode == MayBeMode.Bad) {
            value = this._Value!;
            return true;
        }
        value = default;
        return false;
    }
    public bool TryGetAsBadValue([MaybeNullWhen(false)] out BadValue<V> value) {
        if (this._Mode == MayBeMode.Bad) {
            value = new BadValue<V>(this._Value!);
            return true;
        }
        value = default;
        return false;
    }

    public bool TryGetFailValue([MaybeNullWhen(false)] out F failure) {
        if (this._Mode == MayBeMode.Fail) {
            failure = this._Failure!;
            return true;
        }
        failure = default;
        return false;
    }

    public bool TryGetAsFailValue([MaybeNullWhen(false)] out FailValue<F> failure) {
        if (this._Mode == MayBeMode.Fail) {
            failure = new FailValue<F>(this._Failure!);
            return true;
        }
        failure = default;
        return false;
    }

    public bool TryGetErrorValue([MaybeNullWhen(false)] out Exception error) {
        if (this._Mode == MayBeMode.Error) {
            error = this._Error!;
            return true;
        }
        error = default;
        return false;
    }


    public bool TryGetAsErrorValue([MaybeNullWhen(false)] out ErrorValue error) {
        if (this._Mode == MayBeMode.Error) {
            error = new ErrorValue(this._Error!);
            return true;
        }
        error = default;
        return false;
    }

    public bool IsNoValue => this._Mode == MayBeMode.NoValue;
    public bool IsGood => this._Mode == MayBeMode.Good;
    public bool IsBad => this._Mode == MayBeMode.Bad;
    public bool IsBadOrNoValue => (this._Mode == MayBeMode.Bad) || (this._Mode == MayBeMode.NoValue);
    public bool IsFail => this._Mode == MayBeMode.Fail;
    public bool IsError => this._Mode == MayBeMode.Error;
    public bool IsFailOrError => (this._Mode == MayBeMode.Fail) || (this._Mode == MayBeMode.Error);

    public MayBeMode Mode => this._Mode;
    public V GoodValue => (this._Mode == MayBeMode.Good) ? this._Value : throw new InvalidOperationException("Mode must be Good");
    public V? Value => ((this._Mode == MayBeMode.Good) || (this._Mode == MayBeMode.Bad)) ? this._Value : default;
    public F? Failure => (this._Mode == MayBeMode.Fail) ? this._Failure : default;
    public Exception? Error => (this._Mode == MayBeMode.Error) ? this._Error : default;

    public void Deconstruct(
            out MayBeMode mode,
            out V? value,
            out F? failure,
            out Exception? error
        ) {
        mode = this._Mode;
        value = ((this._Mode == MayBeMode.Good) || (this._Mode == MayBeMode.Bad)) ? this._Value : default;
        failure = (this._Mode == MayBeMode.Fail) ? this._Failure : default;
        error = (this._Mode == MayBeMode.Error) ? this._Error : default;
    }
    //
    public MayBe<V, F> WithNoValue()
        => new MayBe<V, F>(MayBeMode.NoValue, default, default, default);
    public MayBe<V, F> WithGoodValue(V value)
        => new MayBe<V, F>(MayBeMode.Good, value, default, default);
    public MayBe<V, F> WithBadValue(V value)
        => new MayBe<V, F>(MayBeMode.Bad, value, default, default);
    public MayBe<V, F> WithFailVallue(F failure)
        => new MayBe<V, F>(MayBeMode.Fail, default, failure, default);
    public MayBe<V, F> WithError(Exception error)
        => new MayBe<V, F>(MayBeMode.Error, default, default, error);
    //
    public MayBe<RV, RF> If<RV, RF>(
            Func<MayBe<V, F>, bool> condition,
            Func<MayBe<V, F>, MayBe<RV, RF>>? trueCase,
            Func<MayBe<V, F>, MayBe<RV, RF>>? falseCase
    ) where RV : notnull
        where RF : notnull {
        if (this.TryGetAsGoodValue(out var goodValue)) {
            var conditionValue = condition(goodValue);
            if (conditionValue) {
                if (trueCase is not null) {
                    return trueCase(goodValue);
                }
            } else {
                if (falseCase is not null) {
                    return falseCase(goodValue);
                }
            }
        }
        return MayBe.MayBeNoValue<RV, RF>();
    }
    //
    public MayBe<RV, RF> IfGoodValue<RV, RF>(
            Func<GoodValue<V>, bool> condition,
            Func<GoodValue<V>, MayBe<RV, RF>>? trueCase,
            Func<GoodValue<V>, MayBe<RV, RF>>? falseCase,
            Func<MayBe<V, F>, MayBe<RV, RF>>? otherwise
    ) where RV : notnull
        where RF : notnull {
        if (this.TryGetAsGoodValue(out var goodValue)) {
            var conditionValue = condition(goodValue);
            if (conditionValue) {
                if (trueCase is not null) {
                    return trueCase(goodValue);
                }
            } else {
                if (falseCase is not null) {
                    return falseCase(goodValue);
                }
            }
        }
        if (otherwise is not null) {
            return otherwise(this);
        }
        return MayBe.MayBeNoValue<RV, RF>();
    }

    public MayBe<RV, RF> Map<RV, RF>(
        Func<MayBe<V, F>, MayBe<RV, RF>>? onNoValue = default,
        Func<MayBe<V, F>, MayBe<RV, RF>>? onGood = default,
        Func<MayBe<V, F>, MayBe<RV, RF>>? onBad = default,
        Func<MayBe<V, F>, MayBe<RV, RF>>? onFail = default,
        Func<MayBe<V, F>, MayBe<RV, RF>>? onError = default,
        Func<MayBe<V, F>, MayBe<RV, RF>>? otherwise = default
    ) where RV : notnull
        where RF : notnull {
        switch (this._Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return onNoValue(this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return new ErrorValue(new InvalidOperationException("MayBeNoValue is null"));
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return onGood(this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return new ErrorValue(new InvalidOperationException("Good is null"));
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return onBad(this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return new ErrorValue(new InvalidOperationException("Bad is null"));
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return onFail(this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return new ErrorValue(new InvalidOperationException("Fail is null"));
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return onError(this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return new ErrorValue(new InvalidOperationException("Error is null"));
                }
            default:
                return new ErrorValue(new InvalidOperationException($"{this._Mode} is unexpected."));
        }
    }
    public MayBe<RV, RF> MapValue<RV, RF>(
        Func<NoValue, MayBe<RV, RF>>? onNoValue = default,
        Func<GoodValue<V>, MayBe<RV, RF>>? onGood = default,
        Func<BadValue<V>, MayBe<RV, RF>>? onBad = default,
        Func<FailValue<F>, MayBe<RV, RF>>? onFail = default,
        Func<ErrorValue, MayBe<RV, RF>>? onError = default,
        Func<MayBe<V, F>, MayBe<RV, RF>>? otherwise = default
    ) where RV : notnull
        where RF : notnull {
        switch (this._Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return onNoValue((NoValue)this);
                }
                if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return new ErrorValue(new InvalidOperationException("MayBeNoValue is null"));
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return onGood((GoodValue<V>)this);
                }
                if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return new ErrorValue(new InvalidOperationException("Good is null"));
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return onBad((BadValue<V>)this);
                }
                if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return new ErrorValue(new InvalidOperationException("Bad is null"));
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return onFail((FailValue<F>)this);
                }
                if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return new ErrorValue(new InvalidOperationException("Fail is null"));
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return onError((ErrorValue)this);
                }
                if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return new ErrorValue(new InvalidOperationException("Error is null"));
                }
            default:
                return new ErrorValue(new InvalidOperationException($"{this._Mode} is unexpected."));
        }
        //this.Mode== MayBeMode.NoValue
        //return MayBe.MayBeNoValue<RV, RF>();
    }
    //
    public MayBe<RV, RF> MapValueWithDefault<RV, RF>(
        MayBe<RV, RF> result,
        Func<NoValue, MayBe<RV, RF>, MayBe<RV, RF>>? onNoValue = default,
        Func<GoodValue<V>, MayBe<RV, RF>, MayBe<RV, RF>>? onGood = default,
        Func<BadValue<V>, MayBe<RV, RF>, MayBe<RV, RF>>? onBad = default,
        Func<FailValue<F>, MayBe<RV, RF>, MayBe<RV, RF>>? onFail = default,
        Func<ErrorValue, MayBe<RV, RF>, MayBe<RV, RF>>? onError = default,
        Func<MayBe<V, F>, MayBe<RV, RF>, MayBe<RV, RF>>? otherwise = default
    )
        where RV : notnull
        where RF : notnull {

        switch (this._Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return onNoValue((NoValue)this, result);
                }
                if (otherwise is not null) {
                    return otherwise(this, result);
                } else {
                    return new ErrorValue(new InvalidOperationException("MayBeNoValue is null"));
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return onGood((GoodValue<V>)this, result);
                }
                if (otherwise is not null) {
                    return otherwise(this, result);
                } else {
                    return new ErrorValue(new InvalidOperationException("Good is null"));
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return onBad((BadValue<V>)this, result);
                }
                if (otherwise is not null) {
                    return otherwise(this, result);
                } else {
                    return new ErrorValue(new InvalidOperationException("Bad is null"));
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return onFail((FailValue<F>)this, result);
                }
                if (otherwise is not null) {
                    return otherwise(this, result);
                } else {
                    return new ErrorValue(new InvalidOperationException("Fail is null"));
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return onError((ErrorValue)this, result);
                }
                if (otherwise is not null) {
                    return otherwise(this, result);
                } else {
                    return new ErrorValue(new InvalidOperationException("Error is null"));
                }
            default:
                return new ErrorValue(new InvalidOperationException($"{this._Mode} is unexpected."));
        }
    }
    //
    public MayBe<RV, RF> MapOnValueWithDefault<RV, RF, Args>(
        Args args,
        MayBe<RV, RF> result,
        Func<NoValue, Args, MayBe<RV, RF>, MayBe<RV, RF>>? onNoValue = default,
        Func<GoodValue<V>, Args, MayBe<RV, RF>, MayBe<RV, RF>>? onGood = default,
        Func<BadValue<V>, Args, MayBe<RV, RF>, MayBe<RV, RF>>? onBad = default,
        Func<FailValue<F>, Args, MayBe<RV, RF>, MayBe<RV, RF>>? onFail = default,
        Func<ErrorValue, Args, MayBe<RV, RF>, MayBe<RV, RF>>? onError = default,
        Func<MayBe<V, F>, Args, MayBe<RV, RF>, MayBe<RV, RF>>? otherwise = default
    )
        where RV : notnull
        where RF : notnull {

        switch (this._Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return onNoValue((NoValue)this, args, result);
                }
                if (otherwise is not null) {
                    return otherwise(this, args, result);
                } else {
                    return new ErrorValue(new InvalidOperationException("MayBeNoValue is null"));
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return onGood((GoodValue<V>)this, args, result);
                }
                if (otherwise is not null) {
                    return otherwise(this, args, result);
                } else {
                    return new ErrorValue(new InvalidOperationException("Good is null"));
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return onBad((BadValue<V>)this, args, result);
                }
                if (otherwise is not null) {
                    return otherwise(this, args, result);
                } else {
                    return new ErrorValue(new InvalidOperationException("Bad is null"));
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return onFail((FailValue<F>)this, args, result);
                }
                if (otherwise is not null) {
                    return otherwise(this, args, result);
                } else {
                    return new ErrorValue(new InvalidOperationException("Fail is null"));
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return onError((ErrorValue)this, args, result);
                }
                if (otherwise is not null) {
                    return otherwise(this, args, result);
                } else {
                    return new ErrorValue(new InvalidOperationException("Error is null"));
                }
            default:
                return new ErrorValue(new InvalidOperationException($"{this._Mode} is unexpected."));
        }
    }
    //
    public T MapTo<T>(
        Func<MayBe<V, F>, T>? onNoValue = default,
        Func<MayBe<V, F>, T>? onGood = default,
        Func<MayBe<V, F>, T>? onBad = default,
        Func<MayBe<V, F>, T>? onFail = default,
        Func<MayBe<V, F>, T>? onError = default,
        Func<MayBe<V, F>, T>? otherwise = default
    ) {

        switch (this._Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return onNoValue(this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    throw new ArgumentNullException(nameof(onNoValue));
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return onGood(this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    throw new ArgumentNullException(nameof(onGood));
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return onBad(this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    throw new ArgumentNullException(nameof(onBad));
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return onFail(this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    throw new ArgumentNullException(nameof(onFail));
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return onError(this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    throw new ArgumentNullException(nameof(onError));
                }
            default:
                throw new InvalidOperationException($"{this._Mode} is unexpected.");
        }
    }
    //
    public T MapToValue<T>(
        Func<NoValue, T>? onNoValue = default,
        Func<GoodValue<V>, T>? onGood = default,
        Func<BadValue<V>, T>? onBad = default,
        Func<FailValue<F>, T>? onFail = default,
        Func<ErrorValue, T>? onError = default,
        Func<MayBe<V, F>, T>? otherwise = default
    ) {

        switch (this._Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return onNoValue((NoValue)this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    throw new ArgumentNullException(nameof(onNoValue));
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return onGood((GoodValue<V>)this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    throw new ArgumentNullException(nameof(onGood));
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return onBad((BadValue<V>)this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    throw new ArgumentNullException(nameof(onBad));
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return onFail((FailValue<F>)this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    throw new ArgumentNullException(nameof(onFail));
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return onError((ErrorValue)this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    throw new ArgumentNullException(nameof(onError));
                }
            default:
                throw new InvalidOperationException($"{this._Mode} is unexpected.");
        }
    }
    //
    public T MapToValueWithFallback<T>(
        Func<MayBe<V, F>, T> fallback,
        Func<NoValue, T>? onNoValue = default,
        Func<GoodValue<V>, T>? onGood = default,
        Func<BadValue<V>, T>? onBad = default,
        Func<FailValue<F>, T>? onFail = default,
        Func<ErrorValue, T>? onError = default,
        Func<MayBe<V, F>, T>? otherwise = default
    ) {
        switch (this._Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return onNoValue((NoValue)this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return fallback(this);
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return onGood((GoodValue<V>)this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return fallback(this);
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return onBad((BadValue<V>)this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return fallback(this);
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return onFail((FailValue<F>)this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return fallback(this);
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return onError((ErrorValue)this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return fallback(this);
                }
            default:
                throw new InvalidOperationException($"{this._Mode} is unexpected.");
        }
    }
    //
    public T MapToWithDefault<T>(
        T result,
        Func<NoValue, T>? onNoValue = default,
        Func<GoodValue<V>, T>? onGood = default,
        Func<BadValue<V>, T>? onBad = default,
        Func<FailValue<F>, T>? onFail = default,
        Func<ErrorValue, T>? onError = default,
        Func<MayBe<V, F>, T>? otherwise = default
    ) {

        switch (this._Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return onNoValue((NoValue)this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return result;
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return onGood((GoodValue<V>)this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return result;
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return onBad((BadValue<V>)this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return result;
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return onFail((FailValue<F>)this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return result;
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return onError((ErrorValue)this);
                } else if (otherwise is not null) {
                    return otherwise(this);
                } else {
                    return result;
                }
            default:
                throw new InvalidOperationException($"{this._Mode} is unexpected.");
        }
    }
    //
    public T MapToOnWithArgsDefault<T, Args>(
        Args args,
        T result,
        Func<NoValue, Args, T, T>? onNoValue = default,
        Func<GoodValue<V>, Args, T, T>? onGood = default,
        Func<BadValue<V>, Args, T, T>? onBad = default,
        Func<FailValue<F>, Args, T, T>? onFail = default,
        Func<ErrorValue, Args, T, T>? onError = default,
        Func<MayBe<V, F>, Args, T, T>? otherwise = default
    ) {
        switch (this._Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return onNoValue((NoValue)this, args, result);
                } else if (otherwise is not null) {
                    return otherwise(this, args, result);
                } else {
                    return result;
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return onGood((GoodValue<V>)this, args, result);
                } else if (otherwise is not null) {
                    return otherwise(this, args, result);
                } else {
                    return result;
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return onBad((BadValue<V>)this, args, result);
                } else if (otherwise is not null) {
                    return otherwise(this, args, result);
                } else {
                    return result;
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return onFail((FailValue<F>)this, args, result);
                } else if (otherwise is not null) {
                    return otherwise(this, args, result);
                } else {
                    return result;
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return onError((ErrorValue)this, args, result);
                } else if (otherwise is not null) {
                    return otherwise(this, args, result);
                } else {
                    return result;
                }
            default:
                throw new InvalidOperationException($"{this._Mode} is unknown");
        }
    }
    //
    public static implicit operator MayBe<V, F>(GoodValue<V> value)
        => new MayBe<V, F>(MayBeMode.Good, value.Value, default, default);
    public static implicit operator MayBe<V, F>(BadValue<V> value)
        => new MayBe<V, F>(MayBeMode.Bad, value.Value, default, default);
    public static implicit operator MayBe<V, F>(NoValue value)
        => new MayBe<V, F>(MayBeMode.NoValue, default, default, default);
    public static implicit operator MayBe<V, F>(FailValue<F> value)
        => new MayBe<V, F>(MayBeMode.Fail, default, value.Failure, default);
    public static implicit operator MayBe<V, F>(ErrorValue value)
        => new MayBe<V, F>(MayBeMode.Error, default, default, value.Error);

    public static explicit operator NoValue(MayBe<V, F> value)
        => (value.Mode == MayBeMode.NoValue) ? new NoValue() : throw new InvalidCastException($"Cannot convert a MayBe({value._Mode}) to NoValue");
    public static explicit operator GoodValue<V>(MayBe<V, F> value)
        => (value.Mode == MayBeMode.Good) ? new GoodValue<V>(value.Value!) : throw new InvalidCastException($"Cannot convert a MayBe({value._Mode}) to GoodValue");
    public static explicit operator BadValue<V>(MayBe<V, F> value)
        => (value.Mode == MayBeMode.Bad) ? new BadValue<V>(value.Value!) : throw new InvalidCastException($"Cannot convert a MayBe({value._Mode}) to BadValue");
    public static explicit operator FailValue<F>(MayBe<V, F> value)
        => (value.Mode == MayBeMode.Fail) ? new FailValue<F>(value.Failure!) : throw new InvalidCastException($"Cannot convert a MayBe({value._Mode}) to FailValue");
    public static explicit operator ErrorValue(MayBe<V, F> value)
        => (value.Mode == MayBeMode.Error) ? new ErrorValue(value.Error!) : throw new InvalidCastException($"Cannot convert a MayBe({value._Mode}) to ErrorValue");

}
