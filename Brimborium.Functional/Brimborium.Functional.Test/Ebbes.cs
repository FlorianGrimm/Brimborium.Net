#pragma warning disable xUnit2004 // Do not use equality check to test for boolean conditions



namespace Brimborium.Functional;

public class Ebbes {
    public Ebbes(long entityVersion = 0) {
        this.EntityVersion = entityVersion;
    }
    public long EntityVersion { get; set; }
}