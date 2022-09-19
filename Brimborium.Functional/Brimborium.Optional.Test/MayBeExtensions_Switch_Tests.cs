#pragma warning disable xUnit2003 // Do not use equality check to test for null value
#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions
using System.Threading.Tasks;

namespace Brimborium.Optional;

public class MayBeExtensions_Switch_Tests {
    [Fact]
    public void Switch_Test() {
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = sut.Switch(
                (that) => MayBe.GoodValue(3),
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }

        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = sut.Switch(
                (that) => MayBe.GoodValue(3),
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }

        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = sut.Switch(
                (that) => MayBe.GoodValue(3),
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }

        {
            MayBe<int, string> sut = MayBe.FailValue("1");
            var act = sut.Switch(
                (that) => MayBe.GoodValue(3),
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(MayBeMode.Fail, act.Mode);
        }

        {
            MayBe<int, string> sut = MayBe.ErrorValue(new Exception("1"));
            var act = sut.Switch(
                (that) => MayBe.GoodValue(3),
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("1", r?.Message);
        }
    }
    //

    [Fact]
    public async Task SwitchAsync_Test() {
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = await sut.SwitchAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); },
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }

        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.SwitchAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); },
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }

        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.SwitchAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); },
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }

        {
            MayBe<int, string> sut = MayBe.FailValue("1");
            var act = await sut.SwitchAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); },
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(MayBeMode.Fail, act.Mode);
        }

        {
            MayBe<int, string> sut = MayBe.ErrorValue(new Exception("1"));
            var act = await sut.SwitchAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); },
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("1", r?.Message);
        }
    }
    //

    [Fact]
    public async Task SwitchAsyncWithTaskValue_Test() {
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = await sut.ChainAsync().SwitchAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); },
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }

        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainAsync().SwitchAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); },
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }

        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainAsync().SwitchAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); },
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }

        {
            MayBe<int, string> sut = MayBe.FailValue("1");
            var act = await sut.ChainAsync().SwitchAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); },
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(MayBeMode.Fail, act.Mode);
        }

        {
            MayBe<int, string> sut = MayBe.ErrorValue(new Exception("1"));
            var act = await sut.ChainAsync().SwitchAsync(
                async (that) => { await Task.CompletedTask; return MayBe.GoodValue(3); },
                (that) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("1", r?.Message);
        }
    }
    //

    [Fact]
    public void SwitchOn_Test() {
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = sut.SwitchOn(
                2,
                (that, args) => MayBe.GoodValue(3),
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }

        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = sut.SwitchOn(
                2,
                (that, args) => MayBe.GoodValue(3),
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }

        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = sut.SwitchOn(
                2,
                (that, args) => MayBe.GoodValue(3),
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3, r);
        }

        {
            MayBe<int, string> sut = MayBe.FailValue("1");
            var act = sut.SwitchOn(
                2,
                (that, args) => MayBe.GoodValue(3),
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(MayBeMode.Fail, act.Mode);
        }

        {
            MayBe<int, string> sut = MayBe.ErrorValue(new Exception("1"));
            var act = sut.SwitchOn(
                2,
                (that, args) => MayBe.GoodValue(3),
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("1", r?.Message);
        }
    }
    //

    [Fact]
    public async Task SwitchOnAsync_Test() {
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = await sut.SwitchOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3+args); },
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3+2, r);
        }

        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.SwitchOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3+args); },
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3+2, r);
        }

        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.SwitchOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3+args); },
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3+2, r);
        }

        {
            MayBe<int, string> sut = MayBe.FailValue("1");
            var act = await sut.SwitchOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3+args); },
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(MayBeMode.Fail, act.Mode);
        }

        {
            MayBe<int, string> sut = MayBe.ErrorValue(new Exception("1"));
            var act = await sut.SwitchOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3+args); },
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("1", r?.Message);
        }
    }
    //

    [Fact]
    public async Task SwitchOnAsyncWithTaskValue_Test() {
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = await sut.ChainAsync().SwitchOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); },
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3 + 2, r);
        }

        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainAsync().SwitchOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); },
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3 + 2, r);
        }

        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = await sut.ChainAsync().SwitchOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); },
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(3 + 2, r);
        }

        {
            MayBe<int, string> sut = MayBe.FailValue("1");
            var act = await sut.ChainAsync().SwitchOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); },
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(MayBeMode.Fail, act.Mode);
        }

        {
            MayBe<int, string> sut = MayBe.ErrorValue(new Exception("1"));
            var act = await sut.ChainAsync().SwitchOnAsync(
                2,
                async (that, args) => { await Task.CompletedTask; return MayBe.GoodValue(3 + args); },
                (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetErrorValue(out var r));
            Assert.Equal("1", r?.Message);
        }
    }
    //
}
//ChainOnAsync
