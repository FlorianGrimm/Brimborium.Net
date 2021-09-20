using Brimborium.CodeFlow.RequestHandler;
using Brimborium.WebFlow.Web.Communication;

using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.WebLogic {

    public class GnaServiceFactory {
        public IGnaService Create(
            IRequestHandlerContext context,
            ClaimsPrincipal user
            ) {
            return new GnaService(context, user);
        }
    }
    public class GnaService : IGnaService {
        public GnaService(
            IRequestHandlerContext context,
            ClaimsPrincipal user
            ) {
            this.User = user;
            this.Context = context;
        }

        public ClaimsPrincipal User { get; }
        public IRequestHandlerContext Context { get; }

        public async Task<RequestResult<GnaQueryResponse>> GnaQueryAsync(GnaQueryRequest request, CancellationToken cancellationToken) {
            var requestHandler = this.Context.CreateRequestHandler<IGnaQueryRequestHandler>();
            var response = await requestHandler.ExecuteAsync(request, this.Context, cancellationToken);
            return response;
        }

        public async Task<RequestResult<GnaUpsertResponse>> GnaUpsertAsync(GnaUpsertRequest request, CancellationToken cancellationToken) {
            var requestHandler = this.Context.CreateRequestHandler<IGnaUpsertRequestHandler>();
            var response = await requestHandler.ExecuteAsync(request, this.Context, cancellationToken);
            return response;
        }
    }
}
