namespace Brimborium.RowVersion.Entity;
public class AggregateDictionaryTests {
    [Fact()]
    public void ToAggregateDictionary_Test() {
        var inputNbr = new List<int>();
        for (int idx = 1; idx <= 42; idx += 2) {
            inputNbr.Add(idx);
        }
        var input = new Dictionary<int, TestEntityWithVersion>();
        foreach(var idx in inputNbr) { 
            input.Add(idx, new TestEntityWithVersion(idx));
        }
        var act = input.ToAggregateDictionary();
        Assert.Equal(input.Last().Value.EntityVersion, act.EntityVersion.EntityVersion);
        Assert.Equal(input.Count, act.EntityVersion.CountVersion);
    }

    [Fact()]
    public void ToAggregateDictionary_Test1() {
        var inputNbr = new List<int>();
        for (int idx = 1; idx <= 42; idx += 2) {
            inputNbr.Add(idx);
        }
        var input = new List<TestEntityWithVersion>();
        foreach (var idx in inputNbr) {
            input.Add(new TestEntityWithVersion(idx));
        }
        var act = input.ToAggregateDictionary(getKey: (i)=>i.EntityVersion, (i)=>i);
        Assert.Equal(input.Last().EntityVersion, act.EntityVersion.EntityVersion);
        Assert.Equal(input.Count, act.EntityVersion.CountVersion);
    }

    /*
    [Fact()]
    public void ToAggregateDictionaryValidRange_Test() {
#warning Assert.True(false, "This test needs an implementation");
    }

    [Fact()]
    public void ToAggregateDictionaryValidRangeQ_Test() {
#warning Assert.True(false, "This test needs an implementation");
    }
    */
}
