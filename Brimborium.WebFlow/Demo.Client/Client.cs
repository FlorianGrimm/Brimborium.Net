using Brimborium.CodeFlow.RequestHandler;

using Demo.API;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Client {
    public class Client : IEbbesClientAPI, IGnaClientAPI {
        public Task<RequestResult<IEnumerable<Ebbes>>> EbbesGetAsync(string? pattern, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<RequestResult> EbbesUpsertAsync(Ebbes value, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<RequestResult<IEnumerable<Gna>>> GnaGetAsync(string? pattern, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<RequestResult> GnaUpsertAsync(Gna value, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}
