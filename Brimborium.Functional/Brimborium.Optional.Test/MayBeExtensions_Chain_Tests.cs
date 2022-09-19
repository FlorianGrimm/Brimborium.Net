#pragma warning disable xUnit2003 // Do not use equality check to test for null value
#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions

using System.Threading.Tasks;

namespace Brimborium.Optional;

public class MayBeExtensions_Chain_Tests {
    [Fact]
    public void Chain_Test() {
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = sut.Chain(
                (that) => MayBe.GoodValue(3)
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = sut.Chain(
                (that) => MayBe.GoodValue(3),
                (that) => throw new InvalidOperationException("4")
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("4", r?.Message);
        }

        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = sut.Chain(
                (that) => MayBe.GoodValue(3)
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }
        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = sut.Chain(
                (that) => MayBe.GoodValue(3),
                (that) => throw new InvalidOperationException("4")
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("4", r?.Message);
        }

        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = sut.Chain(
                (that) => MayBe.GoodValue(3)
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }
        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = sut.Chain(
                (that) => MayBe.GoodValue(3),
                (that) => throw new InvalidOperationException("4")
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("4", r?.Message);
        }

        {
            MayBe<int, string> sut = MayBe.FailValue("1");
            var act = sut.Chain(
                (that) => MayBe.GoodValue(3),
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(MayBeMode.Fail, act.Mode);
        }

        {
            MayBe<int, string> sut = MayBe.ErrorValue(new Exception("1"));
            var act = sut.Chain(
                (that) => MayBe.GoodValue(3),
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("1", r?.Message);
        }
    }

    [Fact]
    public async Task ChainAsync_Test() {
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = await sut.ChainAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); }
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = await sut.ChainAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); },
                (that) => throw new InvalidOperationException("4")
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("4", r?.Message);
        }

        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); }
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }
        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); },
                (that) => throw new InvalidOperationException("4")
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("4", r?.Message);
        }

        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); }
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }
        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); },
                (that) => throw new InvalidOperationException("4")
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("4", r?.Message);
        }

        {
            MayBe<int, string> sut = MayBe.FailValue("1");
            var act = await sut.ChainAsync(
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(MayBeMode.Fail, act.Mode);
        }

        {
            MayBe<int, string> sut = MayBe.ErrorValue(new Exception("1"));
            var act = await sut.ChainAsync(
                (that) => throw new InvalidOperationException("2")
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("1", r?.Message);
        }
    }

    [Fact]
    public async Task ChainAsyncWithTaskValue_Test() {
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = await sut.ChainAsync().ChainAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); }
                );
            Assert.Equal(MayBeMode.Good, act.Mode);
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }

        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainAsync().ChainAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); },
                (that) => throw new InvalidOperationException("4")
                );
            Assert.Equal(MayBeMode.Error, act.Mode);
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("4", r?.Message);
        }

        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainAsync().ChainAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); }
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }

        {
            MayBe<int, string> sut = MayBe.FailValue("1");
            var act = await sut.ChainAsync().ChainAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); }
                );
            Assert.Equal(MayBeMode.Fail, act.Mode);
        }

        {
            MayBe<int, string> sut = MayBe.ErrorValue(new Exception("1"));
            var act = await sut.ChainAsync().ChainAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); }
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("1", r?.Message);
        }
    }

    [Fact]
    public void ChainOn_Test() {
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = sut.ChainOn(
                2,
                (that, args) => MayBe.GoodValue(3)
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = sut.ChainOn(
                2,
                (that, args) => MayBe.GoodValue(3),
                (that, args) => throw new InvalidOperationException("4")
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("4", r?.Message);
        }

        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = sut.ChainOn(
                2,
                (that, args) => MayBe.GoodValue(3)
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }
        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = sut.ChainOn(
                2,
                (that, args) => MayBe.GoodValue(3),
                (that, args) => throw new InvalidOperationException("4")
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("4", r?.Message);
        }

        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = sut.ChainOn(
                2,
                (that, args) => MayBe.GoodValue(3)
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }
        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = sut.ChainOn(
                2,
                (that, args) => MayBe.GoodValue(3),
                (that, args) => throw new InvalidOperationException("4")
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("4", r?.Message);
        }

        {
            MayBe<int, string> sut = MayBe.FailValue("1");
            var act = sut.ChainOn(
                2,
                (that, args) => MayBe.GoodValue(3),
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(MayBeMode.Fail, act.Mode);
        }

        {
            MayBe<int, string> sut = MayBe.ErrorValue(new Exception("1"));
            var act = sut.ChainOn(
                2,
                (that, args) => MayBe.GoodValue(3),
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("1", r?.Message);
        }
    }

    [Fact]
    public async Task ChainOnAsync_Test() {
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = await sut.ChainOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); }
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3+2, r);
        }
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = await sut.ChainOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); },
                (that, args) => throw new InvalidOperationException("4")
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("4", r?.Message);
        }

        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); }
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3+2, r);
        }
        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); },
                (that, args) => throw new InvalidOperationException("4")
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("4", r?.Message);
        }

        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); }
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3+2, r);
        }
        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); },
                (that, args) => throw new InvalidOperationException("4")
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("4", r?.Message);
        }

        {
            MayBe<int, string> sut = MayBe.FailValue("1");
            var act = await sut.ChainOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); },
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(MayBeMode.Fail, act.Mode);
        }

        {
            MayBe<int, string> sut = MayBe.ErrorValue(new Exception("1"));
            var act = await sut.ChainOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); },
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("1", r?.Message);
        }
    }

    [Fact]
    public async Task ChainOnAsyncWithTaskValue_Test() {
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = await sut.ChainAsync().ChainOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); }
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3 + 2, r);
        }
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = await sut.ChainAsync().ChainOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); },
                (that, args) => throw new InvalidOperationException("4")
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("4", r?.Message);
        }

        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainAsync().ChainOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); }
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3 + 2, r);
        }
        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainAsync().ChainOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); },
                (that, args) => throw new InvalidOperationException("4")
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("4", r?.Message);
        }

        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainAsync().ChainOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); }
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3 + 2, r);
        }
        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainAsync().ChainOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); },
                (that, args) => throw new InvalidOperationException("4")
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("4", r?.Message);
        }

        {
            MayBe<int, string> sut = MayBe.FailValue("1");
            var act = await sut.ChainAsync().ChainOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); },
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(MayBeMode.Fail, act.Mode);
        }

        {
            MayBe<int, string> sut = MayBe.ErrorValue(new Exception("1"));
            var act = await sut.ChainAsync().ChainOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); },
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("1", r?.Message);
        }
    }
}
