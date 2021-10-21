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
    public sealed class GnaController
        : Microsoft.AspNetCore.Mvc.ControllerBase
        , Demo.Controllers.IGnaController {
        private readonly System.IServiceProvider _RequestServices;
        
        private readonly Microsoft.Extensions.Logging.ILogger<Demo.Server.GnaController> _Logger;
        
        public GnaController(
            System.IServiceProvider requestServices,
            Microsoft.Extensions.Logging.ILogger<Demo.Server.GnaController> logger) {
            this._RequestServices = requestServices;
            this._Logger = logger;
        }
        
        
        public async System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.Generic.IEnumerable<Demo.API.Gna>>> GetAsync(
            System.String pattern
            ) {
            var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? this.HttpContext?.TraceIdentifier ?? "";
            try {
                this._Logger.LogInformation("GetAsync({parameter}) {traceId}", new { pattern }, traceId);
                IGnaServerAPI service = this._RequestServices.GetRequiredService<IGnaServerAPI>();
                System.Security.Claims.ClaimsPrincipal user = this.HttpContext?.User ?? new System.Security.Claims.ClaimsPrincipal();
                CancellationToken requestAborted = this.HttpContext?.RequestAborted ?? CancellationToken.None;
                var request = new Demo.Server.GnaServerGetRequest(
                    pattern,
                    user
                );
                Brimborium.CodeFlow.RequestHandler.RequestResult<Demo.Server.GnaServerGetResponse>response = await service.GetAsync(request, requestAborted);
                Microsoft.AspNetCore.Mvc.ActionResult<System.Collections.Generic.IEnumerable<Demo.API.Gna>> actionResult = this._RequestServices.GetRequiredService<IActionResultConverter>()
                    .ConvertToActionResultOfT<Demo.Server.GnaServerGetResponse, System.Collections.Generic.IEnumerable<Demo.API.Gna>>(this, response);
                return actionResult;
            } catch (System.Exception error) {
                this._Logger.LogError(error, "GetAsync({parameter}) {traceId}", new { pattern }, traceId);
                throw;
            }
        }
        
        public async System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult> PostAsync(
            Demo.API.Gna value
            ) {
            var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? this.HttpContext?.TraceIdentifier ?? "";
            try {
                this._Logger.LogInformation("PostAsync({parameter}) {traceId}", new { value }, traceId);
                IGnaServerAPI service = this._RequestServices.GetRequiredService<IGnaServerAPI>();
                System.Security.Claims.ClaimsPrincipal user = this.HttpContext?.User ?? new System.Security.Claims.ClaimsPrincipal();
                CancellationToken requestAborted = this.HttpContext?.RequestAborted ?? CancellationToken.None;
                var request = new Demo.Server.GnaServerPostRequest(
                    value,
                    user
                );
                Brimborium.CodeFlow.RequestHandler.RequestResult<Demo.Server.GnaServerPostResponse>response = await service.PostAsync(request, requestAborted);
                Microsoft.AspNetCore.Mvc.ActionResult actionResult = this._RequestServices.GetRequiredService<IActionResultConverter>()
                    .ConvertToActionResultVoid<Demo.Server.GnaServerPostResponse>(this, response);
                return actionResult;
            } catch (System.Exception error) {
                this._Logger.LogError(error, "PostAsync({parameter}) {traceId}", new { value }, traceId);
                throw;
            }
        }
        
        public async System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult> PostNameAsync(
            System.String Name,
            System.Boolean Done
            ) {
            var traceId = System.Diagnostics.Activity.Current?.TraceId.ToString() ?? this.HttpContext?.TraceIdentifier ?? "";
            try {
                this._Logger.LogInformation("PostNameAsync({parameter}) {traceId}", new { Name, Done }, traceId);
                IGnaServerAPI service = this._RequestServices.GetRequiredService<IGnaServerAPI>();
                System.Security.Claims.ClaimsPrincipal user = this.HttpContext?.User ?? new System.Security.Claims.ClaimsPrincipal();
                CancellationToken requestAborted = this.HttpContext?.RequestAborted ?? CancellationToken.None;
                var request = new Demo.Server.GnaServerPostNameRequest(
                    Name,
                    Done,
                    user
                );
                Brimborium.CodeFlow.RequestHandler.RequestResult<Demo.Server.GnaServerPostNameResponse>response = await service.PostNameAsync(request, requestAborted);
                Microsoft.AspNetCore.Mvc.ActionResult actionResult = this._RequestServices.GetRequiredService<IActionResultConverter>()
                    .ConvertToActionResultVoid<Demo.Server.GnaServerPostNameResponse>(this, response);
                return actionResult;
            } catch (System.Exception error) {
                this._Logger.LogError(error, "PostNameAsync({parameter}) {traceId}", new { Name, Done }, traceId);
                throw;
            }
        }
        
    }
}
