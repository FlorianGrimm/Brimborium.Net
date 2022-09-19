using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Optional;

public struct MayBe<V, F>
    where V : notnull
    where F : notnull {

    private MayBeMode _Mode;
    private V? _Value;
    private F? _Failure;
    private Exception? _Error;

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

    public bool TryGetBadValue([MaybeNullWhen(false)] out V value) {
        if (this._Mode == MayBeMode.Bad) {
            value = this._Value!;
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
    public bool TryGetErrorValue([MaybeNullWhen(false)] out Exception error) {
        if (this._Mode == MayBeMode.Error) {
            error = this._Error!;
            return true;
        }
        error = default;
        return false;
    }
    public MayBeMode Mode => this._Mode;
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
    public MayBe<RV, RF> Map<RV, RF>(
        Func<MayBe<V, F>, MayBe<RV, RF>>? onNoValue = default,
        Func<MayBe<V, F>, MayBe<RV, RF>>? onGood = default,
        Func<MayBe<V, F>, MayBe<RV, RF>>? onBad = default,
        Func<MayBe<V, F>, MayBe<RV, RF>>? onFail = default,
        Func<MayBe<V, F>, MayBe<RV, RF>>? onError = default
    )
        where RV : notnull
        where RF : notnull {
        switch (this._Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return onNoValue(this);
                } else {
                    return new ErrorValue(new InvalidOperationException("MayBeNoValue is null"));
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return onGood(this);
                } else {
                    return new ErrorValue(new InvalidOperationException("Good is null"));
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return onBad(this);
                } else {
                    return new ErrorValue(new InvalidOperationException("Bad is null"));
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return onFail(this);
                } else {
                    return new ErrorValue(new InvalidOperationException("Fail is null"));
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return onError(this);
                } else {
                    return new ErrorValue(new InvalidOperationException("Error is null"));
                }
            default:
                break;
        }
        //this.Mode== MayBeMode.NoValue
        return MayBe.MayBeNoValue<RV, RF>();
    }
    //
    public MayBe<RV, RF> Map<RV, RF>(
        MayBe<RV, RF> result,
        Func<MayBe<V, F>, MayBe<RV, RF>, MayBe<RV, RF>>? onNoValue = default,
        Func<MayBe<V, F>, MayBe<RV, RF>, MayBe<RV, RF>>? onGood = default,
        Func<MayBe<V, F>, MayBe<RV, RF>, MayBe<RV, RF>>? onBad = default,
        Func<MayBe<V, F>, MayBe<RV, RF>, MayBe<RV, RF>>? onFail = default,
        Func<MayBe<V, F>, MayBe<RV, RF>, MayBe<RV, RF>>? onError = default
    )
        where RV : notnull
        where RF : notnull {
        switch (this._Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return onNoValue(this, result);
                } else {
                    return result;
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return onGood(this, result);
                } else {
                    return result;
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return onBad(this, result);
                } else {
                    return result;
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return onFail(this, result);
                } else {
                    return result;
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return onError(this, result);
                } else {
                    return result;
                }
            default:
                return new ErrorValue(new InvalidOperationException($"{this._Mode} is unknown"));
        }
    }
    //
    public MayBe<RV, RF> MapOn<RV, RF, Args>(
        Args args,
        MayBe<RV, RF> result,
        Func<MayBe<V, F>, Args, MayBe<RV, RF>, MayBe<RV, RF>>? onNoValue = default,
        Func<MayBe<V, F>, Args, MayBe<RV, RF>, MayBe<RV, RF>>? onGood = default,
        Func<MayBe<V, F>, Args, MayBe<RV, RF>, MayBe<RV, RF>>? onBad = default,
        Func<MayBe<V, F>, Args, MayBe<RV, RF>, MayBe<RV, RF>>? onFail = default,
        Func<MayBe<V, F>, Args, MayBe<RV, RF>, MayBe<RV, RF>>? onError = default
    )
        where RV : notnull
        where RF : notnull {
        switch (this._Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return onNoValue(this, args, result);
                } else {
                    return result;
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return onGood(this, args, result);
                } else {
                    return result;
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return onBad(this, args, result);
                } else {
                    return result;
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return onFail(this, args, result);
                } else {
                    return result;
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return onError(this, args, result);
                } else {
                    return result;
                }
            default:
                return new ErrorValue(new InvalidOperationException($"{this._Mode} is unknown"));
        }
    }
    //
    public T MapToOn<T, Args>(
        Args args,
        T result,
        Func<MayBe<V, F>, Args, T>? onNoValue = default,
        Func<MayBe<V, F>, Args, T>? onGood = default,
        Func<MayBe<V, F>, Args, T>? onBad = default,
        Func<MayBe<V, F>, Args, T>? onFail = default,
        Func<MayBe<V, F>, Args, T>? onError = default
    ) {
        switch (this._Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return onNoValue(this, args);
                } else {
                    return result;
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return onGood(this, args);
                } else {
                    return result;
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return onBad(this, args);
                } else {
                    return result;
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return onFail(this, args);
                } else {
                    return result;
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return onError(this, args);
                } else {
                    return result;
                }
            default:
                throw new InvalidOperationException($"{this._Mode} is unknown");
        }
    }
    //
    public T MapToOn<T, Args>(
        Args args,
        T result,
        Func<MayBe<V, F>, Args, T, T>? onNoValue = default,
        Func<MayBe<V, F>, Args, T, T>? onGood = default,
        Func<MayBe<V, F>, Args, T, T>? onBad = default,
        Func<MayBe<V, F>, Args, T, T>? onFail = default,
        Func<MayBe<V, F>, Args, T, T>? onError = default
    ) {
        switch (this._Mode) {
            case MayBeMode.NoValue:
                if (onNoValue is not null) {
                    return onNoValue(this, args, result);
                } else {
                    return result;
                }
            case MayBeMode.Good:
                if (onGood is not null) {
                    return onGood(this, args, result);
                } else {
                    return result;
                }
            case MayBeMode.Bad:
                if (onBad is not null) {
                    return onBad(this, args, result);
                } else {
                    return result;
                }
            case MayBeMode.Fail:
                if (onFail is not null) {
                    return onFail(this, args, result);
                } else {
                    return result;
                }
            case MayBeMode.Error:
                if (onError is not null) {
                    return onError(this, args, result);
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
}
