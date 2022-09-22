namespace Brimborium.LocalObservability;

public class IgnoreLocalObservabilitySink
    : ILocalObservabilitySink {
    public void Visit(CodePoint location, CodePoint? direction = null, string? args = null) {
    }
}
