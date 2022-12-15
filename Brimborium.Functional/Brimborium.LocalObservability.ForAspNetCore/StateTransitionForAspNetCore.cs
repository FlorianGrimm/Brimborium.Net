namespace Brimborium.LocalObservability.ForAspNetCore;

public class StateTransitionForAspNetCore : IStateTransition {
    public StateTransitionForAspNetCore() {
    }

    public void Apply(
        ActualPolymorphCodePoint polymorphCodePoint,
        ICodePointState codePointState,
        IIncidentProcessingEngine1 incidentProcessingEngine) {
        foreach (var acp in polymorphCodePoint.MatchedActualCodePoint) {
            if (acp.CodePoint == WaypointsForAspNetCore.RequestStart) {
                var requestId = (string)acp.Values.First(v => v.Key == "RequestId").Value;
                var requestCPS = codePointState.CreateChild("Request", requestId);
                requestCPS.Add(polymorphCodePoint);
                return;
            }
            if (acp.CodePoint == WaypointsForAspNetCore.RequestMember) {
                var requestId = (string)acp.Values.First(v => v.Key == "RequestId").Value;
                var requestCPS = codePointState.GetChild("Request", requestId);
                if (requestCPS is not null) {
                    requestCPS.Add(polymorphCodePoint);
                    return;
                } else {
                    incidentProcessingEngine.SetReport(acp, $"no child for RequestId {requestId}");
                }
            }
            if (acp.CodePoint == WaypointsForAspNetCore.RequestStop) {
                var requestId = (string)acp.Values.First(v => v.Key == "RequestId").Value;
                var requestCPS = codePointState.GetChild("Request", requestId);
                if (requestCPS is not null) {
                    requestCPS.Add(polymorphCodePoint);
                    incidentProcessingEngine.SetTerminating(acp, codePointState);
                    return;
                } else {
                    incidentProcessingEngine.SetReport(acp, $"no child for RequestId {requestId}");
                }
            }
        }
    }
}