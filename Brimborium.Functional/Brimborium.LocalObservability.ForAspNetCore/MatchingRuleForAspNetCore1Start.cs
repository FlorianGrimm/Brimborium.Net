namespace Brimborium.LocalObservability.ForAspNetCore;

public class MatchingRuleForAspNetCore1Start : IMatchingRule, IStateTransition {
    public MatchingRuleForAspNetCore1Start() {
    }
    public MatchingKind Kind => MatchingKind.Start;
    public int Priority => 0;
    public string Name => "ForAspNetCore";

    public IActualCodePoint? DoesConditionMatch(LogEntryData entry) {
        var categoryName = entry.CategoryName;
        var eventId = entry.EventId;
        if (categoryName == "Microsoft.AspNetCore.Hosting.Diagnostics") {
            if (eventId.Id == 1) {
                return new ActualCodePoint(WaypointsForAspNetCore.StartSession, entry);
            }
        }
        return default;
    }

    public bool DoesActualCodePointMatch(IActualCodePoint actualCodePoint) {
        if (actualCodePoint.CodePoint == WaypointsForAspNetCore.StartSession) {
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
        var next = codePointState.CreateChild("Request", valueRequestId);
        return (CodePointState: next, Done: false);
    }
}

