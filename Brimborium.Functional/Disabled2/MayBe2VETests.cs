#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions

namespace Brimborium.Optional;
public class MayBe2VETests {
    [Fact()]
    public void TryGetValueTest() {
        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            Assert.Equal(true, sut.TryGetValue(out var r));
            Assert.Equal(1, r);
        }
        {
            var sut = new MayBeValue<int, Exception>(false, 1);
            Assert.Equal(true, sut.TryGetValue(out var r));
            Assert.Equal(1, r);
        }
        {
            var sut = new MayBeNoValue<int, Exception>();
            Assert.Equal(false, sut.TryGetValue(out var _));
        }
        {
            var sut = new MayBeFail<int, Exception>(new Exception("error"));
            Assert.Equal(false, sut.TryGetValue(out var _));
        }
    }

    [Fact()]
    public void TryGetSuccessfullValueTest() {
        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            Assert.Equal(true, sut.TryGetSuccessfullValue(out var r));
            Assert.Equal(1, r);
        }
        {
            var sut = new MayBeValue<int, Exception>(false, 1);
            Assert.Equal(false, sut.TryGetSuccessfullValue(out var _));
        }
        {
            var sut = new MayBeNoValue<int, Exception>();
            Assert.Equal(false, sut.TryGetSuccessfullValue(out var _));
        }
        {
            var sut = new MayBeFail<int, Exception>(new Exception("error"));
            Assert.Equal(false, sut.TryGetSuccessfullValue(out var _));
        }

    }

    [Fact()]
    public void TryGetFailureValueTest() {
        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            Assert.Equal(true, sut.Success);
            Assert.Equal(false, sut.Fail);
            Assert.Equal(false, sut.TryGetFailureValue(out var _));
        }
        {
            var sut = new MayBeValue<int, Exception>(false, 1);
            Assert.Equal(false, sut.Success);
            Assert.Equal(false, sut.Fail);
            Assert.Equal(false, sut.TryGetFailureValue(out var _));
        }
        {
            var sut = new MayBeNoValue<int, Exception>();
            Assert.Equal(false, sut.Success);
            Assert.Equal(false, sut.Fail);
            Assert.Equal(false, sut.TryGetFailureValue(out var _));
        }
        {
            var sut = new MayBeFail<int, Exception>(new Exception("error"));
            Assert.Equal(false, sut.Success);
            Assert.Equal(true, sut.Fail);
            Assert.Equal(true, sut.TryGetFailureValue(out var failure));
            Assert.Equal("error", failure?.Message);
        }
    }

    [Fact()]
    public void WithSuccessfullValueTest() {
        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.WithSuccessfullValue(2);
            Assert.Equal(false, act.Fail);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(2, r);
        }
        {
            var sut = new MayBeValue<int, Exception>(false, 1);
            var act = sut.WithSuccessfullValue(2);
            Assert.Equal(false, act.Fail);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(2, r);
        }
        {
            var sut = new MayBeNoValue<int, Exception>();
            var act = sut.WithSuccessfullValue(2);
            Assert.Equal(false, act.Fail);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(2, r);
        }
        {
            var sut = new MayBeFail<int, Exception>(new Exception("error"));
            var act = sut.WithSuccessfullValue(2);
            Assert.Equal(false, act.Fail);
            Assert.Equal(true, act.TryGetSuccessfullValue(out var r));
            Assert.Equal(2, r);
        }
    }

    [Fact()]
    public void WithUndecidedValueTest() {
        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.WithUndecidedValue(2);
            Assert.Equal(false, act.Success);
            Assert.Equal(false, act.Fail);
            Assert.Equal(true, act.TryGetValue(out var r));
            Assert.Equal(2, r);
        }
        {
            var sut = new MayBeValue<int, Exception>(false, 1);
            var act = sut.WithUndecidedValue(2);
            Assert.Equal(false, act.Success);
            Assert.Equal(false, act.Fail);
            Assert.Equal(true, act.TryGetValue(out var r));
            Assert.Equal(2, r);
        }
        {
            var sut = new MayBeNoValue<int, Exception>();
            var act = sut.WithUndecidedValue(2);
            Assert.Equal(false, act.Success);
            Assert.Equal(false, act.Fail);
            Assert.Equal(true, act.TryGetValue(out var r));
            Assert.Equal(2, r);
        }
        {
            var sut = new MayBeFail<int, Exception>(new Exception("error"));
            var act = sut.WithUndecidedValue(2);
            Assert.Equal(false, act.Success);
            Assert.Equal(false, act.Fail);
            Assert.Equal(true, act.TryGetValue(out var r));
            Assert.Equal(2, r);
        }
    }

    [Fact()]
    public void WithFailureTest() {
        {
            var sut = new MayBeValue<int, Exception>(true, 1);
            var act = sut.WithFailure(new Exception("error"));
            Assert.Equal(false, act.Success);
            Assert.Equal(true, act.Fail);
            Assert.Equal(false, act.TryGetValue(out var _));
            Assert.Equal(true, act.TryGetFailureValue(out var r));
            Assert.Equal("error", r?.Message);
        }
        {
            var sut = new MayBeValue<int, Exception>(false, 1);
            var act = sut.WithFailure(new Exception("error"));
            Assert.Equal(false, act.Success);
            Assert.Equal(true, act.Fail);
            Assert.Equal(false, act.TryGetValue(out var _));
            Assert.Equal(true, act.TryGetFailureValue(out var r));
            Assert.Equal("error", r?.Message);
        }
        {
            var sut = new MayBeNoValue<int, Exception>();
            var act = sut.WithFailure(new Exception("error"));
            Assert.Equal(false, act.Success);
            Assert.Equal(true, act.Fail);
            Assert.Equal(false, act.TryGetValue(out var _));
            Assert.Equal(true, act.TryGetFailureValue(out var r));
            Assert.Equal("error", r?.Message);
        }
        {
            var sut = new MayBeFail<int, Exception>(new Exception("error"));
            var act = sut.WithFailure(new Exception("error"));
            Assert.Equal(false, act.Success);
            Assert.Equal(true, act.Fail);
            Assert.Equal(false, act.TryGetValue(out var _));
            Assert.Equal(true, act.TryGetFailureValue(out var r));
            Assert.Equal("error", r?.Message);
        }
    }

    [Fact()]
    public void MapTest() {

    }

    [Fact()]
    public void MapToTest() {

    }

    [Fact()]
    public void GetSuccessOrFailOrDefaultTest() {

    }
}
