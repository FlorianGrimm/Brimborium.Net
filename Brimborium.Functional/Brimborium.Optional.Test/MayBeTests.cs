#pragma warning disable xUnit2003 // Do not use equality check to test for null value
#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions
namespace Brimborium.Optional;

public class MayBeTests {
    [Fact]
    public void MayBeTest() {
        {
            var act = MayBe.MayBeNoValue<int, string>();
            Assert.Equal(MayBeMode.NoValue, act.Mode);
            Assert.Equal(default, act.Value);
            Assert.Equal(default, act.Failure);
            Assert.Equal(default, act.Error);
        }
        {
            var act = MayBe.MayBeGood<int, string>(1);
            Assert.Equal(MayBeMode.Good, act.Mode);
            Assert.Equal(1, act.Value);
            Assert.Equal(default, act.Failure);
            Assert.Equal(default, act.Error);
        }
        {
            var act = MayBe.MayBeBad<int, string>(2);
            Assert.Equal(MayBeMode.Bad, act.Mode);
            Assert.Equal(2, act.Value);
            Assert.Equal(default, act.Failure);
            Assert.Equal(default, act.Error);
        }

        {
            var act = MayBe.MayBeFail<int, string>("3");
            Assert.Equal(MayBeMode.Fail, act.Mode);
            Assert.Equal(default, act.Value);
            Assert.Equal("3", act.Failure);
            Assert.Equal(default, act.Error);
        }
        {
            var act = MayBe.MayBeError<int, string>(new Exception("4"));
            Assert.Equal(MayBeMode.Error, act.Mode);
            Assert.Equal(default, act.Value);
            Assert.Equal(default, act.Failure);
            Assert.Equal("4", act.Error?.Message);
        }


    }
}
