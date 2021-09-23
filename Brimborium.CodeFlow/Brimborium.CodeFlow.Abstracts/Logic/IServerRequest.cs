using System.Security.Claims;

namespace Brimborium.CodeFlow.Logic {
    public interface IServerRequest { 
        ClaimsPrincipal User { get; } 
    }
}
