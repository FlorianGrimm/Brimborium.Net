#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions



namespace Brimborium.Functional;

public class NullExtensionTests {
    [Fact()]
    public void TryGetNotNull_Test() {
        {
            Ebbes? sut = null;
            Assert.False(sut.TryGetNotNull(out var result));
        }
        {
            Ebbes? sut = new Ebbes(1);
            Assert.True(sut.TryGetNotNull(out var result));
        }

        {
            string? sut = null;
            Assert.Equal(false, sut.TryGetNotNull(out var v));
            Assert.Null(v);
        }
        {
            var sut = "a";
            Assert.Equal(true, sut.TryGetNotNull(out var v));
            Assert.NotNull(v);
        }

        {
            string? sut = null;
            Assert.Equal(false, sut.TryGetNotNull((x) => x == "a", out var v));
            Assert.Null(v);
        }
        {
            var sut = "a";
            Assert.Equal(true, sut.TryGetNotNull((x) => x == "a", out var v));
            Assert.Equal("a", v);
        }
        {
            var sut = "b";
            Assert.Equal(false, sut.TryGetNotNull((x) => x == "a", out var v));
            Assert.Null(v);
        }
    }

    [Fact()]
    public void GetValueOrDefault_Test() {
        {
            Ebbes? sut = null;
            Assert.Equal(-1, sut.GetValueNotNullOrDefault(new Ebbes(-1)).EntityVersion);
        }
        {
            Ebbes? sut = new Ebbes(1);
            Assert.Equal(1, sut.GetValueNotNullOrDefault(new Ebbes(-1)).EntityVersion);
        }

        {
            string? sut = null;
            Assert.Equal("d", sut.GetValueOrDefault("d"));
            Assert.Equal("d", sut.GetValueOrDefault(() => "d"));
        }
        {
            var sut = "a";
            Assert.Equal("a", sut.GetValueOrDefault("d"));
            Assert.Equal("a", sut.GetValueOrDefault(() => "d"));
        }
    }

    [Fact()]
    public void GetValueNotNullOrDefault_Test() {
        {
            string? sut = null;
            Assert.Equal("default", sut.GetValueNotNullOrDefault("default"));
        }
        {
            var sut = "a";
            Assert.Equal("a", sut.GetValueNotNullOrDefault("default"));
        }
    }

    [Fact()]
    public void GetValueNotNullOrDefault_Test1() {
        {
            string? sut = null;
            Assert.Equal("default", sut.GetValueNotNullOrDefault(() => "default"));
        }
        {
            var sut = "a";
            Assert.Equal("a", sut.GetValueNotNullOrDefault(() => "default"));
        }
    }
}
