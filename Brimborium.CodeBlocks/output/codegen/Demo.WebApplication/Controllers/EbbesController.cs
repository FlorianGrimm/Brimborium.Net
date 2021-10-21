using Brimborium.CodeFlow.RequestHandler;

using Demo.API;
using Demo.Logic;
using Demo.Server;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Server {
    [ApiController]
    [Route("api/[controller]")]
    public sealed class EbbesController
        : Microsoft.AspNetCore.Mvc.ControllerBase
        , Demo.Controllers.IEbbesController {
        private readonly System.IServiceProvider _RequestServices;
        
        private readonly Microsoft.Extensions.Logging.ILogger<Demo.Server.EbbesController> _Logger;
        
        public EbbesController(
            System.IServiceProvider requestServices,
            Microsoft.Extensions.Logging.ILogger<Demo.Server.EbbesController> logger) {
            this._RequestServices = requestServices;
            this._Logger = logger;
        }
        
        
        public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.Generic.IEnumerable<Demo.API.Ebbes>>> GetAsync(
            System.String pattern
            ) {
            var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? this.HttpContext?.TraceIdentifier ?? "";
            try {
                this._Logger.LogInformation("GetAsync({parameter}) {traceId}", new { pattern }, traceId);
                IEbbesServerAPI service = this._RequestServices.GetRequiredService<IEbbesServerAPI>();
                System.Security.Claims.ClaimsPrincipal user = this.HttpContext?.User ?? new System.Security.Claims.ClaimsPrincipal();
                CancellationToken requestAborted = this.HttpContext?.RequestAborted ?? CancellationToken.None;
                var request = new Demo.Server.EbbesServerGetRequest(
                    pattern,
                    user
                );
                Brimborium.CodeFlow.RequestHandler.RequestResult<Demo.Server.EbbesServerGetResponse>response = await service.GetAsync(request, requestAborted);
                Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.Generic.IEnumerable<Demo.API.Ebbes>> actionResult = this._RequestServices.GetRequiredService<IActionResultConverter>()
                    .ConvertToActionResultOfT<Demo.Server.EbbesServerGetResponse, System.Collections.Generic.IEnumerable<Demo.API.Ebbes>>(this, response);
                return actionResult;
            } catch (System.Exception error) {
                this._Logger.LogError(error, "GetAsync({parameter}) {traceId}", new { pattern }, traceId);
                throw;
            }
        }
        
        public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult> UpsertAsync(
            Demo.API.Ebbes value
            ) {
            var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? this.HttpContext?.TraceIdentifier ?? "";
            try {
                this._Logger.LogInformation("UpsertAsync({parameter}) {traceId}", new { value }, traceId);
                IEbbesServerAPI service = this._RequestServices.GetRequiredService<IEbbesServerAPI>();
                System.Security.Claims.ClaimsPrincipal user = this.HttpContext?.User ?? new System.Security.Claims.ClaimsPrincipal();
                CancellationToken requestAborted = this.HttpContext?.RequestAborted ?? CancellationToken.None;
                var request = new Demo.Server.EbbesServerUpsertRequest(
                    value,
                    user
                );
                Brimborium.CodeFlow.RequestHandler.RequestResult<Demo.Server.EbbesServerUpsertResponse>response = await service.GetAsync(request, requestAborted);
                Microsoft.AspNetCore.Mvc.ActionResult actionResult = this._RequestServices.GetRequiredService<IActionResultConverter>()
                    .ConvertToActionResult<Demo.Server.EbbesServerUpsertResponse>(this, response);
                return actionResult;
            } catch (System.Exception error) {
                this._Logger.LogError(error, "UpsertAsync({parameter}) {traceId}", new { pattern }, traceId);
                throw;
            }
        }
        
    }
}
