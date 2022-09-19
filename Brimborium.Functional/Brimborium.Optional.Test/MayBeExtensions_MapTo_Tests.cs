#pragma warning disable xUnit2003 // Do not use equality check to test for null value
#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions

namespace Brimborium.Optional;

public class MayBeExtensions_MapTo_Tests {

    [Fact]
    public void MapToOn_NoResult_WithHandler_Test() {
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args) => args,
                onGood: (that, args) => throw new InvalidOperationException(),
                onBad: (that, args) => throw new InvalidOperationException(),
                onFail: (that, args) => throw new InvalidOperationException(),
                onError: (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(2, act);
        }
        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args) => throw new InvalidOperationException(),
                onGood: (that, args) => args,
                onBad: (that, args) => throw new InvalidOperationException(),
                onFail: (that, args) => throw new InvalidOperationException(),
                onError: (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(2, act);
        }
        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args) => throw new InvalidOperationException(),
                onGood: (that, args) => throw new InvalidOperationException(),
                onBad: (that, args) => args,
                onFail: (that, args) => throw new InvalidOperationException(),
                onError: (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(2, act);
        }

        {
            var sut = MayBe.FailValue("1").ToMayBeAddValueType<string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args) => throw new InvalidOperationException(),
                onGood: (that, args) => throw new InvalidOperationException(),
                onBad: (that, args) => throw new InvalidOperationException(),
                onFail: (that, args) => args,
                onError: (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(2, act);
        }

        {
            var sut = MayBe.ErrorValue(new Exception("2")).ToMayBe<int, string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args) => throw new InvalidOperationException(),
                onGood: (that, args) => throw new InvalidOperationException(),
                onBad: (that, args) => throw new InvalidOperationException(),
                onFail: (that, args) => throw new InvalidOperationException(),
                onError: (that, args) => args
                );
            Assert.Equal(2, act);
        }
    }
    [Fact]
    public void MapToOn_NoResult_NoHandler_Test() {
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: default,
                onGood: (that, args) => throw new InvalidOperationException(),
                onBad: (that, args) => throw new InvalidOperationException(),
                onFail: (that, args) => throw new InvalidOperationException(),
                onError: (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(3, act);
        }
        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args) => throw new InvalidOperationException(),
                onGood: default,
                onBad: (that, args) => throw new InvalidOperationException(),
                onFail: (that, args) => throw new InvalidOperationException(),
                onError: (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(3, act);
        }
        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args) => throw new InvalidOperationException(),
                onGood: (that, args) => throw new InvalidOperationException(),
                onBad: default,
                onFail: (that, args) => throw new InvalidOperationException(),
                onError: (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(3, act);
        }

        {
            var sut = MayBe.FailValue("1").ToMayBeAddValueType<string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args) => throw new InvalidOperationException(),
                onGood: (that, args) => throw new InvalidOperationException(),
                onBad: (that, args) => throw new InvalidOperationException(),
                onFail: default,
                onError: (that, args) => throw new InvalidOperationException()
                );
            Assert.Equal(3, act);
        }

        {
            var sut = MayBe.ErrorValue(new Exception("2")).ToMayBe<int, string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args) => throw new InvalidOperationException(),
                onGood: (that, args) => throw new InvalidOperationException(),
                onBad: (that, args) => throw new InvalidOperationException(),
                onFail: (that, args) => throw new InvalidOperationException(),
                onError: default
                );
            Assert.Equal(3, act);
        }
    }

    //

    [Fact]
    public void MapToOn_WithResult_WithHandler_Test() {
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args, result) => args + result,
                onGood: (that, args, result) => throw new InvalidOperationException(),
                onBad: (that, args, result) => throw new InvalidOperationException(),
                onFail: (that, args, result) => throw new InvalidOperationException(),
                onError: (that, args, result) => throw new InvalidOperationException()
                );
            Assert.Equal(2 + 3, act);
        }

        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args, result) => throw new InvalidOperationException(),
                onGood: (that, args, result) => args + result,
                onBad: (that, args, result) => throw new InvalidOperationException(),
                onFail: (that, args, result) => throw new InvalidOperationException(),
                onError: (that, args, result) => throw new InvalidOperationException()
                );
            Assert.Equal(2 + 3, act);
        }

        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args, result) => throw new InvalidOperationException(),
                onGood: (that, args, result) => throw new InvalidOperationException(),
                onBad: (that, args, result) => args + result,
                onFail: (that, args, result) => throw new InvalidOperationException(),
                onError: (that, args, result) => throw new InvalidOperationException()
                );
            Assert.Equal(2 + 3, act);
        }

        {
            var sut = MayBe.FailValue("1").ToMayBeAddValueType<string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args, result) => throw new InvalidOperationException(),
                onGood: (that, args, result) => throw new InvalidOperationException(),
                onBad: (that, args, result) => throw new InvalidOperationException(),
                onFail: (that, args, result) => args + result,
                onError: (that, args, result) => throw new InvalidOperationException()
                );
            Assert.Equal(2 + 3, act);
        }

        {
            var sut = MayBe.ErrorValue(new Exception("2")).ToMayBe<int, string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args, result) => throw new InvalidOperationException(),
                onGood: (that, args, result) => throw new InvalidOperationException(),
                onBad: (that, args, result) => throw new InvalidOperationException(),
                onFail: (that, args, result) => throw new InvalidOperationException(),
                onError: (that, args, result) => args + result
                );
            Assert.Equal(2 + 3, act);
        }
    }
    [Fact]
    public void MapToOn_WithResult_NoHandler_Test() {
        {
            var sut = MayBe.MayBeNoValue<int, string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: default,
                onGood: (that, args, result) => throw new InvalidOperationException(),
                onBad: (that, args, result) => throw new InvalidOperationException(),
                onFail: (that, args, result) => throw new InvalidOperationException(),
                onError: (that, args, result) => throw new InvalidOperationException()
                );
            Assert.Equal(3, act);
        }

        {
            var sut = MayBe.GoodValue(1).ToMayBeAddFailureType<string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args, result) => throw new InvalidOperationException(),
                onGood: default,
                onBad: (that, args, result) => throw new InvalidOperationException(),
                onFail: (that, args, result) => throw new InvalidOperationException(),
                onError: (that, args, result) => throw new InvalidOperationException()
                );
            Assert.Equal(3, act);
        }

        {
            var sut = MayBe.BadValue(1).ToMayBeAddFailureType<string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args, result) => throw new InvalidOperationException(),
                onGood: (that, args, result) => throw new InvalidOperationException(),
                onBad: default,
                onFail: (that, args, result) => throw new InvalidOperationException(),
                onError: (that, args, result) => throw new InvalidOperationException()
                );
            Assert.Equal(3, act);
        }

        {
            var sut = MayBe.FailValue("1").ToMayBeAddValueType<string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args, result) => throw new InvalidOperationException(),
                onGood: (that, args, result) => throw new InvalidOperationException(),
                onBad: (that, args, result) => throw new InvalidOperationException(),
                onFail: default,
                onError: (that, args, result) => throw new InvalidOperationException()
                );
            Assert.Equal(3, act);
        }

        {
            var sut = MayBe.ErrorValue(new Exception("2")).ToMayBe<int, string>();
            var act = sut.MapToOn(
                2,
                3,
                onNoValue: (that, args, result) => throw new InvalidOperationException(),
                onGood: (that, args, result) => throw new InvalidOperationException(),
                onBad: (that, args, result) => throw new InvalidOperationException(),
                onFail: (that, args, result) => throw new InvalidOperationException(),
                onError: default
                );
            Assert.Equal(3, act);
        }
    }

}
//ChainOnAsync
