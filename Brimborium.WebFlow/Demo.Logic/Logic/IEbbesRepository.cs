using Demo.Model;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Logic {
    public interface IEbbesRepository {
        Task<List<EbbesModel>> QueryAsync(string pattern, CancellationToken cancellationToken);
        Task<bool> UpsertAsync(EbbesModel value);
    }
}