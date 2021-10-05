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

namespace Brimborium.WebFlow.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public sealed class GnaController : ControllerBase, IGnaController {
        private readonly IServiceProvider _RequestServices;
        private readonly ILogger _Logger;

        public GnaController(
            IServiceProvider requestServices,
            ILogger<GnaController> logger
            ) {
            this._RequestServices = requestServices;
            this._Logger = logger;
        }

        [HttpGet("", Name = "Get")]
        public async Task<ActionResult<IEnumerable<Gna>>> GetAsync(string? pattern) {
            var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? this.HttpContext?.TraceIdentifier ?? "";
            try {
                this._Logger.LogInformation("GetAsync({parameter}) {traceId}", new { pattern }, traceId);
                IGnaServerAPI service = this._RequestServices.GetRequiredService<IGnaServerAPI>();
                System.Security.Claims.ClaimsPrincipal user = this.HttpContext?.User ?? new System.Security.Claims.ClaimsPrincipal();
                CancellationToken requestAborted = this.HttpContext?.RequestAborted ?? CancellationToken.None;
                var request = new GnaServerGetRequest(pattern, user);
                RequestResult<GnaServerGetResponse> response = await service.GetAsync(request, requestAborted);
                ActionResult<IEnumerable<Gna>> actionResult = this._RequestServices.GetRequiredService<IActionResultConverter>()
                    .ConvertToActionResultOfT<GnaServerGetResponse, IEnumerable<Gna>>(this, response);
                return actionResult;
            } catch (System.Exception error) {
                this._Logger.LogError(error, "GetAsync({parameter}) {traceId}", new { pattern }, traceId);
                throw;
            }
        }

        [HttpPost("", Name = "Post")]
        public async Task<ActionResult> PostAsync(
            Gna value
            ) {
            var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? this.HttpContext?.TraceIdentifier ?? "";
            try {
                this._Logger.LogInformation("PostAsync({parameter}) {traceId}", new { value }, traceId);
                IGnaServerAPI service = this._RequestServices.GetRequiredService<IGnaServerAPI>();
                System.Security.Claims.ClaimsPrincipal user = this.HttpContext?.User ?? new System.Security.Claims.ClaimsPrincipal();
                CancellationToken requestAborted = this.HttpContext?.RequestAborted ?? CancellationToken.None;
                GnaServerUpsertRequest request = new GnaServerUpsertRequest(value, user);
                RequestResult<GnaServerUpsertResponse> response = await service.UpsertAsync(request, requestAborted);
                ActionResult actionResult = this._RequestServices.GetRequiredService<IActionResultConverter>()
                    .ConvertToActionResultVoid(this, response);
                return actionResult;
            } catch (System.Exception error) {
                this._Logger.LogError(error, "PostAsync({parameter}) {traceId}", new { value }, traceId);
                throw;
            }
        }

        [HttpPost("{Name}", Name = "PostName")]
        public async Task<ActionResult> PostNameAsync(
            string Name,
            bool Done
            ) {
            var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? this.HttpContext?.TraceIdentifier ?? "";
            try {
                this._Logger.LogInformation("PostNameAsync({parameter}) {traceId}", new { Name, Done }, traceId);
                //var (context, user, cancellationToken, responseConverter) = this._RequestHandlerSupport.GetRequestHandlerRootContext(this);
                //var gnaService = new GnaService(context, user);
                //var request = new GnaUpsertRequest(Name, Done);
                //var response = await gnaService.GnaUpsertAsync(request, cancellationToken);
                //return responseConverter.ConvertToActionResult(this, response, (responseT) => new OkResult());

                IGnaServerAPI service = this._RequestServices.GetRequiredService<IGnaServerAPI>();
                System.Security.Claims.ClaimsPrincipal user = this.HttpContext?.User ?? new System.Security.Claims.ClaimsPrincipal();
                CancellationToken requestAborted = this.HttpContext?.RequestAborted ?? CancellationToken.None;
                GnaServerUpsertRequest request = new GnaServerUpsertRequest(new Gna(Name, Done), user);
                RequestResult<GnaServerUpsertResponse> response = await service.UpsertAsync(request, requestAborted);
                ActionResult actionResult = this._RequestServices.GetRequiredService<IActionResultConverter>()
                    .ConvertToActionResultVoid(this, response);
                return actionResult;
            } catch (System.Exception error) {
                this._Logger.LogError(error, "PostNameAsync({parameter}) {traceId}", new { Name, Done }, traceId);
                throw;
            }
        }
    }
}
