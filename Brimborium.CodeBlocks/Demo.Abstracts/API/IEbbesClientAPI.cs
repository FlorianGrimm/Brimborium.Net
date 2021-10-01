using Brimborium.CodeFlow.API;
using Brimborium.CodeFlow.RequestHandler;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.API {
    public interface IEbbesClientAPI : IClientAPI {
        Task<RequestResult<IEnumerable<Ebbes>>> EbbesGetAsync(
            string? pattern,
            CancellationToken cancellationToken
            );

        Task<RequestResult> EbbesUpsertAsync(
            Ebbes value,
            CancellationToken cancellationToken
            );
    }
}
