using Brimborium.WebFlow.Web.API;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.Controllers {
    public interface IEbbesController {
        Task<ActionResult> EbbesUpsertAsync(Ebbes value);
        Task<ActionResult<IEnumerable<Ebbes>>> GetAsync(string? pattern);
    }
}