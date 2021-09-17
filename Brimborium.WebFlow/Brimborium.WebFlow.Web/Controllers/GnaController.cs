using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Brimborium.CodeFlow.RequestHandler;
using Brimborium.WebFlow.WebLogic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Brimborium.WebFlow.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class GnaController : ControllerBase {
        private readonly IRequestHandlerSupport _RequestHandlerSupport;

        public GnaController(
            IRequestHandlerSupport requestHandlerSupport
            ) {
            this._RequestHandlerSupport = requestHandlerSupport;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GnaModel>>> GetAsync(string? pattern) {
            var (context, cancellationToken, responseConverter) = this._RequestHandlerSupport.GetRequestHandlerRootContext(this);
            var requestHandler = context.CreateRequestHandler<IGnaQueryRequestHandler>();
            var request = new GnaQueryRequest(pattern ?? string.Empty);
            var response = await requestHandler.ExecuteAsync(request, context, cancellationToken);

            return responseConverter.Convert(this, response, convertResponse);
            //return this.ConvertToActionResult(response, convertResponse);

            static IEnumerable<GnaModel> convertResponse(GnaQueryResponse response) {
                return response.Items;
            }

            //return this.ConvertToActionResult<GnaQueryResponse, IEnumerable<GnaModel>>(response, (r)=>r.Items);
            //return this.ConvertToActionResult(response, (r)=>(IEnumerable<GnaModel>)r.Items);

            //if (response.TryGetValue(out var value) {
            //    return value.Items;
            //} else {
            //    if (response.Result is RequestResultOK requestResultOk) {
            //        if (requestResultOk.Value is GnaQueryResponse responseValue) {
            //            return responseValue.Items;
            //        }
            //    }
            //    if (response.Result is RequestResultFailed requestResultFailed) {
            //        if (requestResultFailed.Exception is not null && requestResultFailed.Status == -1) { 
            //            throw requestResultFailed.Exception;
            //        }
            //        return this.Problem(
            //            detail: requestResultFailed.Message,
            //            statusCode: requestResultFailed.Status,
            //            title: requestResultFailed.Scope
            //            );
            //    }
            //}
        }
    }
}
