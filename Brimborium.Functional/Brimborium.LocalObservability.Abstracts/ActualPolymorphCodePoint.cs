namespace Brimborium.LocalObservability;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class ActualPolymorphCodePoint {
    public ActualPolymorphCodePoint() {
        this.MatchedActualCodePoint = new List<IActualCodePoint>();
    }

    public List<IActualCodePoint> MatchedActualCodePoint { get; }

    public void AddActualCodePoint(IActualCodePoint actualCodePoint) {
        this.MatchedActualCodePoint.Add(actualCodePoint);
        actualCodePoint.Container = this;
    }

    private string GetDebuggerDisplay() {
        return $"ActualPolymorphCodePoint #{this.MatchedActualCodePoint.Count}";
    }
}
