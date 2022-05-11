
namespace Brimborium.RowVersion.Extensions;

public class TestValues {
    public DateTimeOffset dtoA;
    public DateTimeOffset dtoB;
    public DateTimeOffset dtoC;

    public DTOTestValidRange dtoAB;
    public DTOTestValidRange dtoBC;
    public DTOTestValidRange dtoAC;

    public DTOTestValidRangeQ dtoQAB;
    public DTOTestValidRangeQ dtoQBC;
    public DTOTestValidRangeQ dtoQAC;

    public DTOTestValidRangeQ dtoQAN;
    public DTOTestValidRangeQ dtoQBN;
    public DTOTestValidRangeQ dtoQCN;

    public DateTime dtA;
    public DateTime dtB;
    public DateTime dtC;

    public DTTestValidRange dtAB;
    public DTTestValidRange dtBC;
    public DTTestValidRange dtAC;

    public DTTestValidRangeQ dtQAB;
    public DTTestValidRangeQ dtQBC;
    public DTTestValidRangeQ dtQAC;

    public DTTestValidRangeQ dtQAN;
    public DTTestValidRangeQ dtQBN;
    public DTTestValidRangeQ dtQCN;

    public TestValues() {
        this.dtoA = new DateTimeOffset(new DateTime(2000, 1, 1), TimeSpan.Zero);
        this.dtoB = new DateTimeOffset(new DateTime(2000, 1, 2), TimeSpan.Zero);
        this.dtoC = new DateTimeOffset(new DateTime(2000, 1, 3), TimeSpan.Zero);

        this.dtoAB = new DTOTestValidRange(this.dtoA, this.dtoB, 10);
        this.dtoBC = new DTOTestValidRange(this.dtoB, this.dtoC, 11);
        this.dtoAC = new DTOTestValidRange(this.dtoA, this.dtoC, 12);

        this.dtoQAB = new DTOTestValidRangeQ(this.dtoA, this.dtoB, 20);
        this.dtoQBC = new DTOTestValidRangeQ(this.dtoB, this.dtoC, 21);
        this.dtoQAC = new DTOTestValidRangeQ(this.dtoA, this.dtoC, 22);

        this.dtoQAN = new DTOTestValidRangeQ(this.dtoA, null, 30);
        this.dtoQBN = new DTOTestValidRangeQ(this.dtoB, null, 31);
        this.dtoQCN = new DTOTestValidRangeQ(this.dtoC, null, 32);

        this.dtA = new DateTime(2000, 1, 1);
        this.dtB = new DateTime(2000, 1, 2);
        this.dtC = new DateTime(2000, 1, 3);

        this.dtAB = new DTTestValidRange(this.dtA, this.dtB, 40);
        this.dtBC = new DTTestValidRange(this.dtB, this.dtC, 41);
        this.dtAC = new DTTestValidRange(this.dtA, this.dtC, 42);

        this.dtQAB = new DTTestValidRangeQ(this.dtA, this.dtB, 50);
        this.dtQBC = new DTTestValidRangeQ(this.dtB, this.dtC, 51);
        this.dtQAC = new DTTestValidRangeQ(this.dtA, this.dtC, 52);

        this.dtQAN = new DTTestValidRangeQ(this.dtA, null, 60);
        this.dtQBN = new DTTestValidRangeQ(this.dtB, null, 61);
        this.dtQCN = new DTTestValidRangeQ(this.dtC, null, 62);
    }
}
