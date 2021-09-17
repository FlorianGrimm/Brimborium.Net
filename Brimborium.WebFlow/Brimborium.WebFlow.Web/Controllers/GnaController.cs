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

            return responseConverter.ConvertTyped(this, response, convertResponse);

            static IEnumerable<GnaModel> convertResponse(GnaQueryResponse response) {
                return response.Items;
            }
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GnaModel value) {
            var (context, cancellationToken, responseConverter) = this._RequestHandlerSupport.GetRequestHandlerRootContext(this);
            var requestHandler = context.CreateRequestHandler<IGnaUpsertRequestHandler>();
            var request = new GnaUpsertRequest(value);
            var response = await requestHandler.ExecuteAsync(request, context, cancellationToken);

            return responseConverter.Convert(this, response, convertResponse);

            static ActionResult convertResponse(GnaUpsertResponse response) {
                return response.Items;
            }
        }
    }
}
