using Demo.Model;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Logic {
    public sealed class EbbesRepository : IEbbesRepository {
        public List<EbbesModel> Items { get; }

        public EbbesRepository() {
            this.Items = new List<EbbesModel>();
        }

        public async Task<List<EbbesModel>> QueryAsync(string pattern, CancellationToken cancellationToken) {
            pattern = pattern.Trim();
            await Task.CompletedTask;
            lock (this) { }
            var result = this.Items.Where(item => item.Name.Contains(pattern)).ToList();
            return result;
        }

        public async Task<bool> UpsertAsync(EbbesModel value) {
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
