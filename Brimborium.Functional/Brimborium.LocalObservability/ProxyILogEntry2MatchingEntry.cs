using System.Collections;

namespace Brimborium.LocalObservability;

public class ProxyILogEntry2MatchingEntry 
    : IMatchingEntry
    ,IMatchingEntry<ILogEntry> {
    private readonly ILogEntry _LogEntry;

    public ProxyILogEntry2MatchingEntry(ILogEntry logEntry) {
        this._LogEntry = logEntry;
    }

    ILogEntry? IMatchingEntry<ILogEntry>.GetLogEntry() => this._LogEntry;
}
public class ProxyStateLogEntry: IEnumerable<KeyValuePair<string, object>> {
    private readonly IEnumerable<KeyValuePair<string, object>> _State;

    public static ProxyStateLogEntry? Create(object? values) {
        if (values is IEnumerable<KeyValuePair<string, object>> e) { 
            return new ProxyStateLogEntry(e);
        }
        return null;
    }

    public ProxyStateLogEntry(IEnumerable<KeyValuePair<string, object>> state) {
        this._State = state;
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
        return this._State.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable)this._State).GetEnumerator();
    }
}