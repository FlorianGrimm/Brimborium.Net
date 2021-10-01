using Brimborium.CodeFlow.API;
using Brimborium.CodeFlow.RequestHandler;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.API {
    public interface IGnaClientAPI : IClientAPI {
        Task<RequestResult<IEnumerable<Gna>>> GnaGetAsync(
            string? pattern,
            CancellationToken cancellationToken
            );

        Task<RequestResult> GnaUpsertAsync(
            Gna value,
            CancellationToken cancellationToken
            );
    }
}
