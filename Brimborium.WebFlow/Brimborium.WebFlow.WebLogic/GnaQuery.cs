using Brimborium.CodeFlow.RequestHandler;
using Brimborium.WebFlow.Web.Communication;
using Brimborium.WebFlow.Web.Model;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.WebLogic {

    public interface IGnaQueryRequestHandler : IRequestHandler<GnaQueryRequest, RequestResult<GnaQueryResponse>> { }

    public interface IGnaQueryRequestHandlerFactory : ITypedRequestHandlerFactory<IGnaQueryRequestHandler> { }

    public class GnaQueryRequestHandler : IGnaQueryRequestHandler {
        private readonly IGnaRepository _GnaRepository;
        private readonly ILogger _Logger;

        public GnaQueryRequestHandler(
            IGnaRepository gnaRepository,
            ILogger<GnaQueryRequestHandler> logger
            ) {
            this._GnaRepository = gnaRepository;
            this._Logger = logger;
        }

        public async Task<RequestResult<GnaQueryResponse>> ExecuteAsync(GnaQueryRequest request, IRequestHandlerContext context, CancellationToken cancellationToken = default) {
            request.Deconstruct(out var pattern);

            if (pattern == "*") {
                return new RequestResultErrorDetails(400) {
                    Title = "Pattern",
                    Detail = "Pattern cannot be *"
                };
            }
            try {
                if (pattern == "fail") {
                    throw new ArgumentException("fail");
                }
                if (pattern == "rethrow") {
                    throw new InvalidOperationException("rethrow");
                }

                return await this._GnaRepository
                    .QueryAsync(pattern, context, cancellationToken)
                    .WrapRequestResult(
                        convertOk: (items) => new GnaQueryResponse(items),
                        logger: this._Logger
                    );

            } catch (ArgumentException) {
                return new RequestResultErrorDetails(400) {
                    Title = "Pattern",
                    Detail = "Pattern is fail"
                };
            } catch (InvalidOperationException error) {
                return new RequestResultException(null, error) { Rethrow = true };
            }
        }
    }
}

