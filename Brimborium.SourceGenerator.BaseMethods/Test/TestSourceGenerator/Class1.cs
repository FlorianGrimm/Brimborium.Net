namespace TestSourceGenerator {

    public partial record Test2(
        int A,
        [property: IgnoreEquality] int B,
        [property: IgnoreEquality] int C = 1
        );

    public class UnitTest2 {
        [Fact]
        public void Test1() {
            var a = new Test2(1, 1);
            var b = new Test2(1, 2);
            var c = new Test2(2, 1);
            var d = new Test2(2, 2);
            Assert.Equal(a, b);
            Assert.Equal(c, d);
            Assert.NotEqual(a, c);
            Assert.NotEqual(b, d);
            var x = a.Equals(b);
            Assert.True(x);
        }
    }
}