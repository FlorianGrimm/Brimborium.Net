using Brimborium.CodeFlow.RequestHandler;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.WebLogic {
    public record GnaQueryRequest(
        string Pattern,
        ClaimsPrincipal User
    );

    public record GnaQueryResponse(
        List<GnaModel> Items
    );

    public interface IGnaQueryRequestHandler : IRequestHandler<GnaQueryRequest, RequestResult<GnaQueryResponse>> { }

    public interface IGnaQueryRequestHandlerFactory : ITypedRequestHandlerFactory<IGnaQueryRequestHandler> { }

    public class GnaQueryRequestHandler : IGnaQueryRequestHandler {
        private readonly GnaRepository _GnaRepository;

        public GnaQueryRequestHandler(GnaRepository gnaRepository) {
            this._GnaRepository = gnaRepository;
        }

        public async Task<RequestResult<GnaQueryResponse>> ExecuteAsync(GnaQueryRequest request, IRequestHandlerContext context, CancellationToken cancellationToken = default) {
            if (request.Pattern == "*") {
                return new RequestResultErrorDetails(400) {
                    Title = "Pattern",
                    Detail = "Pattern cannot be *"
                };
            }
            try {
                if (request.Pattern == "fail") {
                    throw new ArgumentException("fail");
                }
                if (request.Pattern == "rethrow") {
                    throw new InvalidOperationException("rethrow");
                }
                var items = await this._GnaRepository.QueryAsync(request.Pattern, context, cancellationToken);
                return new GnaQueryResponse(items);
            } catch (ArgumentException) {
                return new RequestResultErrorDetails(400) {
                    Title = "Pattern",
                    Detail = "Pattern is fail"
                };
            } catch (InvalidOperationException error) {
                return new RequestResultException(null, error) { Rethrow = true};
            }
        }
    }
}

