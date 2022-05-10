#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions

namespace Brimborium.RowVersion.Extensions;

public class ValidRangeExtensionTests {
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

    public ValidRangeExtensionTests() {
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

    [Fact()]
    public void WithinValidRange_Test() {
        {
            Assert.Equal(true, this.dtAB.WithinValidRange(this.dtA));
            Assert.Equal(false, this.dtAB.WithinValidRange(this.dtB));
            Assert.Equal(false, this.dtAB.WithinValidRange(this.dtC));

            Assert.Equal(false, this.dtBC.WithinValidRange(this.dtA));
            Assert.Equal(true, this.dtBC.WithinValidRange(this.dtB));
            Assert.Equal(false, this.dtBC.WithinValidRange(this.dtC));

            Assert.Equal(true, this.dtAC.WithinValidRange(this.dtA));
            Assert.Equal(true, this.dtAC.WithinValidRange(this.dtB));
            Assert.Equal(false, this.dtAC.WithinValidRange(this.dtC));
        }
        {
            Assert.Equal(true, this.dtoAB.WithinValidRange(this.dtoA));
            Assert.Equal(false, this.dtoAB.WithinValidRange(this.dtoB));
            Assert.Equal(false, this.dtoAB.WithinValidRange(this.dtoC));

            Assert.Equal(false, this.dtoBC.WithinValidRange(this.dtoA));
            Assert.Equal(true, this.dtoBC.WithinValidRange(this.dtoB));
            Assert.Equal(false, this.dtoBC.WithinValidRange(this.dtoC));

            Assert.Equal(true, this.dtoAC.WithinValidRange(this.dtoA));
            Assert.Equal(true, this.dtoAC.WithinValidRange(this.dtoB));
            Assert.Equal(false, this.dtoAC.WithinValidRange(this.dtoC));
        }
    }

    [Fact()]
    public void WithinValidRangeQ_Test() {
        {
            Assert.Equal(true, this.dtQAB.WithinValidRangeQ(this.dtA));
            Assert.Equal(false, this.dtQAB.WithinValidRangeQ(this.dtB));
            Assert.Equal(false, this.dtQAB.WithinValidRangeQ(this.dtC));

            Assert.Equal(false, this.dtQBC.WithinValidRangeQ(this.dtA));
            Assert.Equal(true, this.dtQBC.WithinValidRangeQ(this.dtB));
            Assert.Equal(false, this.dtQBC.WithinValidRangeQ(this.dtC));

            Assert.Equal(true, this.dtQAC.WithinValidRangeQ(this.dtA));
            Assert.Equal(true, this.dtQAC.WithinValidRangeQ(this.dtB));
            Assert.Equal(false, this.dtQAC.WithinValidRangeQ(this.dtC));
        }
        {
            Assert.Equal(true, this.dtoQAB.WithinValidRangeQ(this.dtoA));
            Assert.Equal(false, this.dtoQAB.WithinValidRangeQ(this.dtoB));
            Assert.Equal(false, this.dtoQAB.WithinValidRangeQ(this.dtoC));

            Assert.Equal(false, this.dtoQBC.WithinValidRangeQ(this.dtoA));
            Assert.Equal(true, this.dtoQBC.WithinValidRangeQ(this.dtoB));
            Assert.Equal(false, this.dtoQBC.WithinValidRangeQ(this.dtoC));

            Assert.Equal(true, this.dtoQAC.WithinValidRangeQ(this.dtoA));
            Assert.Equal(true, this.dtoQAC.WithinValidRangeQ(this.dtoB));
            Assert.Equal(false, this.dtoQAC.WithinValidRangeQ(this.dtoC));
        }
        {
            Assert.Equal(true, this.dtQAN.WithinValidRangeQ(this.dtA));
            Assert.Equal(true, this.dtQAN.WithinValidRangeQ(this.dtB));
            Assert.Equal(true, this.dtQAN.WithinValidRangeQ(this.dtC));

            Assert.Equal(false, this.dtQBN.WithinValidRangeQ(this.dtA));
            Assert.Equal(true, this.dtQBN.WithinValidRangeQ(this.dtB));
            Assert.Equal(true, this.dtQBN.WithinValidRangeQ(this.dtC));

            Assert.Equal(false, this.dtQCN.WithinValidRangeQ(this.dtA));
            Assert.Equal(false, this.dtQCN.WithinValidRangeQ(this.dtB));
            Assert.Equal(true, this.dtQCN.WithinValidRangeQ(this.dtC));
        }
        {
            Assert.Equal(true, this.dtoQAN.WithinValidRangeQ(this.dtoA));
            Assert.Equal(true, this.dtoQAN.WithinValidRangeQ(this.dtoB));
            Assert.Equal(true, this.dtoQAN.WithinValidRangeQ(this.dtoC));

            Assert.Equal(false, this.dtoQBN.WithinValidRangeQ(this.dtoA));
            Assert.Equal(true, this.dtoQBN.WithinValidRangeQ(this.dtoB));
            Assert.Equal(true, this.dtoQBN.WithinValidRangeQ(this.dtoC));

            Assert.Equal(false, this.dtoQCN.WithinValidRangeQ(this.dtoA));
            Assert.Equal(false, this.dtoQCN.WithinValidRangeQ(this.dtoB));
            Assert.Equal(true, this.dtoQCN.WithinValidRangeQ(this.dtoC));
        }
    }

    [Fact()]
    public void Bind_Test() {
        {
            var boundEbbes = this.dtA.Bind<int>(ebbesDT);
            Assert.Equal(true, boundEbbes(1));
            Assert.Equal(false, boundEbbes(2));
        }
        {
            var boundEbbes = this.dtoA.Bind<int>(ebbesDTO);
            Assert.Equal(true, boundEbbes(1));
            Assert.Equal(false, boundEbbes(2));
        }


        static bool ebbesDT(int day, DateTime at) {
            return (day == at.Day);
        }

        static bool ebbesDTO(int day, DateTimeOffset at) {
            return (day == at.Day);
        }
    }

    [Fact()]
    public void WhereAtValidRange_Test() {
        {
            var sut = new List<DTTestValidRange>() { this.dtAB, this.dtBC, this.dtAC };
            var actA = sut.WhereAtValidRange(this.dtA).ToList();
            var actB = sut.WhereAtValidRange(this.dtB).ToList();
            var actC = sut.WhereAtValidRange(this.dtC).ToList();

            Assert.Equal(new List<DTTestValidRange>() { this.dtAB, this.dtAC }, actA);
            Assert.Equal(new List<DTTestValidRange>() { this.dtBC, this.dtAC }, actB);
            Assert.Equal(new List<DTTestValidRange>() { }, actC);
        }
        {
            var sut = new List<DTOTestValidRange>() { this.dtoAB, this.dtoBC, this.dtoAC };
            var actA = sut.WhereAtValidRange(this.dtoA).ToList();
            var actB = sut.WhereAtValidRange(this.dtoB).ToList();
            var actC = sut.WhereAtValidRange(this.dtoC).ToList();

            Assert.Equal(new List<DTOTestValidRange>() { this.dtoAB, this.dtoAC }, actA);
            Assert.Equal(new List<DTOTestValidRange>() { this.dtoBC, this.dtoAC }, actB);
            Assert.Equal(new List<DTOTestValidRange>() { }, actC);
        }
    }

    [Fact()]
    public void WhereAtValidRangeQ_Test() {
        {
            var sut = new List<DTTestValidRangeQ>() { this.dtQAB, this.dtQBC, this.dtQAC };
            var actA = sut.WhereAtValidRangeQ(this.dtA).ToList();
            var actB = sut.WhereAtValidRangeQ(this.dtB).ToList();
            var actC = sut.WhereAtValidRangeQ(this.dtC).ToList();

            Assert.Equal(new List<DTTestValidRangeQ>() { this.dtQAB, this.dtQAC }, actA);
            Assert.Equal(new List<DTTestValidRangeQ>() { this.dtQBC, this.dtQAC }, actB);
            Assert.Equal(new List<DTTestValidRangeQ>() { }, actC);
        }
        {
            var sut = new List<DTTestValidRangeQ>() { this.dtQAN, this.dtQBN, this.dtQCN };
            var actA = sut.WhereAtValidRangeQ(this.dtA).ToList();
            var actB = sut.WhereAtValidRangeQ(this.dtB).ToList();
            var actC = sut.WhereAtValidRangeQ(this.dtC).ToList();

            Assert.Equal(new List<DTTestValidRangeQ>() { this.dtQAN }, actA);
            Assert.Equal(new List<DTTestValidRangeQ>() { this.dtQAN, this.dtQBN }, actB);
            Assert.Equal(new List<DTTestValidRangeQ>() { this.dtQAN, this.dtQBN, this.dtQCN }, actC);
        }
        {
            var sut = new List<DTOTestValidRangeQ>() { this.dtoQAB, this.dtoQBC, this.dtoQAC };
            var actA = sut.WhereAtValidRangeQ(this.dtoA).ToList();
            var actB = sut.WhereAtValidRangeQ(this.dtoB).ToList();
            var actC = sut.WhereAtValidRangeQ(this.dtoC).ToList();

            Assert.Equal(new List<DTOTestValidRangeQ>() { this.dtoQAB, this.dtoQAC }, actA);
            Assert.Equal(new List<DTOTestValidRangeQ>() { this.dtoQBC, this.dtoQAC }, actB);
            Assert.Equal(new List<DTOTestValidRangeQ>() { }, actC);
        }
        {
            var sut = new List<DTOTestValidRangeQ>() { this.dtoQAN, this.dtoQBN, this.dtoQCN };
            var actA = sut.WhereAtValidRangeQ(this.dtoA).ToList();
            var actB = sut.WhereAtValidRangeQ(this.dtoB).ToList();
            var actC = sut.WhereAtValidRangeQ(this.dtoC).ToList();

            Assert.Equal(new List<DTOTestValidRangeQ>() { this.dtoQAN }, actA);
            Assert.Equal(new List<DTOTestValidRangeQ>() { this.dtoQAN, this.dtoQBN }, actB);
            Assert.Equal(new List<DTOTestValidRangeQ>() { this.dtoQAN, this.dtoQBN, this.dtoQCN }, actC);
        }
    }
}
