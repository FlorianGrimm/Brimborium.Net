using Brimborium.CodeFlow.RequestHandler;

using Demo.API;
using Demo.Server;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public sealed class EbbesController : ControllerBase, IEbbesController {
        private readonly IServiceProvider _RequestServices;
        private readonly ILogger _Logger;

        // nearly IEbbesAPI 
        public EbbesController(
            IServiceProvider requestServices,
            ILogger<EbbesController> logger
            ) {
            this._RequestServices = requestServices;
            this._Logger = logger;
        }

        [HttpGet("", Name = "GetAsync")]
        public async Task<ActionResult<IEnumerable<Ebbes>>> GetAsync(string? pattern) {
            var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? this.HttpContext?.TraceIdentifier ?? "";
            try {
                this._Logger.LogInformation("GetAsync({parameter}) {traceId}", new { pattern }, traceId);
                IEbbesServerAPI? service = this._RequestServices.GetRequiredService<IEbbesServerAPI>();
                System.Security.Claims.ClaimsPrincipal? user = this.HttpContext?.User ?? new System.Security.Claims.ClaimsPrincipal();
                CancellationToken requestAborted = this.HttpContext?.RequestAborted ?? CancellationToken.None;
                EbbesServerGetRequest request = new EbbesServerGetRequest(pattern, user);
                RequestResult<EbbesServerGetResponse> response = await service.GetAsync(request, requestAborted);
                ActionResult<IEnumerable<Ebbes>> actionResult = this._RequestServices.GetRequiredService<IActionResultConverter>()
                    .ConvertToActionResultOfT<EbbesServerGetResponse, IEnumerable<Ebbes>>(this, response);
                return actionResult;
            } catch (System.Exception error) {
                this._Logger.LogError(error, "GetAsync({parameter}) {traceId}", new { pattern }, traceId);
                throw;
            }
        }

        [HttpPost("", Name = "UpsertAsync")]
        public async Task<ActionResult> UpsertAsync(Ebbes value) {
            var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? this.HttpContext?.TraceIdentifier ?? "";
            try {
                this._Logger.LogInformation("EbbesUpsertAsync({parameter}) {traceId}", new { value }, traceId);
                IEbbesServerAPI? service = this._RequestServices.GetRequiredService<IEbbesServerAPI>();
                System.Security.Claims.ClaimsPrincipal? user = this.HttpContext?.User ?? new System.Security.Claims.ClaimsPrincipal();
                CancellationToken requestAborted = this.HttpContext?.RequestAborted ?? CancellationToken.None;
                RequestResult<EbbesServerUpsertResponse>? response = await service.UpsertAsync(new EbbesServerUpsertRequest(value, user), requestAborted);
                ActionResult actionResult = this._RequestServices.GetRequiredService<IActionResultConverter>()
                    .ConvertToActionResultVoid<EbbesServerUpsertResponse>(this, response);
                return actionResult;
            } catch (System.Exception error) {
                this._Logger.LogError(error, "EbbesUpsertAsync({parameter}) {traceId}", new { value }, traceId);
                throw;
            }
        }
    }
}
