using Brimborium.CodeFlow.RequestHandler;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.WebLogic {
    public interface IGnaRepository {
        Task<List<GnaModel>> QueryAsync(string pattern, IRequestHandlerContext context, CancellationToken cancellationToken);
        Task<bool> UpsertAsync(GnaModel value);
    }
}