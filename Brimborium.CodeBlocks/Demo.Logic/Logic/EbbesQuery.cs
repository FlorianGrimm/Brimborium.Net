using Brimborium.CodeFlow.RequestHandler;
using Brimborium.Registrator;

using Demo.Model;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Logic {
    public record EbbesQueryRequest(
        string Pattern,
        ClaimsPrincipal User
        ) : IRequest;

    public record EbbesQueryResponse(
        List<EbbesModel> Value
        ) : IResponse<List<EbbesModel>>;

    public interface IEbbesQueryRequestHandler : IRequestHandler<EbbesQueryRequest, RequestResult<EbbesQueryResponse>> { }

    //public interface IEbbesQueryRequestHandlerFactory : ITypedRequestHandlerFactory<IEbbesQueryRequestHandler> { }

    //public class EbbesQueryRequestHandlerFactory : IEbbesQueryRequestHandlerFactory {
    //    public IEbbesQueryRequestHandler CreateTypedRequestHandler(IServiceProvider scopedServiceProvider) {
    //        return scopedServiceProvider.GetRequiredService<EbbesQueryRequestHandler>();
    //    }
    //}

    [Scoped]
    public class EbbesQueryRequestHandler : IEbbesQueryRequestHandler {
        private readonly IEbbesRepository _EbbesRepository;
        private readonly ILogger _Logger;

        public EbbesQueryRequestHandler(
            IEbbesRepository EbbesRepository,
            ILogger<EbbesQueryRequestHandler> logger
            ) {
            this._EbbesRepository = EbbesRepository;
            this._Logger = logger;
        }

        public async Task<RequestResult<EbbesQueryResponse>> ExecuteAsync(EbbesQueryRequest request, CancellationToken cancellationToken = default) {
            request.Deconstruct(out var pattern, out var _);

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

                return await this._EbbesRepository
                    .QueryAsync(pattern, cancellationToken)
                    .WrapRequestResult(
                        convertOk: (items) => new EbbesQueryResponse(items),
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

