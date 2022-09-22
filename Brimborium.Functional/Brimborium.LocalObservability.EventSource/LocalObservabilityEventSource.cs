namespace Brimborium.LocalObservability.EventSource;

public class LocalObservabilityEventSource : System.Diagnostics.Tracing.EventSource {
    public void Visit(CodePoint location, CodePoint? direction = null, string? args = null) {
        WriteEvent(location.EventId, location.Name, direction?.Name ?? String.Empty, args ?? String.Empty);
    }

    //public static LocalObservabilityEventSource Log = new LocalObservabilityEventSource();
}
