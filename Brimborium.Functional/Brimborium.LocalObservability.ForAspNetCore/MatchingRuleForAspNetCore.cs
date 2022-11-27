namespace Brimborium.LocalObservability.ForAspNetCore;

public class MatchingRuleForAspNetCore : IMatchingRule {
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
}

public class WaypointsForAspNetCore {
    public static CodePoint StartSession = new CodePoint("StartSession");
}