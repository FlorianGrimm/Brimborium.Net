//using Brimborium.CodeFlow.Logic;
//using Brimborium.CodeFlow.RequestHandler;
//using Brimborium.CodeFlow.Server;

//using Demo.API;

//using System.Collections.Generic;
//using System.Security.Claims;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Demo.Server {
//    public record GnaServerGetRequest(
//        string? Pattern,
//        ClaimsPrincipal User
//        ) : IServerRequest {
//    }
//    public record GnaServerGetResponse(
//       IEnumerable<Gna> Value
//    ) : IServerResponse<IEnumerable<Gna>> {
//    }

//    public record GnaServerUpsertRequest(
//        Gna Value,
//        ClaimsPrincipal User
//        ) : IServerRequest {
//    }

//    public record GnaServerUpsertResponse(
//       ResultVoid Value
//    ) : IServerResponse<ResultVoid> {
//    }

//    public interface IGnaServerAPI : IServerAPI {
//        Task<RequestResult<GnaServerGetResponse>> GetAsync(
//            GnaServerGetRequest request,
//            CancellationToken cancellationToken);

//        Task<RequestResult<GnaServerUpsertResponse>> UpsertAsync(
//            GnaServerUpsertRequest request,
//            CancellationToken cancellationToken);
//    }
//}
using Demo.Server;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Server {
    public interface IGnaServerAPI {
        System.Threading.Tasks.Task<Brimborium.CodeFlow.RequestHandler.RequestResult<Demo.Server.GnaServerGetResponse>> GetAsync(
            Demo.Server.GnaServerGetRequest request,
            System.Threading.CancellationToken cancellationToken
        );

        System.Threading.Tasks.Task<Brimborium.CodeFlow.RequestHandler.RequestResult<Demo.Server.GnaServerPostResponse>> PostAsync(
            Demo.Server.GnaServerPostRequest request,
            System.Threading.CancellationToken cancellationToken
        );

        System.Threading.Tasks.Task<Brimborium.CodeFlow.RequestHandler.RequestResult<Demo.Server.GnaServerPostNameResponse>> PostNameAsync(
            Demo.Server.GnaServerPostNameRequest request,
            System.Threading.CancellationToken cancellationToken
        );

    }
    public record GnaServerGetRequest(
        System.String Pattern,
        System.Security.Claims.ClaimsPrincipal User
    ) : Brimborium.CodeFlow.Logic.IServerRequest;

    public record GnaServerGetResponse(
        System.Collections.Generic.IEnumerable<Demo.API.Gna> Value
    ) : Brimborium.CodeFlow.Logic.IServerResponse<System.Collections.Generic.IEnumerable<Demo.API.Gna>>;

    public record GnaServerPostRequest(
        Demo.API.Gna Value,
        System.Security.Claims.ClaimsPrincipal User
    ) : Brimborium.CodeFlow.Logic.IServerRequest;

    public record GnaServerPostResponse(
        Brimborium.CodeFlow.RequestHandler.ResultVoid Value
    ) : Brimborium.CodeFlow.Logic.IServerResponse<Brimborium.CodeFlow.RequestHandler.ResultVoid>;

    public record GnaServerPostNameRequest(
        System.String Name,
        System.Boolean Done,
        System.Security.Claims.ClaimsPrincipal User
    ) : Brimborium.CodeFlow.Logic.IServerRequest;

    public record GnaServerPostNameResponse(
        Brimborium.CodeFlow.RequestHandler.ResultVoid Value
    ) : Brimborium.CodeFlow.Logic.IServerResponse<Brimborium.CodeFlow.RequestHandler.ResultVoid>;

}
