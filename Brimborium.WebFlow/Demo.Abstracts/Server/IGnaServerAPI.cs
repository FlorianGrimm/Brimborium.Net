using Brimborium.CodeFlow.Logic;
using Brimborium.CodeFlow.RequestHandler;
using Brimborium.CodeFlow.Server;

using Demo.API;

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Server {
    public record GnaServerGetRequest(
        string? Pattern,
        ClaimsPrincipal User
        ) : IServerRequest {
    }
    public record GnaServerGetResponse(
       IEnumerable<Gna> Value
    ) : IServerResponse<IEnumerable<Gna>> {
    }

    public record GnaServerUpsertRequest(
        Gna Value,
        ClaimsPrincipal User
        ) : IServerRequest {
    }

    public record GnaServerUpsertResponse(
       ResultVoid Value
    ) : IServerResponse<ResultVoid> {
    }

    public interface IGnaServerAPI : IServerAPI {
        Task<RequestResult<GnaServerGetResponse>> GetAsync(
            GnaServerGetRequest request,
            CancellationToken cancellationToken);

        Task<RequestResult<GnaServerUpsertResponse>> UpsertAsync(
            GnaServerUpsertRequest request,
            CancellationToken cancellationToken);
    }
}