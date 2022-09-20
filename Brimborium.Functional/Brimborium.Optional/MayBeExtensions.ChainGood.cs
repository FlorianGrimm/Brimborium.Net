namespace Brimborium.Optional;

public static partial class MayBeExtensions {
    public static MayBe<V, F> ChainGood<V, F>(
        this MayBe<V, F> that,
        params Func<GoodValue<V>, MayBe<V, F>>[] steps)
        where V : notnull
        where F : notnull {
        if (that.TryGetAsGoodValue(out var currentGoodResult)) {
            try {
                for (int idx = 0; idx < steps.Length; idx++) {
                    var step = steps[idx];
                    var stepResult = step(currentGoodResult);
                    if (stepResult.TryGetAsGoodValue(out var stepResultGoodValue)) {
                        currentGoodResult = stepResultGoodValue;
                    } else if (stepResult.IsBadOrNoValue) {
                        continue;
                    } else if (stepResult.IsFailOrError) {
                        return stepResult;
                    } else {
                        throw new ArgumentException("unknown mode", nameof(stepResult.Mode));
                    }
                }
                return currentGoodResult;
            } catch (Exception error) {
                return new ErrorValue(error);
            }
        }
        return that;
    }
    //
    public static MayBe<V, F> ChainOnGood<V, F, Args>(
        this MayBe<V, F> that,
        Args args,
        params Func<GoodValue<V>, Args, MayBe<V, F>>[] steps)
        where V : notnull
        where F : notnull {
        if (that.TryGetAsGoodValue(out var currentGoodResult)) {
            try {
                for (int idx = 0; idx < steps.Length; idx++) {
                    var step = steps[idx];
                    var stepResult = step(currentGoodResult, args);
                    if (stepResult.TryGetAsGoodValue(out var stepResultGoodValue)) {
                        currentGoodResult = stepResultGoodValue;
                    } else if (stepResult.IsBadOrNoValue) {
                        continue;
                    } else if (stepResult.IsFailOrError) {
                        return stepResult;
                    } else {
                        throw new ArgumentException("unknown mode", nameof(stepResult.Mode));
                    }
                }
                return currentGoodResult;
            } catch (Exception error) {
                return new ErrorValue(error);
            }
        }
        return that;
    }
    //
    public static async Task<MayBe<V, F>> ChainGoodAsync<V, F>(
        this MayBe<V, F> that,
        params Func<GoodValue<V>, Task<MayBe<V, F>>>[] steps)
        where V : notnull
        where F : notnull {
        if (that.TryGetAsGoodValue(out var currentGoodResult)) {
            try {
                for (int idx = 0; idx < steps.Length; idx++) {
                    var step = steps[idx];
                    var taskOfStepResult = step(currentGoodResult);
                    var stepResult = await taskOfStepResult;
                    if (stepResult.TryGetAsGoodValue(out var stepResultGoodValue)) {
                        currentGoodResult = stepResultGoodValue;
                    } else if (stepResult.IsBadOrNoValue) {
                        continue;
                    } else if (stepResult.IsFailOrError) {
                        return stepResult;
                    } else {
                        throw new ArgumentException("unknown mode", nameof(stepResult.Mode));
                    }
                }
                return currentGoodResult;
            } catch (Exception error) {
                return new ErrorValue(error);
            }
        }
        return that;
    }
    //
    public static async Task<MayBe<V, F>> ChainGoodAsync<V, F>(
        this Task<MayBe<V, F>> that,
        params Func<GoodValue<V>, Task<MayBe<V, F>>>[] steps)
        where V : notnull
        where F : notnull {
        MayBe<V, F> currentResult;
        try {
            currentResult = await that;
        } catch (Exception error) {
            return new ErrorValue(error);
        }
        if (currentResult.TryGetAsGoodValue(out var currentGoodResult)) {
            try {
                for (int idx = 0; idx < steps.Length; idx++) {
                    var step = steps[idx];
                    var taskOfStepResult = step(currentGoodResult);
                    var stepResult = await taskOfStepResult;
                    if (stepResult.TryGetAsGoodValue(out var stepResultGoodValue)) {
                        currentGoodResult = stepResultGoodValue;
                    } else if (stepResult.IsBadOrNoValue) {
                        continue;
                    } else if (stepResult.IsFailOrError) {
                        return stepResult;
                    } else {
                        throw new ArgumentException("unknown mode", nameof(stepResult.Mode));
                    }
                }
                return currentGoodResult;
            } catch (Exception error) {
                return new ErrorValue(error);
            }
        }
        return currentResult;
    }
    //
    public static async Task<MayBe<V, F>> ChainOnGoodAsync<V, F, Args>(
        this MayBe<V, F> that,
        Args args,
        params Func<GoodValue<V>, Args, Task<MayBe<V, F>>>[] steps)
        where V : notnull
        where F : notnull {
        if (that.TryGetAsGoodValue(out var currentGoodResult)) {
            try {
                for (int idx = 0; idx < steps.Length; idx++) {
                    var step = steps[idx];
                    var taskOfStepResult = step(currentGoodResult, args);
                    var stepResult = await taskOfStepResult;
                    if (stepResult.TryGetAsGoodValue(out var stepResultGoodValue)) {
                        currentGoodResult = stepResultGoodValue;
                    } else if (stepResult.IsBadOrNoValue) {
                        continue;
                    } else if (stepResult.IsFailOrError) {
                        return stepResult;
                    } else {
                        throw new ArgumentException("unknown mode", nameof(stepResult.Mode));
                    }
                }
                return currentGoodResult;
            } catch (Exception error) {
                return new ErrorValue(error);
            }
        }
        return that;
    }
    //
    public static async Task<MayBe<V, F>> ChainOnGoodAsync<V, F, Args>(
        this Task<MayBe<V, F>> that,
        Args args,
        params Func<GoodValue<V>, Args, Task<MayBe<V, F>>>[] steps)
        where V : notnull
        where F : notnull {
        MayBe<V, F> currentResult;
        try {
            currentResult = await that;
        } catch (Exception error) { 
            return new ErrorValue(error);
        }
        if (currentResult.TryGetAsGoodValue(out var currentGoodResult)) {
            try {
                for (int idx = 0; idx < steps.Length; idx++) {
                    var step = steps[idx];
                    var taskOfStepResult = step(currentGoodResult, args);
                    var stepResult = await taskOfStepResult;
                    if (stepResult.TryGetAsGoodValue(out var stepResultGoodValue)) {
                        currentGoodResult = stepResultGoodValue;
                    } else if (stepResult.IsBadOrNoValue) {
                        continue;
                    } else if (stepResult.IsFailOrError) {
                        return stepResult;
                    } else {
                        throw new ArgumentException("unknown mode", nameof(stepResult.Mode));
                    }
                }
                return currentGoodResult;
            } catch (Exception error) {
                return new ErrorValue(error);
            }
        }
        return currentResult;
    }
    //
}
