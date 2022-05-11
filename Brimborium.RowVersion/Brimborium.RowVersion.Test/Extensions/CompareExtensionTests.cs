namespace Brimborium.RowVersion.Extensions;

public class CompareExtensionTests {
    private TestValues tv;

    public CompareExtensionTests() {
        this.tv = new TestValues();
    }

    /*
    [Fact()]
    public void ChainCompareValidRangeQ_Test() {
        CompareExtension.ChainCompareDTValidRangeQ()
#warning Assert.True(false, "This test needs an implementation");
    }
    */

    [Fact()]
    public void CompareValidRange_Test() {
        Assert.True(0 == CompareExtension.CompareDTValidRange(this.tv.dtAB, this.tv.dtAB));
        Assert.True(CompareExtension.CompareDTValidRange(this.tv.dtAB, this.tv.dtAC) < 0);
        Assert.True(CompareExtension.CompareDTValidRange(this.tv.dtAB, this.tv.dtBC) < 0);
    }

    [Fact()]
    public void CompareValidRangeQ_Test() {
        Assert.True(0 == CompareExtension.CompareDTValidRangeQ(this.tv.dtQAB, this.tv.dtQAB));
        Assert.True(CompareExtension.CompareDTValidRangeQ(this.tv.dtQAB, this.tv.dtQAC) < 0);
        Assert.True(CompareExtension.CompareDTValidRangeQ(this.tv.dtQAB, this.tv.dtQAN) < 0);
        Assert.True(CompareExtension.CompareDTValidRangeQ(this.tv.dtQAB, this.tv.dtQBN) < 0);
        Assert.True(CompareExtension.CompareDTValidRangeQ(this.tv.dtQAB, this.tv.dtQCN) < 0);
    }
}
