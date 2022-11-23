namespace TestSourceGenerator {

    [Equatable]
    public partial record TestIgnoreEquality(
        int A,
        [property: IgnoreEquality] int B,
        [property: IgnoreEquality] int C=1
        );

    public class UnitTestIgnoreEquality {
        [Fact]
        public void Test1() {
            var a = new TestIgnoreEquality(1, 1);
            var b = new TestIgnoreEquality(1, 2);
            var c = new TestIgnoreEquality(2, 1);
            var d = new TestIgnoreEquality(2, 2);
            Assert.Equal(a, b);
            Assert.Equal(c, d);
            Assert.NotEqual(a, c);
            Assert.NotEqual(b, d);
            var x = a.Equals(b);
            Assert.True(x);
        }
    }
}