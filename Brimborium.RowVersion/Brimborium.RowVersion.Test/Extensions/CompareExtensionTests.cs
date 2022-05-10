namespace Brimborium.RowVersion.Extensions;
public class CompareExtensionTests {
    private DateTimeOffset dtoA;
    private DateTimeOffset dtoB;
    private DateTimeOffset dtoC;

    private DTOTestValidRange dtoAB;
    private DTOTestValidRange dtoBC;
    private DTOTestValidRange dtoAC;

    private DTOTestValidRangeQ dtoQAB;
    private DTOTestValidRangeQ dtoQBC;
    private DTOTestValidRangeQ dtoQAC;

    private DTOTestValidRangeQ dtoQAN;
    private DTOTestValidRangeQ dtoQBN;
    private DTOTestValidRangeQ dtoQCN;

    private DateTime dtA;
    private DateTime dtB;
    private DateTime dtC;

    private DTTestValidRange dtAB;
    private DTTestValidRange dtBC;
    private DTTestValidRange dtAC;

    private DTTestValidRangeQ dtQAB;
    private DTTestValidRangeQ dtQBC;
    private DTTestValidRangeQ dtQAC;

    private DTTestValidRangeQ dtQAN;
    private DTTestValidRangeQ dtQBN;
    private DTTestValidRangeQ dtQCN;

    public CompareExtensionTests() {
        this.dtoA = new DateTimeOffset(new DateTime(2000, 1, 1), TimeSpan.Zero);
        this.dtoB = new DateTimeOffset(new DateTime(2000, 1, 2), TimeSpan.Zero);
        this.dtoC = new DateTimeOffset(new DateTime(2000, 1, 3), TimeSpan.Zero);

        this.dtoAB = new DTOTestValidRange(this.dtoA, this.dtoB);
        this.dtoBC = new DTOTestValidRange(this.dtoB, this.dtoC);
        this.dtoAC = new DTOTestValidRange(this.dtoA, this.dtoC);

        this.dtoQAB = new DTOTestValidRangeQ(this.dtoA, this.dtoB);
        this.dtoQBC = new DTOTestValidRangeQ(this.dtoB, this.dtoC);
        this.dtoQAC = new DTOTestValidRangeQ(this.dtoA, this.dtoC);

        this.dtoQAN = new DTOTestValidRangeQ(this.dtoA, null);
        this.dtoQBN = new DTOTestValidRangeQ(this.dtoB, null);
        this.dtoQCN = new DTOTestValidRangeQ(this.dtoC, null);

        this.dtA = new DateTime(2000, 1, 1);
        this.dtB = new DateTime(2000, 1, 2);
        this.dtC = new DateTime(2000, 1, 3);

        this.dtAB = new DTTestValidRange(this.dtA, this.dtB);
        this.dtBC = new DTTestValidRange(this.dtB, this.dtC);
        this.dtAC = new DTTestValidRange(this.dtA, this.dtC);

        this.dtQAB = new DTTestValidRangeQ(this.dtA, this.dtB);
        this.dtQBC = new DTTestValidRangeQ(this.dtB, this.dtC);
        this.dtQAC = new DTTestValidRangeQ(this.dtA, this.dtC);

        this.dtQAN = new DTTestValidRangeQ(this.dtA, null);
        this.dtQBN = new DTTestValidRangeQ(this.dtB, null);
        this.dtQCN = new DTTestValidRangeQ(this.dtC, null);
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
        Assert.True(0 == CompareExtension.CompareDTValidRange(this.dtAB, dtAB));
        Assert.True(CompareExtension.CompareDTValidRange(this.dtAB, dtAC) < 0);
        Assert.True(CompareExtension.CompareDTValidRange(this.dtAB, dtBC) < 0);
    }

    [Fact()]
    public void CompareValidRangeQ_Test() {
        Assert.True(0 == CompareExtension.CompareDTValidRangeQ(this.dtQAB, dtQAB));
        Assert.True(CompareExtension.CompareDTValidRangeQ(this.dtQAB, dtQAC) < 0);
        Assert.True(CompareExtension.CompareDTValidRangeQ(this.dtQAB, dtQAN) < 0);
        Assert.True(CompareExtension.CompareDTValidRangeQ(this.dtQAB, dtQBN) < 0);
        Assert.True(CompareExtension.CompareDTValidRangeQ(this.dtQAB, dtQCN) < 0);
    }
}
