namespace Brimborium.Functional;
public class MD5ExtensionTests {
    [Fact]
    public void GetMD5HashFromString_Test() {
        Assert.Equal("bm1f5rHXIl9AtlSdW9b1xQ==", MD5Extension.GetMD5HashFromString("ebbes"));

    }
    [Fact]
    public void GetMD5HashFromByteArray_Test() {
        Assert.Equal("DuBkbBx32BMcyPTuZcdnOw==", MD5Extension.GetMD5HashFromByteArray(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }));
    }
}

