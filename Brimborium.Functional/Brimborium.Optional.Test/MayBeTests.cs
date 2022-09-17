#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions

namespace Brimborium.Optional;

public class MayBeTests {
    [Fact()]
    public void MayBeStaticMayValueTest() {
        var (a,b)=MayBe.MayValue(true, 2);
        Assert.Equal(true, a);
        Assert.Equal(2,b);
    }

    [Fact()]
    public void MayBeStaticSuccessfullValueTest() {
        var (a, b) = MayBe.SuccessfullValue( 2);
        Assert.Equal(true, a);
        Assert.Equal(2, b);
    }

    [Fact()]
    public void MayBeStaticUndecidedValueTest() {
        var (a, b) = MayBe.UndecidedValue<int>( 2);
        Assert.Equal(false, a);
        Assert.Equal(2, b);

    }

    [Fact()]
    public void MayBeStaticNoValueTest() {
        var sut = MayBe.NoValue<int>();
        Assert.NotNull(sut);
        Assert.True(sut is MayBe<int>);

    }

    [Fact()]
    public void MayBeStaticFailErrorTest() {
        var sut = MayBe.FailError(42);
        Assert.Equal(42, sut.Error);
    }
}