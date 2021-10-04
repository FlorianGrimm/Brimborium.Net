using Brimborium.CodeFlow.RequestHandler;

using System.Threading;
using System.Threading.Tasks;

namespace Demo.Communication {
    /*
     Gna
    Query
    Upsert

     */
    public interface IGnaService {
        Task<RequestResult<GnaQueryResponse>> GnaQueryAsync(
            GnaQueryRequest request,
            CancellationToken cancellationToken);

        Task<RequestResult<GnaUpsertResponse>> GnaUpsertAsync(
            GnaUpsertRequest request,
            CancellationToken cancellationToken);
    }
}
