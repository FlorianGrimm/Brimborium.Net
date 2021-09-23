using Demo.Model;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Logic {
    public interface IGnaRepository {
        Task<List<GnaModel>> QueryAsync(string pattern, CancellationToken cancellationToken);
        Task<bool> UpsertAsync(GnaModel value);
    }
}