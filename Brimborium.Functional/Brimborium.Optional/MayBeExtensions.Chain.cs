namespace Brimborium.Optional;

//
public static partial class MayBeExtensions {
    public static MayBe<V, F> Chain<V, F>(
        this MayBe<V, F> that,
        params Func<MayBe<V, F>, MayBe<V, F>>[] steps)
        where V : notnull
        where F : notnull {
        var currentResult = that;
        {
            if (currentResult.Mode == MayBeMode.Good) {
                // continue
            } else if (currentResult.Mode == MayBeMode.Bad) {
                // continue
            } else if (currentResult.Mode == MayBeMode.NoValue) {
                // continue
            } else if (currentResult.Mode == MayBeMode.Fail) {
                return currentResult;
            } else if (currentResult.Mode == MayBeMode.Error) {
                return currentResult;
            } else {
                throw new ArgumentException("unknown mode", nameof(currentResult.Mode));
            }
        }
        try {
            for (int idx = 0; idx < steps.Length; idx++) {
                var step = steps[idx];
                var stepResult = step(currentResult);
                if (stepResult.Mode == MayBeMode.Good) {
                    currentResult = stepResult;
                } else if (stepResult.Mode == MayBeMode.Bad) {
                    continue;
                } else if (stepResult.Mode == MayBeMode.NoValue) {
                    continue;
                } else if (stepResult.Mode == MayBeMode.Fail) {
                    return stepResult;
                } else if (stepResult.Mode == MayBeMode.Error) {
                    return stepResult;
                } else {
                    throw new ArgumentException("unknown mode", nameof(stepResult.Mode));
                }
            }
        } catch (Exception error) {
            return new ErrorValue(error);
        }
        return currentResult;
    }
    //
    public static MayBe<V, F> ChainOn<V, F, Args>(
        this MayBe<V, F> that,
        Args args,
        params Func<MayBe<V, F>, Args, MayBe<V, F>>[] steps)
        where V : notnull
        where F : notnull {
        var currentResult = that;
        {
            if (currentResult.Mode == MayBeMode.Good) {
                // continue
            } else if (currentResult.Mode == MayBeMode.Bad) {
                // continue
            } else if (currentResult.Mode == MayBeMode.NoValue) {
                // continue
            } else if (currentResult.Mode == MayBeMode.Fail) {
                return currentResult;
            } else if (currentResult.Mode == MayBeMode.Error) {
                return currentResult;
            } else {
                throw new ArgumentException("unknown mode", nameof(currentResult.Mode));
            }
        }
        try {
            for (int idx = 0; idx < steps.Length; idx++) {
                var step = steps[idx];
                var stepResult = step(currentResult, args);
                if (stepResult.Mode == MayBeMode.Good) {
                    currentResult = stepResult;
                } else if (stepResult.Mode == MayBeMode.Bad) {
                    continue;
                } else if (stepResult.Mode == MayBeMode.NoValue) {
                    continue;
                } else if (stepResult.Mode == MayBeMode.Fail) {
                    return stepResult;
                } else if (stepResult.Mode == MayBeMode.Error) {
                    return stepResult;
                } else {
                    throw new ArgumentException("unknown mode", nameof(stepResult.Mode));
                }
            }
        } catch (Exception error) {
            return new ErrorValue(error);
        }
        return currentResult;
    }
    //
    public static async Task<MayBe<V, F>> ChainAsync<V, F>(
        this MayBe<V, F> that,
        params Func<MayBe<V, F>, Task<MayBe<V, F>>>[] steps)
        where V : notnull
        where F : notnull {
        var currentResult = that;
        {
            if (currentResult.Mode == MayBeMode.Good) {
                // continue
            } else if (currentResult.Mode == MayBeMode.Bad) {
                // continue
            } else if (currentResult.Mode == MayBeMode.NoValue) {
                // continue
            } else if (currentResult.Mode == MayBeMode.Fail) {
                return currentResult;
            } else if (currentResult.Mode == MayBeMode.Error) {
                return currentResult;
            } else {
                throw new ArgumentException("unknown mode", nameof(currentResult.Mode));
            }
        }
        try {
            for (int idx = 0; idx < steps.Length; idx++) {
                var step = steps[idx];
                var taskOfStepResult = step(currentResult);
                var stepResult = await taskOfStepResult;
                if (stepResult.Mode == MayBeMode.Good) {
                    currentResult = stepResult;
                } else if (stepResult.Mode == MayBeMode.Bad) {
                    continue;
                } else if (stepResult.Mode == MayBeMode.NoValue) {
                    continue;
                } else if (stepResult.Mode == MayBeMode.Fail) {
                    return stepResult;
                } else if (stepResult.Mode == MayBeMode.Error) {
                    return stepResult;
                } else {
                    throw new ArgumentException("unknown mode", nameof(stepResult.Mode));
                }
            }
        } catch (Exception error) {
            return new ErrorValue(error);
        }
        return currentResult;
    }
    //
    public static async Task<MayBe<V, F>> ChainAsync<V, F>(
        this Task<MayBe<V, F>> that,
        params Func<MayBe<V, F>, Task<MayBe<V, F>>>[] steps)
        where V : notnull
        where F : notnull {
        MayBe<V, F> currentResult;
        try {
            currentResult = await that;
        } catch (Exception error) {
            return new ErrorValue(error);
        }
        {
            if (currentResult.Mode == MayBeMode.Good) {
                // continue
            } else if (currentResult.Mode == MayBeMode.Bad) {
                // continue
            } else if (currentResult.Mode == MayBeMode.NoValue) {
                // continue
            } else if (currentResult.Mode == MayBeMode.Fail) {
                return currentResult;
            } else if (currentResult.Mode == MayBeMode.Error) {
                return currentResult;
            } else {
                throw new ArgumentException("unknown mode", nameof(currentResult.Mode));
            }
        }
        try {
            for (int idx = 0; idx < steps.Length; idx++) {
                var step = steps[idx];
                var taskOfStepResult = step(currentResult);
                var stepResult = await taskOfStepResult;
                if (stepResult.Mode == MayBeMode.Good) {
                    currentResult = stepResult;
                } else if (stepResult.Mode == MayBeMode.Bad) {
                    continue;
                } else if (stepResult.Mode == MayBeMode.NoValue) {
                    continue;
                } else if (stepResult.Mode == MayBeMode.Fail) {
                    return stepResult;
                } else if (stepResult.Mode == MayBeMode.Error) {
                    return stepResult;
                } else {
                    throw new ArgumentException("unknown mode", nameof(stepResult.Mode));
                }
            }
        } catch (Exception error) {
            return new ErrorValue(error);
        }
        return currentResult;
    }
    //
    public static async Task<MayBe<V, F>> ChainOnAsync<V, F, Args>(
        this MayBe<V, F> that,
        Args args,
        params Func<MayBe<V, F>, Args, Task<MayBe<V, F>>>[] steps)
        where V : notnull
        where F : notnull {
        var currentResult = that;
        {
            if (currentResult.Mode == MayBeMode.Good) {
                // continue
            } else if (currentResult.Mode == MayBeMode.Bad) {
                // continue
            } else if (currentResult.Mode == MayBeMode.NoValue) {
                // continue
            } else if (currentResult.Mode == MayBeMode.Fail) {
                return currentResult;
            } else if (currentResult.Mode == MayBeMode.Error) {
                return currentResult;
            } else {
                throw new ArgumentException("unknown mode", nameof(currentResult.Mode));
            }
        }
        try {
            for (int idx = 0; idx < steps.Length; idx++) {
                var step = steps[idx];
                var taskOfStepResult = step(currentResult, args);
                var stepResult = await taskOfStepResult;
                if (stepResult.Mode == MayBeMode.Good) {
                    currentResult = stepResult;
                } else if (stepResult.Mode == MayBeMode.Bad) {
                    continue;
                } else if (stepResult.Mode == MayBeMode.NoValue) {
                    continue;
                } else if (stepResult.Mode == MayBeMode.Fail) {
                    return stepResult;
                } else if (stepResult.Mode == MayBeMode.Error) {
                    return stepResult;
                } else {
                    throw new ArgumentException("unknown mode", nameof(stepResult.Mode));
                }
            }
        } catch (Exception error) {
            return new ErrorValue(error);
        }
        return currentResult;
    }
    //
    public static async Task<MayBe<V, F>> ChainOnAsync<V, F, Args>(
        this Task<MayBe<V, F>> that,
        Args args,
        params Func<MayBe<V, F>, Args, Task<MayBe<V, F>>>[] steps)
        where V : notnull
        where F : notnull {
        MayBe<V, F> currentResult;
        try {
            currentResult = await that;
        } catch (Exception error) { 
            return new ErrorValue(error);
        }
        {
            if (currentResult.Mode == MayBeMode.Good) {
                // continue
            } else if (currentResult.Mode == MayBeMode.Bad) {
                // continue
            } else if (currentResult.Mode == MayBeMode.NoValue) {
                // continue
            } else if (currentResult.Mode == MayBeMode.Fail) {
                return currentResult;
            } else if (currentResult.Mode == MayBeMode.Error) {
                return currentResult;
            } else {
                throw new ArgumentException("unknown mode", nameof(currentResult.Mode));
            }
        }
        try {
            for (int idx = 0; idx < steps.Length; idx++) {
                var step = steps[idx];
                var taskOfStepResult = step(currentResult, args);
                var stepResult = await taskOfStepResult;
                if (stepResult.Mode == MayBeMode.Good) {
                    currentResult = stepResult;
                } else if (stepResult.Mode == MayBeMode.Bad) {
                    continue;
                } else if (stepResult.Mode == MayBeMode.NoValue) {
                    continue;
                } else if (stepResult.Mode == MayBeMode.Fail) {
                    return stepResult;
                } else if (stepResult.Mode == MayBeMode.Error) {
                    return stepResult;
                } else {
                    throw new ArgumentException("unknown mode", nameof(stepResult.Mode));
                }
            }
        } catch (Exception error) {
            return new ErrorValue(error);
        }
        return currentResult;
    }
    //
}
