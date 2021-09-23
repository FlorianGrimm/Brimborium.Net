using Brimborium.CodeFlow.FlowSynchronization;
using Brimborium.CodeFlow.RequestHandler;
using Brimborium.Registrator;

using Demo.Model;

using Microsoft.Extensions.Logging;

using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Logic {
    public record GnaUpsertRequest(
        string Name,
        bool Done,
        ClaimsPrincipal User
    ) : IRequest;

    public record GnaUpsertResponse(
        ResponseVoid Value
    ) : IResponse<ResponseVoid>;

    public interface IGnaUpsertRequestHandler : IRequestHandler<GnaUpsertRequest, RequestResult<GnaUpsertResponse>> {
    }

    [Scoped]
    public class GnaUpsertRequestHandler : IGnaUpsertRequestHandler {
        private readonly IGnaRepository _GnaRepository;
        private readonly SyncLockCollection _SyncLockCollection;
        private readonly SyncDictionary _SyncDictionary;
        private readonly ILogger _Logger;

        public GnaUpsertRequestHandler(
            IGnaRepository gnaRepository,
            SyncLockCollection syncLockCollection,
            SyncDictionary syncDictionary,
            ILogger<GnaUpsertRequestHandler> logger) {
            this._GnaRepository = gnaRepository;
            this._SyncLockCollection = syncLockCollection;
            this._SyncDictionary = syncDictionary;
            this._Logger = logger;
        }

        public async Task<RequestResult<GnaUpsertResponse>> ExecuteAsync(GnaUpsertRequest request, CancellationToken cancellationToken) {
            try {
                request.Deconstruct(out var name, out var done, out var user);
                var id = new Identity<string>(name);
                using (var syncLock = await this._SyncDictionary.LockAsync<GnaModel>(id, true, this._SyncLockCollection, cancellationToken)) {
                    if (name == "Error") {
                        return new RequestResultErrorDetails() {
                            Status = 400,
                            Title = "Error",
                            Detail = "Error"
                        };
                    }
                    if (string.IsNullOrEmpty(user?.Identity?.Name)) {
                        return new RequestResultForbidden();
                    }
                    await this._GnaRepository.UpsertAsync(new GnaModel(name, done));
                    return new GnaUpsertResponse(ResponseVoid.GetInstance());
                }
            } catch (System.Exception error) {
                return RequestResultException.CatchAndLog(error, default, this._Logger);
            }
        }
    }
}
