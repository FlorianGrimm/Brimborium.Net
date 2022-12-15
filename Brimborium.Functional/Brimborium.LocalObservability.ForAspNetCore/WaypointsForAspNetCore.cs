namespace Brimborium.LocalObservability.ForAspNetCore;

public class WaypointsForAspNetCore {
    public static CodePoint RequestStart = new CodePoint("RequestStart");
    public static CodePoint RequestMember = new CodePoint("RequestMember");
    public static CodePoint RequestStop = new CodePoint("RequestStop");
    public static CodePath RequestPath = new CodePathSequence(
        "RequestPath",
        new List<CodePath>() {
            new CodePathStep(RequestStart),
            new CodePathRepeat(
                new CodePathStep( RequestMember)
                ),
            new CodePathStep(RequestStop),
        }
        );
}
