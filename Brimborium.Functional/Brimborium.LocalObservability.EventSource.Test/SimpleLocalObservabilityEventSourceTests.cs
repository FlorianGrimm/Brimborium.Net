namespace Brimborium.LocalObservability.EventSource.Test;

public class SimpleLocalObservabilityEventSourceTests {
    [Fact]
    public void Test1() {
        var cp = new CodePoint("Hello");
        SimpleLocalObservabilityEventSource.Log.Visit(cp);
    }
}