using Demo.API;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.Controllers {
    public interface IEbbesController {
        [HttpGet("", Name = "GetAsync")]
        Task<ActionResult<IEnumerable<Ebbes>>> GetAsync(string? pattern);

        [HttpPost("", Name = "UpsertAsync")]
        Task<ActionResult> UpsertAsync(Ebbes value);

    }
}