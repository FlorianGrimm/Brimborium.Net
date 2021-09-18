using Brimborium.CodeFlow;
using Brimborium.CodeFlow.RequestHandler;

using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.WebLogic {
    public record GnaUpsertRequest(
        string Name,
        bool Done,
        ClaimsPrincipal User
    );

    public record GnaUpsertResponse(
    );

    public interface IGnaUpsertRequestHandler : IRequestHandler<GnaUpsertRequest, RequestResult<GnaUpsertResponse>> {
    }

    public class GnaUpsertRequestHandler : IGnaUpsertRequestHandler {
        private readonly GnaRepository _GnaRepository;

        public GnaUpsertRequestHandler(GnaRepository gnaRepository) {
            this._GnaRepository = gnaRepository;
        }

        public async Task<RequestResult<GnaUpsertResponse>> ExecuteAsync(GnaUpsertRequest request, IRequestHandlerContext context, CancellationToken cancellationToken = default) {
            await Task.CompletedTask;
            if (request.Name == "Error") { 
                return new RequestResultErrorDetails() {
                    Status = 400,
                    Title = "Error",
                    Detail = "Error"
                };
            }
            if (string.IsNullOrEmpty(request.User.Identity?.Name)) {
                return new RequestResultForbidden();
            }
            await this._GnaRepository.UpsertAsync(new GnaModel(request.Name, request.Done));
            return new GnaUpsertResponse();
        }
    }
}
