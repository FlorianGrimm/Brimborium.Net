using Demo.API;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.Controllers {
    public interface IGnaController {
        [HttpGet("", Name = "Get")]
        Task<ActionResult<IEnumerable<Gna>>> GetAsync(string? pattern);

        [HttpPost("", Name = "Post")]
        Task<ActionResult> PostAsync(Gna value);

        [HttpPost("{Name}", Name = "PostName")]
        Task<ActionResult> PostNameAsync(string Name, bool Done);
    }
}