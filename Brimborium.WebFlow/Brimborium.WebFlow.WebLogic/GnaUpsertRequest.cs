using Brimborium.CodeFlow.RequestHandler;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.WebLogic {
    public record GnaUpsertRequest(GnaModel Item);
    public record GnaUpsertResponse(
        string? ErrorMessage,
        string? PropertyName
    );
    public interface IGnaUpsertRequestHandler : IRequestHandler<GnaUpsertRequest, GnaUpsertResponse> {
    }

    public class GnaUpsertRequestHandler : IGnaUpsertRequestHandler {
        private readonly GnaRepository _GnaRepository;

        public GnaUpsertRequestHandler(GnaRepository gnaRepository) {
            this._GnaRepository = gnaRepository;
        }
        public Task<GnaUpsertResponse> ExecuteAsync(GnaUpsertRequest request, IRequestHandlerContext context, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }
    }
}
