using System.Threading.Tasks;

namespace Brimborium.CodeFlow.RequestHandler {
    internal record RequestHandlerClosure<TRequestHandler, TRequest, TResponse>(
        TRequestHandler RequestHandler
        // IRequestHandlerContext Context
        )
        where TRequestHandler : notnull, IRequestHandler, IRequestHandler<TRequest, TResponse> {
        public Task<TResponse> ExecuteAsync(TRequest request) {
            return this.RequestHandler.ExecuteAsync(request);
        }
    }
}
