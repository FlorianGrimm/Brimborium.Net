using Brimborium.WebFlow.Web.API;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.Controllers {
    public interface IGnaController {
        Task<ActionResult<IEnumerable<Gna>>> GetAsync(string? pattern);
        Task<ActionResult> PostAsync(Gna value, CancellationToken cancellationToken);
        Task<ActionResult> PostNameAsync(string Name, bool Done, CancellationToken cancellationToken);
    }
}