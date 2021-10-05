using Brimborium.CodeFlow.RequestHandler;
using Brimborium.CodeFlow.Server;
using Brimborium.Registrator;

using Demo.API;
using Demo.Logic;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Server {
    [Scoped]
    public sealed class GnaServerAPI : IGnaServerAPI {
        private readonly IServiceProvider _RequestServices;

        public GnaServerAPI(
            IServiceProvider requestServices
            ) {
            this._RequestServices = requestServices;
        }

        public async Task<RequestResult<GnaServerGetResponse>> GetAsync(GnaServerGetRequest request, CancellationToken cancellationToken) {
            IGlobalRequestHandlerFactory globalRequestHandlerFactory = this._RequestServices.GetRequiredService<IGlobalRequestHandlerFactory>();
            IGnaQueryRequestHandler requestHandler = globalRequestHandlerFactory.CreateRequestHandler<IGnaQueryRequestHandler>(this._RequestServices);
            request.Deconstruct(out var Pattern, out var User);
            GnaQueryRequest logicRequest = new GnaQueryRequest(Pattern ?? string.Empty, User);
            RequestResult<GnaQueryResponse> logicResponse = await requestHandler.ExecuteAsync(logicRequest, cancellationToken);
            IServerRequestResultConverter serverRequestResultConverter = this._RequestServices.GetRequiredService<IServerRequestResultConverter>();
            return serverRequestResultConverter.ConvertToServerResultOfT<GnaQueryResponse, GnaServerGetResponse>(logicResponse, convert);

            static GnaServerGetResponse convert(GnaQueryResponse response) {
                IEnumerable<Gna> resultValue = response.Value.Select(i => new Gna(i.Name, i.Done)).ToList();
                return new GnaServerGetResponse(resultValue);
            }
        }

        public Task<RequestResult<GnaServerUpsertResponse>> UpsertAsync(GnaServerUpsertRequest request, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}
