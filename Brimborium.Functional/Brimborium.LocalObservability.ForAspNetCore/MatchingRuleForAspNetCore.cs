namespace Brimborium.LocalObservability.ForAspNetCore;

public class MatchingRuleForAspNetCore : IMatchingRule {
    public MatchingRuleForAspNetCore() {
    }

    public MatchingKind Kind => MatchingKind.Start;
    public int Priority => 0;
    public string Name => "ForAspNetCore";

    public bool Match(LogEntryData entryData, ActualPolymorphCodePoint polymorphCodePoint) {
        var requestId = entryData.GetValues().FirstOrDefault(kv => kv.Key == "RequestId").Value as string;
        if (!string.IsNullOrEmpty(requestId)) {
            var categoryName = entryData.CategoryName;
            if (categoryName == "Microsoft.AspNetCore.Hosting.Diagnostics") {
                var eventId = entryData.EventId;
                if (eventId.Id == 1) {
                    polymorphCodePoint.AddActualCodePoint(
                        new ActualCodePoint(
                            WaypointsForAspNetCore.RequestStart,
                            new RequestIdValues(requestId)));
                    return false;
                }
                if (eventId.Id == 2) {
                    polymorphCodePoint.AddActualCodePoint(
                        new ActualCodePoint(
                            WaypointsForAspNetCore.RequestStop,
                            new RequestIdValues(requestId)));
                    return false;
                }
            }
            polymorphCodePoint.AddActualCodePoint(
                new ActualCodePoint(
                        WaypointsForAspNetCore.RequestMember,
                        new RequestIdValues(requestId)));
            return false;
        }
        return false;
    }
}

public record class RequestIdValues(
    string RequestId
) : IEnumerable<KeyValuePair<string, object>> {
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
        yield return new KeyValuePair<string, object>("RequestId", this.RequestId);
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return this.GetEnumerator();
    }
}
