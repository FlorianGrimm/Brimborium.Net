using Microsoft.Extensions.Logging;

using Xunit;

namespace Brimborium.Extensions.Logging.LocalFile.Test;
public class LocalFileTest{
    [Fact]
    public void FailsTest(){
        Assert.True(!false);
    }
}