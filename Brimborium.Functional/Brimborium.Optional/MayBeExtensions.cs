namespace Brimborium.Optional;

public static class MayBeExtensions {
    /// <summary>
    /// return the result of the first case that returns successfull or failure otherwise defaultResult(this).
    /// </summary>
    /// <typeparam name="V">Type of the value</typeparam>
    /// <typeparam name="E">Type of the Failure-Value</typeparam>
    /// <param name="defaultResult">defaultResult (this) is return if no case returns successfull or failure</param>
    /// <param name="cases">the cases are executed in order, the first that returns successfull or failure is used as result</param>
    /// <returns>the first case result (successfull or failure) or the defaultResult(this).</returns>
    public static MayBe<V, E> Switch<V, E>(
            this MayBeValue<V, E> defaultResult,
            params Func<MayBeValue<V, E>, MayBe<V, E>?>[] cases
            ) {
        foreach (var currentCase in cases) {
            var result = currentCase(defaultResult);
            if (result is not null) {
                if (result.Fail || result.Success) {
                    return result;
                }
            }
        }
        return defaultResult;
    }

    /// <summary>
    /// return the result of the first case that returns successfull or failure otherwise defaultResult(this).
    /// </summary>
    /// <typeparam name="V">Type of the value</typeparam>
    /// <typeparam name="E">Type of the Failure-Value</typeparam>
    /// <typeparam name="Args">Type of the Arguments</typeparam>
    /// <param name="defaultResult">defaultResult (this) is return if no case returns successfull or failure</param>
    /// <param name="args">extra arguments to avoid scoping</param>
    /// <param name="cases">the cases are executed in order, the first that returns successfull or failure is used as result</param>
    /// <returns>the first execution result (successfull or failure) or the defaultResult(this).</returns>
    public static MayBe<V, E> SwitchOn<V, E, Args>(
            this MayBeValue<V, E> defaultResult,
            Args args,
            params Func<MayBeValue<V, E>, Args, MayBe<V, E>?>[] cases
            ) {
        foreach (var currentCase in cases) {
            var result = currentCase(defaultResult, args);
            if (result is not null) {
                if (result.Fail || result.Success) {
                    return result;
                }
            }
        }
        return defaultResult;
    }

    /// <summary>
    /// return the result of the first case that returns successfull or failure otherwise defaultResult(this).
    /// </summary>
    /// <typeparam name="V">Type of the value</typeparam>
    /// <typeparam name="E">Type of the Failure-Value</typeparam>
    /// <typeparam name="Args">Type of the Arguments</typeparam>
    /// <param name="defaultResult">defaultResult (this) is return if no case returns successfull or failure</param>
    /// <param name="args">extra arguments to avoid scoping</param>
    /// <param name="cases">the cases are executed in order, the first that returns successfull or failure is used as result</param>
    /// <returns>the first execution result (successfull or failure) or the defaultResult(this).</returns>
    public static async Task<MayBe<V, E>> SwitchOnAsync<V, E, Args>(
            this MayBeValue<V, E> defaultResult,
            Args args,
            params Func<MayBeValue<V, E>, Args, Task<MayBe<V, E>?>?>[] cases
            ) {
        foreach (var currentCase in cases) {
            var task = currentCase(defaultResult, args);
            if (task is not null) {
                var result = await task;
                if (result is not null) {
                    if (result.Fail || result.Success) {
                        return result;
                    }
                }
            }
        }
        return defaultResult;
    }

#if no
    public static async Task<MayBe<V, E>> ConditionalAsync<V, E, I>(
            this MayBeValue<V, E> defaultResult,
            I args,
            Func<MayBeValue<V, E>, I, Task<MayBe<V, E>?>?> thenCase
            ) {
        var task = thenCase(defaultResult, args);
        if (task is not null) {
            var result = await task;
            if (result is not null) {
                if (result.Fail || result.Success) {
                    return result;
                }
            }
        }
        return defaultResult;
    }
#endif



    /// <summary>
    /// return the result of the last step that returns successfull or the first failure otherwise value(this).
    /// </summary>
    /// <typeparam name="V">Type of the value</typeparam>
    /// <typeparam name="F">Type of the Failure-Value</typeparam>
    /// <param name="value">value (this) is returned if no case returns successfull or failure</param>
    /// <param name="steps">the steps are executed in order, the last that returns successfull or the first failure is used as result</param>
    /// <returns>the last execution result of successfull or the first failure or the value(this).</returns>
    public static MayBe<V, E> Chain<V, E>(
        this MayBeValue<V, E> value,
        params Func<MayBeValue<V, E>, MayBe<V, E>?>[] steps
        ) {
        if (value is MayBeValue<V, E> mayBaValue) {
            foreach (var currentStep in steps) {
                var result = currentStep(mayBaValue);
                if (result is not null) {
                    if (result.Fail) {
                        return result;
                    }
                    if (result.Success) {
                        if (result is MayBeValue<V, E> resultMayBeValue) {
                            mayBaValue = resultMayBeValue;
                        } else {
                            throw new InvalidOperationException("result.Success, but result is not a result is MayBeValue<V, E>.");
                        }
                    }
                }
            }
            return mayBaValue;
        } else {
            return value;
        }
    }


    /// <summary>
    /// return the result of the last step that returns successfull or the first failure otherwise value(this).
    /// </summary>
    /// <typeparam name="V">Type of the value</typeparam>
    /// <typeparam name="F">Type of the Failure-Value</typeparam>
    /// <typeparam name="Args">Type of the Arguments</typeparam>
    /// <param name="value">value (this) is returned if no case returns successfull or failure</param>
    /// <param name="args">extra arguments to avoid scoping</param>
    /// <param name="steps">the steps are executed in order, the last that returns successfull or the first failure is used as result</param>
    /// <returns>the last execution result of successfull or the first failure or the value(this).</returns>
    public static async Task<MayBe<V, E>> ChainAsync<V, E>(
            this MayBe<V, E> value,
            params Func<MayBeValue<V, E>, Task<MayBe<V, E>?>?>[] steps
            ) {
        if (value is MayBeValue<V, E> mayBaValue) {
            foreach (var currentStep in steps) {
                var task = currentStep(mayBaValue);
                if (task is not null) {
                    var result = await task;
                    if (result is not null) {
                        if (result.Fail) {
                            return result;
                        }
                        if (result.Success) {
                            if (result is MayBeValue<V, E> resultMayBeValue) {
                                mayBaValue = resultMayBeValue;
                            } else {
                                throw new InvalidOperationException("result.Success, but result is not a result is MayBeValue<V, E>.");
                            }
                        }
                    }
                }
            }
            return mayBaValue;
        } else {
            return value;
        }
    }

    /// <summary>
    /// return the result of the last step that returns successfull or the first failure otherwise value(this).
    /// </summary>
    /// <typeparam name="V">Type of the value</typeparam>
    /// <typeparam name="F">Type of the Failure-Value</typeparam>
    /// <typeparam name="Args">Type of the Arguments</typeparam>
    /// <param name="taskOfValue">value (this) is returned if no case returns successfull or failure</param>
    /// <param name="args">extra arguments to avoid scoping</param>
    /// <param name="steps">the steps are executed in order, the last that returns successfull or the first failure is used as result</param>
    /// <returns>the last execution result of successfull or the first failure or the value(this).</returns>
    public static async Task<MayBe<V, E>> ChainAsync<V, E>(
            this Task<MayBe<V, E>> taskOfValue,
            params Func<MayBeValue<V, E>, Task<MayBe<V, E>?>?>[] steps
            ) {
        var value = await taskOfValue;
        if (value is MayBeValue<V, E> mayBaValue) {
            foreach (var currentStep in steps) {
                var task = currentStep(mayBaValue);
                if (task is not null) {
                    var result = await task;
                    if (result is not null) {
                        if (result.Fail) {
                            return result;
                        }
                        if (result.Success) {
                            if (result is MayBeValue<V, E> resultMayBeValue) {
                                mayBaValue = resultMayBeValue;
                            } else {
                                throw new InvalidOperationException("result.Success, but result is not a result is MayBeValue<V, E>.");
                            }
                        }
                    }
                }
            }
            return mayBaValue;
        } else {
            return value;
        }
    }

    /// <summary>
    /// return the result of the last step that returns successfull or the first failure otherwise value(this).
    /// </summary>
    /// <typeparam name="V">Type of the value</typeparam>
    /// <typeparam name="F">Type of the Failure-Value</typeparam>
    /// <typeparam name="Args">Type of the Arguments</typeparam>
    /// <param name="value">value (this) is returned if no case returns successfull or failure</param>
    /// <param name="args">extra arguments to avoid scoping</param>
    /// <param name="steps">the steps are executed in order, the last that returns successfull or the first failure is used as result</param>
    /// <returns>the last execution result of successfull or the first failure or the value(this).</returns>
    public static MayBe<V, E> ChainOn<V, E, Args>(
            this MayBe<V, E> value,
            Args args,
            params Func<MayBeValue<V, E>, Args, MayBe<V, E>?>[] steps
            ) {
        if (value is MayBeValue<V, E> mayBaValue) {
            foreach (var currentStep in steps) {

                var result = currentStep(mayBaValue, args);
                if (result is not null) {
                    if (result.Fail) {
                        return result;
                    }
                    if (result.Success) {
                        if (result is MayBeValue<V, E> resultMayBeValue) {
                            mayBaValue = resultMayBeValue;
                        } else {
                            throw new InvalidOperationException("result.Success, but result is not a result is MayBeValue<V, E>.");
                        }
                    }
                }
            }
            return mayBaValue;
        } else {
            return value;
        }
    }


    /// <summary>
    /// return the result of the last step that returns successfull or the first failure otherwise value(this).
    /// </summary>
    /// <typeparam name="V">Type of the value</typeparam>
    /// <typeparam name="F">Type of the Failure-Value</typeparam>
    /// <typeparam name="Args">Type of the Arguments</typeparam>
    /// <param name="value">value (this) is returned if no case returns successfull or failure</param>
    /// <param name="args">extra arguments to avoid scoping</param>
    /// <param name="steps">the steps are executed in order, the last that returns successfull or the first failure is used as result</param>
    /// <returns>the last execution result of successfull or the first failure or the value(this).</returns>
    public static async Task<MayBe<V, E>> ChainOnAsync<V, E, Args>(
            this MayBeValue<V, E> value,
            Args args,
            params Func<MayBeValue<V, E>, Args, Task<MayBe<V, E>?>?>[] steps
            ) {
        if (value is MayBeValue<V, E> mayBaValue) {
            foreach (var currentStep in steps) {
                var task = currentStep(mayBaValue, args);
                if (task is not null) {
                    var result = await task;
                    if (result is not null) {
                        if (result.Fail) {
                            return result;
                        }
                        if (result.Success) {
                            if (result is MayBeValue<V, E> resultMayBeValue) {
                                mayBaValue = resultMayBeValue;
                            } else {
                                throw new InvalidOperationException("result.Success, but result is not a result is MayBeValue<V, E>.");
                            }
                        }
                    }
                }
            }
            return mayBaValue;
        } else {
            return value;
        }
    }


    /// <summary>
    /// return the result of the last step that returns successfull or the first failure otherwise value(this).
    /// </summary>
    /// <typeparam name="V">Type of the value</typeparam>
    /// <typeparam name="F">Type of the Failure-Value</typeparam>
    /// <typeparam name="Args">Type of the Arguments</typeparam>
    /// <param name="value">value (this) is returned if no case returns successfull or failure</param>
    /// <param name="args">extra arguments to avoid scoping</param>
    /// <param name="steps">the steps are executed in order, the last that returns successfull or the first failure is used as result</param>
    /// <returns>the last execution result of successfull or the first failure or the value(this).</returns>
    public static async Task<MayBe<V, E>> ChainOnAsync<V, E, Args>(
            this Task<MayBeValue<V, E>> taskOfValue,
            Args args,
            params Func<MayBeValue<V, E>, Args, Task<MayBe<V, E>?>?>[] steps
            ) {
        var value = await taskOfValue;
        if (value is MayBeValue<V, E> mayBaValue) {
            foreach (var currentStep in steps) {
                var task = currentStep(mayBaValue, args);
                if (task is not null) {
                    var result = await task;
                    if (result is not null) {
                        if (result.Fail) {
                            return result;
                        }
                        if (result.Success) {
                            if (result is MayBeValue<V, E> resultMayBeValue) {
                                mayBaValue = resultMayBeValue;
                            } else {
                                throw new InvalidOperationException("result.Success, but result is not a result is MayBeValue<V, E>.");
                            }
                        }
                    }
                }
            }
            return mayBaValue;
        } else {
            return value;
        }
    }
}
