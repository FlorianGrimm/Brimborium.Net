#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions

namespace Brimborium.RowVersion.Extensions;

public class DataVersionExtensionsTests {
    [Fact]
    public void DataVersionIsEmptyOrZero_Null_Test() {
        Assert.True(DataVersionExtensions.DataVersionIsEmptyOrZero(null));
    }

    [Fact]
    public void DataVersionIsEmptyOrZero_Empty_Test() {
        Assert.True(DataVersionExtensions.DataVersionIsEmptyOrZero(""));
    }

    [Fact]
    public void DataVersionIsEmptyOrZero_0_Test() {
        Assert.True(DataVersionExtensions.DataVersionIsEmptyOrZero(DataVersionExtensions.ToDataVersion(0)));
    }

    [Fact]
    public void ToDataVersion_0_Test() {
        Assert.Equal("0000000000000000", DataVersionExtensions.ToDataVersion(0));
    }

    [Fact]
    public void DataVersionExtensions_1_Test() {
        Assert.Equal("0000000000000001", DataVersionExtensions.ToDataVersion(1));
    }

    [Fact]
    public void ToEntityVersion_1_Test() {
        Assert.Equal(1, DataVersionExtensions.ToEntityVersion("0000000000000001"));
    }

    [Fact]
    public void ToEntityVersion_null_Test() {
        Assert.Equal(0, DataVersionExtensions.ToEntityVersion(null));
    }

    [Fact]
    public void ToEntityVersion_empty_Test() {
        Assert.Equal(0, DataVersionExtensions.ToEntityVersion(""));
    }

    [Fact]
    public void ToEntityVersion_notvalidcontent_Test() {
        Assert.Equal(-1, DataVersionExtensions.ToEntityVersion("notvalidcontent"));
    }

    [Fact]
    public void EntityVersionDoesMatch_1_Test() {
        Assert.Equal(true, DataVersionExtensions.EntityVersionDoesMatch(1, 1));
        Assert.Equal(true, DataVersionExtensions.EntityVersionDoesMatch(1, 0));
        Assert.Equal(false, DataVersionExtensions.EntityVersionDoesMatch(1, 2));
        Assert.Equal(false, DataVersionExtensions.EntityVersionDoesMatch(2, 1));
        Assert.Equal(false, DataVersionExtensions.EntityVersionDoesMatch(0, 1));
    }
}
