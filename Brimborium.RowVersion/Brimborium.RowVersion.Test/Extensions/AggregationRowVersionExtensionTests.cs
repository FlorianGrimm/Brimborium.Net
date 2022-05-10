namespace Brimborium.RowVersion.Extensions;

public class AggregationEntityVersionExtensionTests {
    [Fact()]
    public void ToAggregationEntityVersion_Test() {
        {
            var sut = new List<TestEntityWithVersion>() { };
            var act = sut.ToAggregationEntityVersion();
            Assert.Equal(0, act.EntityVersion);
            Assert.Equal(0, act.CountVersion);
        }
        {
            var sut = new List<TestEntityWithVersion>() { new TestEntityWithVersion(1), new TestEntityWithVersion(3), new TestEntityWithVersion(2), new TestEntityWithVersion(2) };
            var act = sut.ToAggregationEntityVersion();
            Assert.Equal(3, act.EntityVersion);
            Assert.Equal(4, act.CountVersion);
        }
    }

    [Fact()]
    public void CalculateArguments_Test() {
        var a = AggregationEntityVersionExtension.CalculateArguments("A");
        var b = AggregationEntityVersionExtension.CalculateArguments("A");
        var c = AggregationEntityVersionExtension.CalculateArguments("B");
        Assert.True(a == b);
        Assert.True(a != c);
    }

    [Fact()]
    public void ToCacheKey_Test() {
        {
            var sut1 = new List<TestEntityWithVersion>() { new TestEntityWithVersion(1), new TestEntityWithVersion(3), new TestEntityWithVersion(2), new TestEntityWithVersion(2) };
            var arv1 = sut1.ToAggregationEntityVersion();
            var act1= arv1.ToCacheKey();
         
            var sut2 = new List<TestEntityWithVersion>() { new TestEntityWithVersion(1), new TestEntityWithVersion(3), new TestEntityWithVersion(2), new TestEntityWithVersion(2) };
            var arv2 = sut2.ToAggregationEntityVersion();
            var act2 = arv2.ToCacheKey();

            var sut3 = new List<TestEntityWithVersion>() { new TestEntityWithVersion(1) };
            var arv3 = sut3.ToAggregationEntityVersion();
            var act3 = arv3.ToCacheKey();

            Assert.True(act1 == act2);
            Assert.True(act1 != act3);
        }
    }


    [Fact()]
    public void MaxEntityVersion_Test() {
        long rowVersion = 0;
        (1L).MaxEntityVersion(ref rowVersion);
        (3L).MaxEntityVersion(ref rowVersion);
        (2L).MaxEntityVersion(ref rowVersion);
        Assert.Equal(3L, rowVersion);
    }

    [Fact()]
    public void MaxEntityVersion_Test1() {
        AggregationEntityVersion aggregationEntityVersion = new AggregationEntityVersion();
        (1L).MaxEntityVersion(ref aggregationEntityVersion);
        (3L).MaxEntityVersion(ref aggregationEntityVersion);
        (2L).MaxEntityVersion(ref aggregationEntityVersion);
        (1L).MaxEntityVersion(ref aggregationEntityVersion);

        Assert.Equal(3L, aggregationEntityVersion.EntityVersion);
        Assert.Equal(4, aggregationEntityVersion.CountVersion);
    }
}
