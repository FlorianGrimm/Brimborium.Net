using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.RequestHandler {
    public interface IRequestHandler { }
    
    public interface IRequestHandler<TRequest, TResponse> : IRequestHandler {
        Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}
