using Brimborium.CodeFlow.FlowSynchronization;
using Brimborium.CodeFlow.RequestHandler;
using Brimborium.WebFlow.Web.Communication;
using Brimborium.WebFlow.Web.Model;

using Microsoft.Extensions.Logging;

using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.WebLogic {

    public interface IGnaUpsertRequestHandler : IRequestHandler<GnaUpsertRequest, RequestResult<GnaUpsertResponse>> {
    }

    public class GnaUpsertRequestHandler : IGnaUpsertRequestHandler {
        private readonly IGnaRepository _GnaRepository;
        private readonly SyncDictionary _SyncDictionary;
        private readonly ILogger _Logger;

        public GnaUpsertRequestHandler(
            IGnaRepository gnaRepository,
            SyncDictionary syncDictionary,
            ILogger<GnaUpsertRequestHandler> logger) {
            this._GnaRepository = gnaRepository;
            this._SyncDictionary = syncDictionary;
            this._Logger = logger;
        }

        public async Task<RequestResult<GnaUpsertResponse>> ExecuteAsync(GnaUpsertRequest request, IRequestHandlerContext context, CancellationToken cancellationToken = default) {
            try {
                var id = new Identity<string>(request.Name);
                using (var syncLock = await this._SyncDictionary.LockAsync<GnaModel>(id, true, context.GetRequiredService<SyncLockCollection>(), cancellationToken)) {
                    request.Deconstruct(out var Name, out var Done);
                    var user = context.GetUser();
                    if (Name == "Error") {
                        return new RequestResultErrorDetails() {
                            Status = 400,
                            Title = "Error",
                            Detail = "Error"
                        };
                    }
                    if (string.IsNullOrEmpty(user?.Identity?.Name)) {
                        return new RequestResultForbidden();
                    }
                    await this._GnaRepository.UpsertAsync(new GnaModel(Name, Done));
                    return new GnaUpsertResponse();
                }
            } catch (System.Exception error) {
                return RequestResultException.CatchAndLog(error, default, this._Logger);
            }
        }
    }
}
