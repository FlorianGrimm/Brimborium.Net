#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions

namespace Brimborium.Functional;

public class IEnumerableExtensionTests {
    [Fact]
    public void WhereP_Test() {
        var sut = System.Linq.Enumerable.Range(0, 100).ToList();
        var p = (lo: 10, hi: 20);
        var act = sut.WhereP(p, (v, ab) => ab.lo <= v && v <= ab.hi).ToList();
        Assert.Equal(10, act.First());
        Assert.Equal(20, act.Last());
        Assert.Equal(true, act.Zip(System.Linq.Enumerable.Range(10, 10)).All((ab) => (ab.First == ab.Second)));
    }

    [Fact]
    public void WhereNP_Test() {
        var sut = new string?[] { "1", "11", null, "21" };
        var p = "11";
        var act = sut.WhereNP(p, (v, p) => v == p).ToList();
        Assert.Equal(new string[] { p }, act);
    }

    [Fact]
    public void WhereSelect_Test() {
        var sut = System.Linq.Enumerable.Range(0, 100).ToList();
        var act = sut.WhereSelect((v) => (10 == v || v == 20) ? $"#{v}" : null).ToList();
        Assert.Equal(new string[] { "#10", "#20" }, act);
    }

    [Fact]
    public void WhereSelectN_Test() {
        var sut = System.Linq.Enumerable.Range(0, 100).Select(v => v.ToString()).Concat(new string?[] { null, null}).ToList();
        var act = sut.WhereSelectN((v) => ("10" == v || v == "20") ? $"#{v}" : null).ToList();
        Assert.Equal(new string[] { "#10", "#20" }, act);
    }

    [Fact]
    public void WhereSelectP_Test() {
        var sut = System.Linq.Enumerable.Range(0, 100).ToList();
        var p = (a: 10, b: 20);
        var act = sut.WhereSelectP(p, (v, ab) => (ab.a == v || v == ab.b) ? $"#{v}" : null).ToList();
        Assert.Equal(new string[] { "#10", "#20" }, act);
    }

    [Fact]
    public void WhereSelectNP_Test() {
        var sut = System.Linq.Enumerable.Range(0, 100).Select(v => v.ToString()).Concat(new string?[] { null, null }).ToList();
        var p = (a: "10", b: "20");
        var act = sut.WhereSelectNP(p, (v, ab) => (ab.a == v || v == ab.b) ? $"#{v}" : null).ToList();
        Assert.Equal(new string[] { "#10", "#20" }, act);
    }

    [Fact]
    public void SelectMore_Test() {
        var sut = new string?[] { "1", "11", null, "21" };
        var p = "11";
        var act = sut.SelectMore((v, l) => {
            if (v == p) {
                l.Add("found1");
                l.Add("found2");
            }
        }).ToList();
        Assert.Equal(new string[] { "found1", "found2" }, act);
    }

    [Fact]
    public void SelectMoreP_Test() {
        var sut = new string?[] { "1", "11", null, "21" };
        var p = "11";
        var act = sut.SelectMoreP(p, (v, p, l) => {
            if (v == p) {
                l.Add("found1");
                l.Add("found2");
            }
        }).ToList();
        Assert.Equal(new string[] { "found1", "found2" }, act);
    }
}