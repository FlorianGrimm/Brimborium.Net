//using System.Runtime.CompilerServices;
//[CallerMemberName]

namespace Brimborium.LocalObservability;

public class CodePoint {
    public string Name;
    public string? Description;
    public int EventId;

    public CodePoint(
        string? name=default, 
        string? description=default,
        int eventId=1
        ) {
        this.Name = name ?? string.Empty;
        this.Description = description;
        this.EventId = eventId;
    }

}
