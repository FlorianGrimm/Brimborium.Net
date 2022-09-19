#pragma warning disable xUnit2003 // Do not use equality check to test for null value
#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions

namespace Brimborium.Optional;

public class MayBe2VFTests {
    [Fact]
    public void MayBe_WithXXXTest() {
        var sut = new MayBe<int, string>();
        {
            var act = sut.WithNoValue();
            Assert.Equal(MayBeMode.NoValue, act.Mode);
            Assert.Equal(0, act.Value);
            Assert.Equal(default, act.Failure);
            Assert.Equal(default, act.Error);
            Assert.Equal(false, act.TryGetGoodValue(out var _));
            Assert.Equal(false, act.TryGetBadValue(out var _));
            Assert.Equal(false, act.TryGetFailValue(out var _));
            Assert.Equal(false, act.TryGetErrorValue(out var _));
        }
        {
            var act = sut.WithGoodValue(1);
            Assert.Equal(MayBeMode.Good, act.Mode);
            Assert.Equal(1, act.Value);
            Assert.Equal(default, act.Failure);
            Assert.Equal(default, act.Error);
            Assert.Equal(true, act.TryGetGoodValue(out var _));
            Assert.Equal(false, act.TryGetBadValue(out var _));
            Assert.Equal(false, act.TryGetFailValue(out var _));
            Assert.Equal(false, act.TryGetErrorValue(out var _));
        }
        {
            var act = sut.WithBadValue(2);
            Assert.Equal(MayBeMode.Bad, act.Mode);
            Assert.Equal(2, act.Value);
            Assert.Equal(default, act.Failure);
            Assert.Equal(default, act.Error);
            Assert.Equal(false, act.TryGetGoodValue(out var _));
            Assert.Equal(true, act.TryGetBadValue(out var _));
            Assert.Equal(false, act.TryGetFailValue(out var _));
            Assert.Equal(false, act.TryGetErrorValue(out var _));
        }
        {
            var act = sut.WithFailVallue("oh no");
            Assert.Equal(MayBeMode.Fail, act.Mode);
            Assert.Equal(default, act.Value);
            Assert.Equal("oh no", act.Failure);
            Assert.Equal(default, act.Error);
            Assert.Equal(false, act.TryGetGoodValue(out var _));
            Assert.Equal(false, act.TryGetBadValue(out var _));
            Assert.Equal(true, act.TryGetFailValue(out var _));
            Assert.Equal(false, act.TryGetErrorValue(out var _));
        }
        {
            var act = sut.WithError(new Exception("OK"));
            Assert.Equal(MayBeMode.Error, act.Mode);
            Assert.Equal(0, act.Value);
            Assert.Equal(null, act.Failure);
            Assert.Equal("OK", act.Error?.Message);
            Assert.Equal(false, act.TryGetGoodValue(out var _));
            Assert.Equal(false, act.TryGetBadValue(out var _));
            Assert.Equal(false, act.TryGetFailValue(out var _));
            Assert.Equal(true, act.TryGetErrorValue(out var _));
        }
    }

}
