namespace Brimborium.RowVersion.Extensions;

public class AggregationEntityVersionExtensionTests {
    private TestValues tv;

    public AggregationEntityVersionExtensionTests() {
        this.tv = new TestValues();
    }

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
            var act1 = arv1.ToCacheKey();

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

    [Fact()]
    public void ToAggregateDictionaryValidRange_Test() {
        var sut = new List<DTOTestValidRange>() {
            this.tv.dtoAB,
            this.tv.dtoBC,
            this.tv.dtoAC
        };

        {
            var act = sut.ToAggregateDictionaryValidRange(this.tv.dtoA, vr => $"{vr.ValidFrom}-{vr.ValidTo}", vr => vr);
            Assert.Equal(2, act.Dict.Count);
            Assert.Equal(12L, act.AggregationVersion.EntityVersion);
        }
    }

    [Fact()]
    public void ToAggregateDictionaryValidRangeQ_Test() {
        var sut = new List<DTOTestValidRangeQ>() {
            this.tv.dtoQAB,
            this.tv.dtoQBC,
            this.tv.dtoQAC
        };

        {
            var act = sut.ToAggregateDictionaryValidRangeQ(this.tv.dtoA, vr => $"{vr.ValidFrom}-{vr.ValidTo}", vr => vr);
            Assert.Equal(2, act.Dict.Count);
            Assert.Equal(22L, act.AggregationVersion.EntityVersion);
        }
    }

    [Fact()]
    public void ToAggregateRecordListValidRange_Test() {
        var sut = new List<DTOTestValidRange>() {
            this.tv.dtoAB,
            this.tv.dtoBC,
            this.tv.dtoAC
        };
        {
            var act = sut.ToAggregateRecordListValidRange(this.tv.dtoA);
            Assert.Equal(2, act.List.Count);
            Assert.Equal(10, act.List[0].EntityVersion);
            Assert.Equal(12, act.List[1].EntityVersion);
        }
    }

    [Fact()]
    public void ToAggregateRecordListValidRangeQ_Test() {
        var sut = new List<DTOTestValidRangeQ>() {
            this.tv.dtoQAB,
            this.tv.dtoQBC,
            this.tv.dtoQAC
        };
        {
            var act = sut.ToAggregateRecordListValidRangeQ(this.tv.dtoA);
            Assert.Equal(2, act.List.Count);
            Assert.Equal(20, act.List[0].EntityVersion);
            Assert.Equal(22, act.List[1].EntityVersion);
        }
        {
            var act = sut.ToAggregateRecordListValidRangeQ(this.tv.dtoB);
            Assert.Equal(2, act.List.Count);
            Assert.Equal(21, act.List[0].EntityVersion);
            Assert.Equal(22, act.List[1].EntityVersion);
        }
    }

    [Fact()]
    public void ToAggregateRecordListSorted_Test() {
        var sut = new List<DTOTestValidRange>() {
            this.tv.dtoAB,
            this.tv.dtoBC,
            this.tv.dtoAC
        };
        var act = sut.ToAggregateRecordListSorted((a, b) => (-a.EntityVersion).CompareTo(-b.EntityVersion));
        Assert.Equal(3, act.List.Count);

        Assert.Equal(12, act.List[0].EntityVersion);
        Assert.Equal(11, act.List[1].EntityVersion);
        Assert.Equal(10, act.List[2].EntityVersion);

        Assert.Equal(12, act.AggregatedVersion.EntityVersion);
    }

    [Fact()]
    public void AggregateRecordList_ctor_Test() {
        var sut = new AggregateRecordList<DTOTestValidRange>();
        sut.Add(this.tv.dtoAB);
        sut.Add(this.tv.dtoBC);
        sut.Add(this.tv.dtoAC);

        Assert.Equal(10, sut.List[0].EntityVersion);
        Assert.Equal(11, sut.List[1].EntityVersion);
        Assert.Equal(12, sut.List[2].EntityVersion);

        Assert.Equal(12, sut.AggregatedVersion.EntityVersion);
    }

    [Fact()]
    public void AggregateRecordList_ctor_items_Test() {
        var sut = new AggregateRecordList<DTOTestValidRange>(new DTOTestValidRange[]{
            this.tv.dtoAB,
            this.tv.dtoBC,
            this.tv.dtoAC
        }) ;

        Assert.Equal(10, sut.List[0].EntityVersion);
        Assert.Equal(11, sut.List[1].EntityVersion);
        Assert.Equal(12, sut.List[2].EntityVersion);

        Assert.Equal(12, sut.AggregatedVersion.EntityVersion);
    }



    [Fact()]
    public void AggregateRecordList_ctor_items2_Test() {
        var sut = new AggregateRecordList<DTOTestValidRange>() {
            this.tv.dtoAB,
            this.tv.dtoBC,
            this.tv.dtoAC
        };

        Assert.Equal(10, sut.List[0].EntityVersion);
        Assert.Equal(11, sut.List[1].EntityVersion);
        Assert.Equal(12, sut.List[2].EntityVersion);

        Assert.Equal(12, sut.AggregatedVersion.EntityVersion);
    }

    [Fact()]
    public void AggregateRecordList_Add_Test() {
        var sut = new AggregateRecordList<DTOTestValidRange>();
        sut.Add(this.tv.dtoAB);
        sut.Add(this.tv.dtoBC);
        sut.Add(this.tv.dtoAC);

        Assert.Equal(10, sut.List[0].EntityVersion);
        Assert.Equal(11, sut.List[1].EntityVersion);
        Assert.Equal(12, sut.List[2].EntityVersion);

        Assert.Equal(12, sut.AggregatedVersion.EntityVersion);
    }


    [Fact()]
    public void AggregateRecordList_Add_func_Test() {
        var sut = new AggregateRecordList<DTOTestValidRange>();

        sut.Add(this.tv.dtoAB, CompareEntityVersion);
        sut.Add(this.tv.dtoAC, CompareEntityVersion);
        sut.Add(this.tv.dtoBC, CompareEntityVersion);

        Assert.Equal(10, sut.List[0].EntityVersion);
        Assert.Equal(11, sut.List[1].EntityVersion);
        Assert.Equal(12, sut.List[2].EntityVersion);

        Assert.Equal(12, sut.AggregatedVersion.EntityVersion);
    }

    private static int CompareEntityVersion(DTOTestValidRange a, DTOTestValidRange b) {
        return a.EntityVersion.CompareTo(b.EntityVersion);
    }
}
