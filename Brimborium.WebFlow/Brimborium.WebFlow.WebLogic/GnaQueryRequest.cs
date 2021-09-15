using Brimborium.CodeFlow.RequestHandler;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.WebLogic {
    public record GnaQueryRequest(string Pattern);
    public record GnaQueryResponse(
            List<GnaModel> Items
        );
    public interface IGnaQueryRequestHandler: IRequestHandler<GnaQueryRequest, GnaQueryResponse> {
    }

    public class GnaQueryRequestHandler : IGnaQueryRequestHandler {
        private readonly GnaRepository _GnaRepository;

        public GnaQueryRequestHandler(GnaRepository gnaRepository) {
            this._GnaRepository = gnaRepository;
        }
        public async Task<GnaQueryResponse> ExecuteAsync(GnaQueryRequest request, IRequestHandlerContext context, CancellationToken cancellationToken = default) {
            var items= await this._GnaRepository.QueryAsync(request.Pattern, context, cancellationToken);
            return new GnaQueryResponse(items);
        }
    }
}
