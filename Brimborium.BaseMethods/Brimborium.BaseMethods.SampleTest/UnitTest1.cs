namespace Brimborium.BaseMethods.SampleTest;
public class UnitTest1 {
    [Fact]
    public void Test1() {
        var a = new UnitTest1Record(1, 1);
        var b = new UnitTest1Record(1, 2);
        var c = new UnitTest1Record(2, 1);
        var d = new UnitTest1Record(2, 2);
        Assert.Equal(a, b);
        Assert.Equal(c, d);
        Assert.NotEqual(a, c);
        Assert.NotEqual(b, d);
        var x = a.Equals(b);
        Assert.True(x);
    }
}

[Equatable(Explicit = false)]
public partial record UnitTest1Record(
    int A,
    [property: IgnoreEquality] int B,
    [property: IgnoreEquality] int C = 1
    );