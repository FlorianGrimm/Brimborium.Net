#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions
using System.Threading.Tasks;

namespace Brimborium.Optional;

public class MayBeExtensionsTests {

    [Fact()]
    public void SwitchTest() {
        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.Switch();
            Assert.True(object.ReferenceEquals(act, sut));
        }
        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.Switch(
                (r) => {
                    return r.WithSuccessfullValue(2);
                },
                (r) => {
                    throw new Exception("Failed");
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(2, r);
        }

        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.Switch(
                (r) => {
                    return r.WithUndecidedValue(3);
                },
                (r) => {
                    return r.WithSuccessfullValue(2);
                },
                (r) => {
                    throw new Exception("Failed");
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(2, r);
        }

        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.Switch(
                (r) => {
                    return r.WithFailure(new Exception("4"));
                },
                (r) => {
                    throw new Exception("Failed");
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetFailureValue(out var r));
            Assert.Equal("4", r.Message);
        }
    }

    [Fact()]
    public void SwitchOnTest() {
        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.SwitchOn(args);
            Assert.True(object.ReferenceEquals(act, sut));
        }
        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.SwitchOn(args,
                (r, a) => {
                    Assert.Equal(42, a);
                    return r.WithSuccessfullValue(2);
                },
                (r, a) => {
                    throw new Exception("Failed");
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(2, r);
        }

        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.SwitchOn(args,
                (r, a) => {
                    Assert.Equal(42, a);
                    return r.WithUndecidedValue(3);
                },
                (r, a) => {
                    Assert.Equal(42, a);
                    return r.WithSuccessfullValue(2);
                },
                (r, a) => {
                    throw new Exception("Failed");
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(2, r);
        }

        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.SwitchOn(args,
                (r, a) => {
                    Assert.Equal(42, a);
                    return r.WithFailure(new Exception("4"));
                },
                (r, a) => {
                    throw new Exception("Failed");
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetFailureValue(out var r));
            Assert.Equal("4", r.Message);
        }
    }

    [Fact()]
    public void SwitchOnAsyncTest() {
        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.SwitchOn(args);
            Assert.True(object.ReferenceEquals(act, sut));
        }
        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.SwitchOn(args,
                (r, a) => {
                    Assert.Equal(42, a);
                    return r.WithSuccessfullValue(2);
                },
                (r, a) => {
                    throw new Exception("Failed");
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(2, r);
        }
        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.SwitchOn(args,
                (r, a) => {
                    Assert.Equal(42, a);
                    return r.WithUndecidedValue(2);
                },
                (r, a) => {
                    Assert.Equal(42, a);
                    return r.WithSuccessfullValue(3);
                },
                (r, a) => {
                    throw new Exception("Failed");
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(3, r);
        }
        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.SwitchOn(args,
                (r, a) => {
                    Assert.Equal(42, a);
                    return r.WithUndecidedValue(2);
                },
                (r, a) => {
                    Assert.Equal(42, a);
                    return r.WithFailure(new Exception("ok"));
                },
                (r, a) => {
                    throw new Exception("Failed");
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetFailureValue(out var r));
            Assert.Equal("ok", r.Message);
        }
        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.SwitchOn(args,
                (r, a) => {
                    Assert.Equal(42, a);
                    return r.WithUndecidedValue(2);
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.Success);
            Assert.Equal(true, act.TryGetValue(out var r));
            Assert.Equal(1, r);
        }
        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.SwitchOn(args,
                (r, a) => {
                    Assert.Equal(42, a);
                    return r;
                }
                );
            Assert.NotNull(act);
            Assert.True(ReferenceEquals(act,sut));
        }
    }

    [Fact()]
    public void ChainTest() {
        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.Chain();
            Assert.True(object.ReferenceEquals(act, sut));
        }
        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.Chain(
                (r) => {
                    return r.WithSuccessfullValue(2);
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(2, r);
        }
        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.Chain(
                (r) => {
                    return r.WithSuccessfullValue(2);
                },
                 (r) => {
                     return r.WithSuccessfullValue(3);
                 }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(3, r);
        }

        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.Chain(
                (r) => {
                    return r.WithUndecidedValue(3);
                }
                );
            Assert.True(object.ReferenceEquals(act, sut));
        }

        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.Chain(
                (r) => {
                    return r.WithUndecidedValue(3);
                },
                (r) => {
                    return r.WithSuccessfullValue(2);
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(2, r);
        }

        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.Chain(
                (r) => {
                    return r.WithFailure(new Exception("4"));
                },
                (r) => {
                    throw new Exception("Failed");
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetFailureValue(out var r));
            Assert.Equal("4", r.Message);
        }
    }

    [Fact()]
    public async Task ChainAsyncTest() {
        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = await sut.ChainAsync();
            Assert.True(object.ReferenceEquals(act, sut));
        }
        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = await sut.ChainAsync(
                async (r) => {
                    await Task.CompletedTask;
                    return r.WithSuccessfullValue(2);
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(2, r);
        }
        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = await sut.ChainAsync(
                async (r) => {
                    await Task.CompletedTask;
                    return r.WithSuccessfullValue(2);
                },
                async (r) => {
                    await Task.CompletedTask;
                    return r.WithSuccessfullValue(3);
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(3, r);
        }

        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = await sut.ChainAsync(
                async (r) => {
                    await Task.CompletedTask;
                    return r.WithUndecidedValue(3);
                }
                );
            Assert.True(object.ReferenceEquals(act, sut));
        }

        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = await sut.ChainAsync(
                async (r) => {
                    await Task.CompletedTask;
                    return r.WithUndecidedValue(3);
                },
                async (r) => {
                    await Task.CompletedTask;
                    return r.WithSuccessfullValue(2);
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(2, r);
        }

        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = await sut.ChainAsync(
                async (r) => {
                    await Task.CompletedTask;
                    return r.WithFailure(new Exception("4"));
                },
                async (r) => {
                    await Task.CompletedTask;
                    throw new Exception("Failed");
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetFailureValue(out var r));
            Assert.Equal("4", r.Message);
        }
    }

    [Fact()]
    public void ChainOnTest() {
        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.ChainOn(args);
            Assert.True(object.ReferenceEquals(act, sut));
        }
        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.SwitchOn(args,
                (r, a) => {
                    Assert.Equal(42, a);
                    return r.WithSuccessfullValue(2);
                },
                (r, a) => {
                    throw new Exception("Failed");
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(2, r);
        }

        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.ChainOn(args,
                (r, a) => {
                    Assert.Equal(42, a);
                    return r.WithUndecidedValue(3);
                }
                );
            Assert.True(object.ReferenceEquals(act, sut));
        }

        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.ChainOn(args,
                (r, a) => {
                    Assert.Equal(42, a);
                    return r.WithUndecidedValue(3);
                },
                (r, a) => {
                    Assert.Equal(42, a);
                    return r.WithSuccessfullValue(2);
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(2, r);
        }

        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.ChainOn(args,
                (r, a) => {
                    Assert.Equal(42, a);
                    return r.WithFailure(new Exception("4"));
                },
                (r, a) => {
                    throw new Exception("Failed");
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetFailureValue(out var r));
            Assert.Equal("4", r.Message);
        }
    }

    [Fact()]
    public async Task ChainOnAsyncTest() {
        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = await sut.ChainOnAsync(args);
            Assert.True(object.ReferenceEquals(act, sut));
        }
        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = await sut.SwitchOnAsync(args,
                async (r, a) => {
                    Assert.Equal(42, a);
                    await Task.CompletedTask;
                    return r.WithSuccessfullValue(2);
                },
                async (r, a) => {
                    await Task.CompletedTask;
                    throw new Exception("Failed");
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(2, r);
        }

        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = await sut.SwitchOnAsync(args,
                async (r, a) => {
                    Assert.Equal(42, a);
                    await Task.CompletedTask;
                    return r.WithUndecidedValue(3);
                }
                );
            Assert.True(object.ReferenceEquals(act, sut));
        }

        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = await sut.SwitchOnAsync(args,
                async (r, a) => {
                    Assert.Equal(42, a);
                    await Task.CompletedTask;
                    return r.WithUndecidedValue(3);
                },
                async (r, a) => {
                    Assert.Equal(42, a);
                    await Task.CompletedTask;
                    return r.WithSuccessfullValue(2);
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(2, r);
        }

        {
            var args = 42;
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = await sut.SwitchOnAsync(args,
                async (r, a) => {
                    Assert.Equal(42, a);
                    await Task.CompletedTask;
                    return r.WithFailure(new Exception("4"));
                },
                async (r, a) => {
                    await Task.CompletedTask;
                    throw new Exception("Failed");
                }
                );
            Assert.NotNull(act);
            Assert.Equal(true, act.TryGetFailureValue(out var r));
            Assert.Equal("4", r.Message);
        }
    }
}
