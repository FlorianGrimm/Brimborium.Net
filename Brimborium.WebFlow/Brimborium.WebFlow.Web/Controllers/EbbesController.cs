#if soon
using Brimborium.CodeFlow.RequestHandler;
using Brimborium.WebFlow.Web.API;
using Brimborium.WebFlow.WebLogic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class EbbesController : ControllerBase, IEbbesController        {
        private readonly ILogger _Logger;

        // nearly IEbbesAPI 
        public EbbesController(ILogger<EbbesController> logger) {
            this._Logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ebbes>>> GetAsync(string? pattern) {
            var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? this.HttpContext.TraceIdentifier;
            this._Logger.LogInformation("GetAsync({parameter}) {traceId}", new { pattern }, traceId);
            try {
                var service = this.HttpContext.RequestServices.GetRequiredService<IEbbesServerAPI>();
                var response = await service.GetAsync(pattern, this.HttpContext.User, this.HttpContext.RequestAborted);
                var actionResult = this.HttpContext.RequestServices.GetRequiredService<IActionResultConverter>()
                    .ConvertToActionResultOfT(this, response);
                return actionResult;
            } catch (System.Exception) {
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> EbbesUpsertAsync(Ebbes value) {
            var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? this.HttpContext.TraceIdentifier;
            this._Logger.LogInformation("EbbesUpsertAsync({parameter}) {traceId}", new { value }, traceId);
            try {
                var service = this.HttpContext.RequestServices.GetRequiredService<IEbbesServerAPI>();
                var response = await service.EbbesUpsertAsync(value, this.HttpContext.User, this.HttpContext.RequestAborted);
                var actionResult = this.HttpContext.RequestServices.GetRequiredService<IActionResultConverter>()
                    .ConvertToActionResult(this, response);
                return actionResult;
            } catch (System.Exception) {
                this._Logger.LogError
                throw;
            }
        }
    }
}
#endif