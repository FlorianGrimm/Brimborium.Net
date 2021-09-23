using Brimborium.CodeFlow.RequestHandler;

using System.Threading;
using System.Threading.Tasks;

namespace Demo.Communication {
    public interface IEbbesService {
        Task<RequestResult<EbbesQueryResponse>> EbbesQueryAsync(
            EbbesQueryRequest request,
            IRequestHandlerContext context,
            CancellationToken cancellationToken);

        Task<RequestResult<EbbesUpsertResponse>> EbbesUpsertAsync(
            EbbesUpsertRequest request,
            IRequestHandlerContext context,
            CancellationToken cancellationToken);
    }
}
