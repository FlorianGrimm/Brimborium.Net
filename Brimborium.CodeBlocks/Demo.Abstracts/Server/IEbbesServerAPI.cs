using Brimborium.CodeFlow.Logic;
using Brimborium.CodeFlow.RequestHandler;
using Brimborium.CodeFlow.Server;

using Demo.API;

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Server {
    public record EbbesServerGetRequest(
        string? Pattern,
        ClaimsPrincipal User
    ) : IServerRequest { 
    }

    public record EbbesServerGetResponse (
       IEnumerable<Ebbes> Value
    ) : IServerResponse<IEnumerable<Ebbes>> {
    }

    public record EbbesServerUpsertRequest(
        Ebbes Value,
        ClaimsPrincipal User
    ) : IServerRequest {
    }

    public record EbbesServerUpsertResponse(
       ResultVoid Value
    ) : IServerResponse<ResultVoid> {
    }

    public interface IEbbesServerAPI : IServerAPI {
        Task<RequestResult<EbbesServerGetResponse>> GetAsync(
            EbbesServerGetRequest request,
            CancellationToken cancellationToken);

        Task<RequestResult<EbbesServerUpsertResponse>> UpsertAsync(
            EbbesServerUpsertRequest request,
            CancellationToken cancellationToken);
    }
}