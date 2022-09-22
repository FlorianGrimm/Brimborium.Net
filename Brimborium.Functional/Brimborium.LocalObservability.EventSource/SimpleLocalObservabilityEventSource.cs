namespace Brimborium.LocalObservability.EventSource;

[EventSource(Name = "Brimborium-LocalObservability")]
public class SimpleLocalObservabilityEventSource 
    : System.Diagnostics.Tracing.EventSource
    , ILocalObservabilitySink {

    public SimpleLocalObservabilityEventSource() {

    }

    public void Visit(CodePoint location, CodePoint? direction = null, string? args = null) {
        WriteEvent(location.EventId, location.Name, direction?.Name ?? String.Empty, args ?? String.Empty);
    }

/*   
    public void Visit(int eventId, string location, string? direction = default, string? args = default) {
        WriteEvent(eventId, location, direction ?? String.Empty, args ?? String.Empty);
    }

    public void Visit(
        SimpleLocalObservabilityItem item
        ) {

        WriteEvent(item.Location.EventId, item.Location.Name, item.Direction?.Name ?? String.Empty, item.args ?? String.Empty);
    }
*/
    public static SimpleLocalObservabilityEventSource Log = new SimpleLocalObservabilityEventSource();
}