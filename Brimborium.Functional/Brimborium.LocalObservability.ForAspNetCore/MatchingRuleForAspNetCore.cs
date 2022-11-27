namespace Brimborium.LocalObservability.ForAspNetCore;

public class MatchingRuleForAspNetCore : IMatchingRule, IStateTransition {
    public MatchingRuleForAspNetCore() {
    }

    public string Name => "ForAspNetCore";

    public IActualCodePoint? DoesConditionMatch(IMatchingEntry entry) {
        if (entry is IMatchingEntry<ILogEntry> matchingLogEntry) {
            if (matchingLogEntry.GetLogEntry() is ILogEntry logEntry) {
                var categoryName = logEntry.CategoryName;
                var eventId = logEntry.EventId;
                if (categoryName == "Microsoft.AspNetCore.Hosting.Diagnostics") {
                    if (eventId.Id == 1) {
                        var proxyState = ProxyStateLogEntry.Create(logEntry.GetState());
                        if (proxyState is null) { return null; }
                        return new ActualCodePoint(WaypointsForAspNetCore.StartSession, proxyState);
                    }
                }
            }
        }
        return default;
    }

    public bool DoesActualCodePointMatch(IActualCodePoint actualCodePoint) {
        if (actualCodePoint.CodePoint == WaypointsForAspNetCore.StartSession) {
            // TODO check session
            return true;
        }
        return false;
    }

    public (ICodePointState CodePointState, bool Done) Execute(IActualCodePoint actualCodePoint, ICodePointState codePointState) {
        return (CodePointState:codePointState, Done:false);
    }
}

public class WaypointsForAspNetCore {
    public static CodePoint StartSession = new CodePoint("StartSession");
}