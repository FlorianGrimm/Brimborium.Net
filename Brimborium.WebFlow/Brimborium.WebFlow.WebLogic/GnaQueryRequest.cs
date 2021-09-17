using Brimborium.CodeFlow.RequestHandler;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.WebLogic {
    public record GnaQueryRequest(
        string Pattern
        );

    public record GnaQueryResponse(
            List<GnaModel> Items
        );

    public interface IGnaQueryRequestHandler : IRequestHandler<GnaQueryRequest, RequestHandlerResult<GnaQueryResponse>> { }

    public interface IGnaQueryRequestHandlerFactory : ITypedRequestHandlerFactory<IGnaQueryRequestHandler> { }

    public class GnaQueryRequestHandler : IGnaQueryRequestHandler {
        private readonly GnaRepository _GnaRepository;

        public GnaQueryRequestHandler(GnaRepository gnaRepository) {
            this._GnaRepository = gnaRepository;
        }

        public async Task<RequestHandlerResult<GnaQueryResponse>> ExecuteAsync(GnaQueryRequest request, IRequestHandlerContext context, CancellationToken cancellationToken = default) {
            if (request.Pattern == "*") {
                return new RequestHandlerResultFailed("Pattern cannot be *", "Pattern");
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
            } catch (ArgumentException error) {
                return new RequestHandlerResultFailed("Pattern is fail", "Pattern", error, 500);
            } catch (InvalidOperationException error){
                return new RequestHandlerResultFailed("Pattern is rethrow", "Pattern", error, -1);
            }
        }
    }
}

