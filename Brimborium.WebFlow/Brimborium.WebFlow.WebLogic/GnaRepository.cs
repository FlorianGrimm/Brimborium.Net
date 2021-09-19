using Brimborium.CodeFlow.RequestHandler;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.WebLogic {
    public sealed class GnaRepository : IGnaRepository {
        public List<GnaModel> Items { get; }

        public GnaRepository() {
            this.Items = new List<GnaModel>();
        }

        public async Task<List<GnaModel>> QueryAsync(string pattern, IRequestHandlerContext context, CancellationToken cancellationToken) {
            pattern = pattern.Trim();
            await Task.CompletedTask;
            lock (this) {
                var result = this.Items.Where(item => item.Name.Contains(pattern)).ToList();
                return result;
            }
        }

        public async Task<bool> UpsertAsync(GnaModel value) {
            bool result;
            lock (this) {
                var index = this.Items.FindIndex(i => i.Name == value.Name);
                if (index < 0) {
                    this.Items.Add(value with { });
                    result = true;
                } else {
                    this.Items[index] = (value with { });
                    result = false;
                }
            }
            await Task.Delay(2);
            return result;
        }
    }
}
