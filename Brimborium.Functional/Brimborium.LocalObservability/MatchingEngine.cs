namespace Brimborium.LocalObservability;

public class MatchingEngineOptions {
    public readonly List<IMatchingRule> ListMatchingRule;
    public readonly List<IStateTransition> ListStateTransition;

    public MatchingEngineOptions() {
        this.ListMatchingRule = new List<IMatchingRule>();
        this.ListStateTransition = new List<IStateTransition>();
    }
}

public class MatchingEngine
    : IMatchingEngine {
    private readonly List<IMatchingRule> _ListMatchingRule;
    private readonly ChannelWriter<ActualPolymorphCodePoint> _ChannelWriter;
    private readonly IIncidentProcessingEngine2 _IncidentProcessingEngine;

    public MatchingEngine(
        MatchingEngineOptions options,
        IIncidentProcessingEngine2 incidentProcessingEngine
        ) {
        this._ListMatchingRule = new List<IMatchingRule>(options.ListMatchingRule.OrderBy(matchingRule => matchingRule.Kind).ThenBy(matchingRule => matchingRule.Priority).ThenBy(matchingRule => matchingRule.Name));
        this._IncidentProcessingEngine = incidentProcessingEngine;
        this._ChannelWriter = this._IncidentProcessingEngine.GetChannelWriter();
    }

    public void Match(LogEntryData entry) {
        // initialize the polymorphCodePoint with the MatchingRules
        // and store the matching polymorphCodePoint.ListActualCodePoint
        var actualPolymorphCodePoint = new ActualPolymorphCodePoint();
        foreach (var matchingRule in this._ListMatchingRule) {
            if (matchingRule.Match(entry, actualPolymorphCodePoint)) {
                return;
            }
        }
        // if some matched
        if (actualPolymorphCodePoint.MatchedActualCodePoint.Count > 0) {
            if (this._ChannelWriter.TryWrite(actualPolymorphCodePoint)) {
            } else {
                this._IncidentProcessingEngine.SetReport(actualPolymorphCodePoint, "Cannot write");
            }
        }
    }
}
