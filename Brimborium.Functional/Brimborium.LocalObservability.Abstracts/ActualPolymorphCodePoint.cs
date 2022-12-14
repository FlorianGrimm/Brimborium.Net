namespace Brimborium.LocalObservability;

public class ActualPolymorphCodePoint {
    public ActualPolymorphCodePoint(LogEntryData entryData) {
        this.ListActualCodePoint = new List<IActualCodePoint>();
        this.EntryData = entryData;
    }

    public List<IActualCodePoint> ListActualCodePoint { get; }
    public LogEntryData EntryData { get; }
}
