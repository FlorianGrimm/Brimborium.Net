using Brimborium.CodeFlow.FlowSynchronization;
using Brimborium.CodeFlow.RequestHandler;
using Brimborium.Registrator;

using Demo.Model;

using Microsoft.Extensions.Logging;

using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Logic {
    public record EbbesUpsertRequest(
        string Name,
        bool Done,
        ClaimsPrincipal User
    ) : IRequest;

    public record EbbesUpsertResponse(
        ResponseVoid Value
    ) : IResponse<ResponseVoid>;

    public interface IEbbesUpsertRequestHandler : IRequestHandler<EbbesUpsertRequest, RequestResult<EbbesUpsertResponse>> {
    }

    [Scoped]
    public class EbbesUpsertRequestHandler : IEbbesUpsertRequestHandler {
        private readonly IEbbesRepository _EbbesRepository;
        private readonly SyncLockCollection _SyncLockCollection;
        private readonly SyncDictionary _SyncDictionary;
        private readonly ILogger _Logger;

        public EbbesUpsertRequestHandler(
            IEbbesRepository EbbesRepository,
            SyncLockCollection syncLockCollection,
            SyncDictionary syncDictionary,
            ILogger<EbbesUpsertRequestHandler> logger) {
            this._EbbesRepository = EbbesRepository;
            this._SyncLockCollection = syncLockCollection;
            this._SyncDictionary = syncDictionary;
            this._Logger = logger;
        }

        public async Task<RequestResult<EbbesUpsertResponse>> ExecuteAsync(EbbesUpsertRequest request, CancellationToken cancellationToken) {
            try {
                request.Deconstruct(out var name, out var done, out var user);
                var id = new Identity<string>(name);
                using (var syncLock = await this._SyncDictionary.LockAsync<EbbesModel>(id, true, this._SyncLockCollection, cancellationToken)) {
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
                    await this._EbbesRepository.UpsertAsync(new EbbesModel(name, done));
                    return new EbbesUpsertResponse(ResponseVoid.GetInstance());
                }
            } catch (System.Exception error) {
                return RequestResultException.CatchAndLog(error, default, this._Logger);
            }
        }
    }
}
