using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.RequestHandler {
    public interface IRequestHandler<TRequest, TResponse> {
        Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
    }
}
