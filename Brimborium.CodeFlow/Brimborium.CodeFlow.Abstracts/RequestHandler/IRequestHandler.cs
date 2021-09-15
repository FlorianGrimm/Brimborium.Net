using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.RequestHandler {
    public interface IRequestHandler { }
    
    public interface IRequestHandler<TRequest, TResponse> : IRequestHandler {
        Task<TResponse> ExecuteAsync(TRequest request, IRequestHandlerContext context, CancellationToken cancellationToken = default);
    }

    //public interface IRequestHandler<TRequest, TResponse, TIRequestHandler> : IRequestHandler, IRequestHandler<TRequest, TResponse>
    //    where TIRequestHandler : IRequestHandler<TRequest, TResponse, TIRequestHandler> {
    //    public RequestHandlerTypeInfo<TRequestHandler, TRequest, TResponse> GetRequestHandlerTypeInfo<TRequestHandler>() => new RequestHandlerTypeInfo<TRequestHandler, TRequest, TResponse>();
    //}

    //public class RequestHandlerTypeInfo<TRequestHandler, TRequest, TResponse> {
    //}
}
