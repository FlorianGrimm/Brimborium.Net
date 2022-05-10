#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions

namespace Brimborium.RowVersion.Extensions;

public class NullExtensionTests
{
    [Fact()]
    public void TryGetNotNull_Test()
    {
        {
            TestEntityWithVersion? sut = null;
            Assert.False(sut.TryGetNotNull(out var result));
        }
        {
            TestEntityWithVersion? sut = new TestEntityWithVersion(1);
            Assert.True(sut.TryGetNotNull(out var result));
        }

        {
            string? sut = null;
            Assert.Equal(false, sut.TryGetNotNull(out var v));
            Assert.Null(v);
        }
        {
            string? sut = "a";
            Assert.Equal(true, sut.TryGetNotNull(out var v));
            Assert.NotNull(v);
        }
    }

    [Fact()]
    public void GetValueOrDefault_Test()
    {
        {
            TestEntityWithVersion? sut = null;
            Assert.Equal(-1, sut.GetValueNotNullOrDefault(new TestEntityWithVersion(-1)).EntityVersion);
        }
        {
            TestEntityWithVersion? sut = new TestEntityWithVersion(1);
            Assert.Equal(1, sut.GetValueNotNullOrDefault(new TestEntityWithVersion(-1)).EntityVersion);
        }

        {
            string? sut = null;
            Assert.Equal(false, sut.TryGetNotNull(out var v));
            Assert.Null(v);
        }
        {
            string? sut = "a";
            Assert.Equal(false, sut.TryGetNotNull(_ => false, out var v));
            Assert.Null(v);
        }
        {
            string? sut = "a";
            Assert.Equal(true, sut.TryGetNotNull(_ => true, out var v));
            Assert.NotNull(v);
        }
    }

    [Fact()]
    public void GetValueNotNullOrDefault_Test()
    {
        {
            string? sut = null;
            Assert.Equal("default", sut.GetValueNotNullOrDefault("default"));
        }
        {
            string? sut = "a";
            Assert.Equal("a", sut.GetValueNotNullOrDefault("default"));
        }
    }

    [Fact()]
    public void GetValueNotNullOrDefault_Test1()
    {
        {
            string? sut = null;
            Assert.Equal("default", sut.GetValueNotNullOrDefault(() => "default"));
        }
        {
            string? sut = "a";
            Assert.Equal("a", sut.GetValueNotNullOrDefault(() => "default"));
        }
    }
}
