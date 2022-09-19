#pragma warning disable xUnit2003 // Do not use equality check to test for null value
#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions

namespace Brimborium.Optional;

public class MayBeExtensions_Map_Tests {
    [Fact]
    public void MapTest() {
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = sut.Map(
                onNoValue: (that) => MayBe.GoodValue(2).ToMayBeAddFailureType<string>(),
                onGood: (that) => throw new InvalidOperationException(),
                onBad: (that) => throw new InvalidOperationException(),
                onFail: (that) => throw new InvalidOperationException(),
                onError: (that) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(2, r);
        }
        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = sut.Map(
                onNoValue: (that) => throw new InvalidOperationException(),
                onGood: (that) => MayBe.GoodValue(2).ToMayBeAddFailureType<string>(),
                onBad: (that) => throw new InvalidOperationException(),
                onFail: (that) => throw new InvalidOperationException(),
                onError: (that) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(2, r);
        }
        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = sut.Map(
                onNoValue: (that) => throw new InvalidOperationException(),
                onGood: (that) => throw new InvalidOperationException(),
                onBad: (that) => MayBe.GoodValue(2).ToMayBeAddFailureType<string>(),
                onFail: (that) => throw new InvalidOperationException(),
                onError: (that) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(2, r);
        }

        {
            var sut = MayBe.FailValue("1").ToMayBeAddValueType<string>();
            var act = sut.Map(
                onNoValue: (that) => throw new InvalidOperationException(),
                onGood: (that) => throw new InvalidOperationException(),
                onBad: (that) => throw new InvalidOperationException(),
                onFail: (that) => MayBe.GoodValue(2).ToMayBeAddFailureType<string>(),
                onError: (that) => throw new InvalidOperationException()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(2, r);
        }

        {
            var sut = MayBe.ErrorValue(new Exception("2")).ToMayBe<int, string>();
            var act = sut.Map(
                onNoValue: (that) => throw new InvalidOperationException(),
                onGood: (that) => throw new InvalidOperationException(),
                onBad: (that) => throw new InvalidOperationException(),
                onFail: (that) => throw new InvalidOperationException(),
                onError: (that) => MayBe.GoodValue(2).ToMayBeAddFailureType<string>()
                );
            Assert.Equal(true, act.TryGetGoodValue(out var r));
            Assert.Equal(2, r);
        }
    }
}
//ChainOnAsync
