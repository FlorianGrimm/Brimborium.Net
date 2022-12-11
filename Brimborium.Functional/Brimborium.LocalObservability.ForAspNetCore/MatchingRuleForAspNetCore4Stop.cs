namespace Brimborium.LocalObservability.ForAspNetCore;

public class MatchingRuleForAspNetCore4Stop : IMatchingRule, IStateTransition {
    public MatchingRuleForAspNetCore4Stop() {
    }
    public MatchingKind Kind => MatchingKind.Stop;
    public int Priority => 0;
    public string Name => "ForAspNetCore";

    public IActualCodePoint? DoesConditionMatch(LogEntryData logEntry) {
        var categoryName = logEntry.CategoryName;
        var eventId = logEntry.EventId;
        if (categoryName == "Microsoft.AspNetCore.Hosting.Diagnostics") {
            if (eventId.Id == 2) {
                return new ActualCodePoint(WaypointsForAspNetCore.RequestStop, logEntry);
            }
        }
        return default;
    }

    public bool DoesActualCodePointMatch(IActualCodePoint actualCodePoint) {
        if (actualCodePoint.CodePoint == WaypointsForAspNetCore.RequestStart) {
            var valueRequestId = actualCodePoint.GetValues().FirstOrDefault(kv => kv.Key == "RequestId").Value as string;
            return !string.IsNullOrEmpty(valueRequestId);
        }
        return false;
    }

    public (ICodePointState CodePointState, bool Done) Execute(IActualCodePoint actualCodePoint, ICodePointState codePointState) {
        var valueRequestId = actualCodePoint.GetValues().FirstOrDefault(kv => kv.Key == "RequestId").Value as string;
        if (string.IsNullOrEmpty(valueRequestId)) {
            return (codePointState, false);
        }
        codePointState.RemoveChild("Request", valueRequestId);
        return (CodePointState: codePointState, Done: false);
    }
}

