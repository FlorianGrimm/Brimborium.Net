using Brimborium.CodeFlow.RequestHandler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.WebLogic {
    public class GnaRepository {
        public List<GnaModel> Items { get; }

        public GnaRepository() {
            this.Items = new List<GnaModel>();
        }

        public async Task<List<GnaModel>> QueryAsync(string pattern, IRequestHandlerContext context, CancellationToken cancellationToken) {
            pattern = pattern.Trim();
            await Task.CompletedTask;
            var result = this.Items.Where(item => item.Name.Contains(pattern)).ToList();
            return result;
        }
    }

}
