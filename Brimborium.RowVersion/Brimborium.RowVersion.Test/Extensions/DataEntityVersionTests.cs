#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions

namespace Brimborium.RowVersion.Extensions;

public class DataEntityVersionTests {
    [Fact]
    public void DataEntityVersion_Ctor_Test() {
        {
            var sut = new DataEntityVersion(1L);
            Assert.Equal(1L, sut.GetEntityVersion(ref sut));
            Assert.Equal("0000000000000001", sut.GetDataVersion(ref sut));
        }
        {
            var sut = new DataEntityVersion("1");
            Assert.Equal("0000000000000001", sut.GetDataVersion(ref sut));
            Assert.Equal(1L, sut.GetEntityVersion(ref sut));
        }
        {
            var sut = new DataEntityVersion(1L);
            Assert.Equal("0000000000000001", sut.GetDataVersion(ref sut));
            Assert.Equal(1L, sut.GetEntityVersion(ref sut));
        }
        {
            var sut = new DataEntityVersion("1");
            Assert.Equal(1L, sut.GetEntityVersion(ref sut));
            Assert.Equal("0000000000000001", sut.GetDataVersion(ref sut));
        }
        {
            var sut = new DataEntityVersion("1");
            Assert.Equal("0000000000000001", sut.GetDataVersion(ref sut));
            Assert.Equal(1L, sut.GetEntityVersion(ref sut));
        }
        {
            var sut = new DataEntityVersion("1");
            Assert.Equal("0000000000000001", (string)sut);
            Assert.Equal(1L, (long)sut);
        }

        {
            var sut = new DataEntityVersion(1);
            Assert.Equal("0000000000000001", (string)sut);
            Assert.Equal(1L, (long)sut);
        }
        {
            var sut = new DataEntityVersion("");
            Assert.Equal("0000000000000000", (string)sut);
            Assert.Equal(0L, (long)sut);
        }

        {
            var sut = new DataEntityVersion("x");
            Assert.Equal("ffffffffffffffff", (string)sut);
            Assert.Equal(-1L, (long)sut);
        }

        {
            var sut = new DataEntityVersion("0000000000000001");
            Assert.Equal("0000000000000001", (string)sut);
            Assert.Equal(1L, (long)sut);
        }
    }
}