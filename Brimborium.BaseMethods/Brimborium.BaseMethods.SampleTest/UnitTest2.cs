namespace Brimborium.BaseMethods.SampleTest;

public class UnitTest2 {
    [Fact]
    public void Test2() {
        var a = new UnitTest2Record(1, 1);
        var b = new UnitTest2Record(1, 2);
        var c = new UnitTest2Record(2, 1);
        var d = new UnitTest2Record(2, 2);
        Assert.Equal(a, b);
        Assert.Equal(c, d);
        Assert.NotEqual(a, c);
        Assert.NotEqual(b, d);
        var x = a.Equals(b);
        Assert.True(x);
    }
}

public partial record UnitTest2Record(
    int A,
    [property: IgnoreEquality] int B,
    [property: IgnoreEquality] int C = 1
    );