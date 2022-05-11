#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions

namespace Brimborium.RowVersion.Extensions;

public class ValidRangeExtensionTests {
    private TestValues tv;

    public ValidRangeExtensionTests() {
        this.tv = new TestValues();
    }

    [Fact()]
    public void WithinValidRange_Test() {
        {
            Assert.Equal(true, this.tv.dtAB.WithinValidRange(this.tv.dtA));
            Assert.Equal(false, this.tv.dtAB.WithinValidRange(this.tv.dtB));
            Assert.Equal(false, this.tv.dtAB.WithinValidRange(this.tv.dtC));

            Assert.Equal(false, this.tv.dtBC.WithinValidRange(this.tv.dtA));
            Assert.Equal(true, this.tv.dtBC.WithinValidRange(this.tv.dtB));
            Assert.Equal(false, this.tv.dtBC.WithinValidRange(this.tv.dtC));

            Assert.Equal(true, this.tv.dtAC.WithinValidRange(this.tv.dtA));
            Assert.Equal(true, this.tv.dtAC.WithinValidRange(this.tv.dtB));
            Assert.Equal(false, this.tv.dtAC.WithinValidRange(this.tv.dtC));
        }
        {
            Assert.Equal(true, this.tv.dtoAB.WithinValidRange(this.tv.dtoA));
            Assert.Equal(false, this.tv.dtoAB.WithinValidRange(this.tv.dtoB));
            Assert.Equal(false, this.tv.dtoAB.WithinValidRange(this.tv.dtoC));

            Assert.Equal(false, this.tv.dtoBC.WithinValidRange(this.tv.dtoA));
            Assert.Equal(true, this.tv.dtoBC.WithinValidRange(this.tv.dtoB));
            Assert.Equal(false, this.tv.dtoBC.WithinValidRange(this.tv.dtoC));

            Assert.Equal(true, this.tv.dtoAC.WithinValidRange(this.tv.dtoA));
            Assert.Equal(true, this.tv.dtoAC.WithinValidRange(this.tv.dtoB));
            Assert.Equal(false, this.tv.dtoAC.WithinValidRange(this.tv.dtoC));
        }
    }

    [Fact()]
    public void WithinValidRangeQ_Test() {
        {
            Assert.Equal(true, this.tv.dtQAB.WithinValidRangeQ(this.tv.dtA));
            Assert.Equal(false, this.tv.dtQAB.WithinValidRangeQ(this.tv.dtB));
            Assert.Equal(false, this.tv.dtQAB.WithinValidRangeQ(this.tv.dtC));

            Assert.Equal(false, this.tv.dtQBC.WithinValidRangeQ(this.tv.dtA));
            Assert.Equal(true, this.tv.dtQBC.WithinValidRangeQ(this.tv.dtB));
            Assert.Equal(false, this.tv.dtQBC.WithinValidRangeQ(this.tv.dtC));

            Assert.Equal(true, this.tv.dtQAC.WithinValidRangeQ(this.tv.dtA));
            Assert.Equal(true, this.tv.dtQAC.WithinValidRangeQ(this.tv.dtB));
            Assert.Equal(false, this.tv.dtQAC.WithinValidRangeQ(this.tv.dtC));
        }
        {
            Assert.Equal(true, this.tv.dtoQAB.WithinValidRangeQ(this.tv.dtoA));
            Assert.Equal(false, this.tv.dtoQAB.WithinValidRangeQ(this.tv.dtoB));
            Assert.Equal(false, this.tv.dtoQAB.WithinValidRangeQ(this.tv.dtoC));

            Assert.Equal(false, this.tv.dtoQBC.WithinValidRangeQ(this.tv.dtoA));
            Assert.Equal(true, this.tv.dtoQBC.WithinValidRangeQ(this.tv.dtoB));
            Assert.Equal(false, this.tv.dtoQBC.WithinValidRangeQ(this.tv.dtoC));

            Assert.Equal(true, this.tv.dtoQAC.WithinValidRangeQ(this.tv.dtoA));
            Assert.Equal(true, this.tv.dtoQAC.WithinValidRangeQ(this.tv.dtoB));
            Assert.Equal(false, this.tv.dtoQAC.WithinValidRangeQ(this.tv.dtoC));
        }
        {
            Assert.Equal(true, this.tv.dtQAN.WithinValidRangeQ(this.tv.dtA));
            Assert.Equal(true, this.tv.dtQAN.WithinValidRangeQ(this.tv.dtB));
            Assert.Equal(true, this.tv.dtQAN.WithinValidRangeQ(this.tv.dtC));

            Assert.Equal(false, this.tv.dtQBN.WithinValidRangeQ(this.tv.dtA));
            Assert.Equal(true, this.tv.dtQBN.WithinValidRangeQ(this.tv.dtB));
            Assert.Equal(true, this.tv.dtQBN.WithinValidRangeQ(this.tv.dtC));

            Assert.Equal(false, this.tv.dtQCN.WithinValidRangeQ(this.tv.dtA));
            Assert.Equal(false, this.tv.dtQCN.WithinValidRangeQ(this.tv.dtB));
            Assert.Equal(true, this.tv.dtQCN.WithinValidRangeQ(this.tv.dtC));
        }
        {
            Assert.Equal(true, this.tv.dtoQAN.WithinValidRangeQ(this.tv.dtoA));
            Assert.Equal(true, this.tv.dtoQAN.WithinValidRangeQ(this.tv.dtoB));
            Assert.Equal(true, this.tv.dtoQAN.WithinValidRangeQ(this.tv.dtoC));

            Assert.Equal(false, this.tv.dtoQBN.WithinValidRangeQ(this.tv.dtoA));
            Assert.Equal(true, this.tv.dtoQBN.WithinValidRangeQ(this.tv.dtoB));
            Assert.Equal(true, this.tv.dtoQBN.WithinValidRangeQ(this.tv.dtoC));

            Assert.Equal(false, this.tv.dtoQCN.WithinValidRangeQ(this.tv.dtoA));
            Assert.Equal(false, this.tv.dtoQCN.WithinValidRangeQ(this.tv.dtoB));
            Assert.Equal(true, this.tv.dtoQCN.WithinValidRangeQ(this.tv.dtoC));
        }
    }

    [Fact()]
    public void Bind_Test() {
        {
            var boundEbbes = this.tv.dtA.Bind<int>(ebbesDT);
            Assert.Equal(true, boundEbbes(1));
            Assert.Equal(false, boundEbbes(2));
        }
        {
            var boundEbbes = this.tv.dtoA.Bind<int>(ebbesDTO);
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
            var sut = new List<DTTestValidRange>() { this.tv.dtAB, this.tv.dtBC, this.tv.dtAC };
            var actA = sut.WhereAtValidRange(this.tv.dtA).ToList();
            var actB = sut.WhereAtValidRange(this.tv.dtB).ToList();
            var actC = sut.WhereAtValidRange(this.tv.dtC).ToList();

            Assert.Equal(new List<DTTestValidRange>() { this.tv.dtAB, this.tv.dtAC }, actA);
            Assert.Equal(new List<DTTestValidRange>() { this.tv.dtBC, this.tv.dtAC }, actB);
            Assert.Equal(new List<DTTestValidRange>() { }, actC);
        }
        {
            var sut = new List<DTOTestValidRange>() { this.tv.dtoAB, this.tv.dtoBC, this.tv.dtoAC };
            var actA = sut.WhereAtValidRange(this.tv.dtoA).ToList();
            var actB = sut.WhereAtValidRange(this.tv.dtoB).ToList();
            var actC = sut.WhereAtValidRange(this.tv.dtoC).ToList();

            Assert.Equal(new List<DTOTestValidRange>() { this.tv.dtoAB, this.tv.dtoAC }, actA);
            Assert.Equal(new List<DTOTestValidRange>() { this.tv.dtoBC, this.tv.dtoAC }, actB);
            Assert.Equal(new List<DTOTestValidRange>() { }, actC);
        }
    }

    [Fact()]
    public void WhereAtValidRangeQ_Test() {
        {
            var sut = new List<DTTestValidRangeQ>() { this.tv.dtQAB, this.tv.dtQBC, this.tv.dtQAC };
            var actA = sut.WhereAtValidRangeQ(this.tv.dtA).ToList();
            var actB = sut.WhereAtValidRangeQ(this.tv.dtB).ToList();
            var actC = sut.WhereAtValidRangeQ(this.tv.dtC).ToList();

            Assert.Equal(new List<DTTestValidRangeQ>() { this.tv.dtQAB, this.tv.dtQAC }, actA);
            Assert.Equal(new List<DTTestValidRangeQ>() { this.tv.dtQBC, this.tv.dtQAC }, actB);
            Assert.Equal(new List<DTTestValidRangeQ>() { }, actC);
        }
        {
            var sut = new List<DTTestValidRangeQ>() { this.tv.dtQAN, this.tv.dtQBN, this.tv.dtQCN };
            var actA = sut.WhereAtValidRangeQ(this.tv.dtA).ToList();
            var actB = sut.WhereAtValidRangeQ(this.tv.dtB).ToList();
            var actC = sut.WhereAtValidRangeQ(this.tv.dtC).ToList();

            Assert.Equal(new List<DTTestValidRangeQ>() { this.tv.dtQAN }, actA);
            Assert.Equal(new List<DTTestValidRangeQ>() { this.tv.dtQAN, this.tv.dtQBN }, actB);
            Assert.Equal(new List<DTTestValidRangeQ>() { this.tv.dtQAN, this.tv.dtQBN, this.tv.dtQCN }, actC);
        }
        {
            var sut = new List<DTOTestValidRangeQ>() { this.tv.dtoQAB, this.tv.dtoQBC, this.tv.dtoQAC };
            var actA = sut.WhereAtValidRangeQ(this.tv.dtoA).ToList();
            var actB = sut.WhereAtValidRangeQ(this.tv.dtoB).ToList();
            var actC = sut.WhereAtValidRangeQ(this.tv.dtoC).ToList();

            Assert.Equal(new List<DTOTestValidRangeQ>() { this.tv.dtoQAB, this.tv.dtoQAC }, actA);
            Assert.Equal(new List<DTOTestValidRangeQ>() { this.tv.dtoQBC, this.tv.dtoQAC }, actB);
            Assert.Equal(new List<DTOTestValidRangeQ>() { }, actC);
        }
        {
            var sut = new List<DTOTestValidRangeQ>() { this.tv.dtoQAN, this.tv.dtoQBN, this.tv.dtoQCN };
            var actA = sut.WhereAtValidRangeQ(this.tv.dtoA).ToList();
            var actB = sut.WhereAtValidRangeQ(this.tv.dtoB).ToList();
            var actC = sut.WhereAtValidRangeQ(this.tv.dtoC).ToList();

            Assert.Equal(new List<DTOTestValidRangeQ>() { this.tv.dtoQAN }, actA);
            Assert.Equal(new List<DTOTestValidRangeQ>() { this.tv.dtoQAN, this.tv.dtoQBN }, actB);
            Assert.Equal(new List<DTOTestValidRangeQ>() { this.tv.dtoQAN, this.tv.dtoQBN, this.tv.dtoQCN }, actC);
        }
    }
}
