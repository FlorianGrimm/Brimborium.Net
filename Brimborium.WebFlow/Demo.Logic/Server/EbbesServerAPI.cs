using Brimborium.CodeFlow.RequestHandler;
using Brimborium.Registrator;

using Demo.Logic;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Server {
    [Scoped]
    public sealed class EbbesServerAPI : IEbbesServerAPI {
        private readonly IServiceProvider _RequestServices;

        public EbbesServerAPI(
            IServiceProvider requestServices
            ) {
            this._RequestServices = requestServices;
        }

        public async Task<RequestResult<EbbesServerGetResponse>> GetAsync(EbbesServerGetRequest request, CancellationToken cancellationToken) {
            IGlobalRequestHandlerFactory globalRequestHandlerFactory = this._RequestServices.GetRequiredService<IGlobalRequestHandlerFactory>();
            var requestHandler = globalRequestHandlerFactory.CreateRequestHandler<IGnaQueryRequestHandler>(this._RequestServices);

            await Task.CompletedTask;
            // this._RequestServices.GetRequiredService<>
            
            //request.User
            throw new NotImplementedException();
        }

        public async Task<RequestResult<EbbesServerUpsertResponse>> UpsertAsync(EbbesServerUpsertRequest request, CancellationToken cancellationToken) {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
    }
}
