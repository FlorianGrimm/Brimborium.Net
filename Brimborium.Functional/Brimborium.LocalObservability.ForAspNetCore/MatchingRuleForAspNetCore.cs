namespace Brimborium.LocalObservability.ForAspNetCore;

public class MatchingRuleForAspNetCore : IMatchingRule {
    public MatchingRuleForAspNetCore() {
    }

    public MatchingKind Kind => MatchingKind.Start;
    public int Priority => 0;
    public string Name => "ForAspNetCore";

    public bool Match(ActualPolymorphCodePoint polymorphCodePoint) {
        var categoryName = polymorphCodePoint.EntryData.CategoryName;
        if (categoryName == "Microsoft.AspNetCore.Hosting.Diagnostics") {
            var eventId = polymorphCodePoint.EntryData.EventId;
            if (eventId.Id == 1) {
                var requestId = polymorphCodePoint.EntryData.GetValues().FirstOrDefault(kv => kv.Key == "RequestId").Value as string;
                if (!string.IsNullOrEmpty(requestId)) {
                    polymorphCodePoint.ListActualCodePoint.Add(
                        new ActualCodePoint(
                            WaypointsForAspNetCore.RequestStart,
                            new RequestIdValues(requestId)));
                    return true;
                };
            }
            if (eventId.Id == 2) {
                var requestId = polymorphCodePoint.EntryData.GetValues().FirstOrDefault(kv => kv.Key == "RequestId").Value as string;
                if (!string.IsNullOrEmpty(requestId)) {
                    polymorphCodePoint.ListActualCodePoint.Add(
                        new ActualCodePoint(
                            WaypointsForAspNetCore.RequestStop,
                            new RequestIdValues(requestId)));
                    return true;
                };
            }
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

