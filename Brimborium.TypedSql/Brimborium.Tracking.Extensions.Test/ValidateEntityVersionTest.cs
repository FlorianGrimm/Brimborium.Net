
using Xunit;

namespace Brimborium.Tracking.Extensions;

public class ValidateEntityVersionTest {
    [Fact]
    public void ValidateEntityVersion_1_Test() {
        var dbContext = new DBContext();
        dbContext.Ebbes.Add(new EbbesEntity(Id: Guid.NewGuid(), Title: "0", EntityVersion: 0L));
        Assert.Equal(1, dbContext.Ebbes.Count);
        Assert.Throws<InvalidModificationException> (() => { 
            dbContext.Ebbes.Add(new EbbesEntity(Id: Guid.NewGuid(), Title: "1", EntityVersion: 1L));
        });
        Assert.Equal(1, dbContext.Ebbes.Count);
    }
}