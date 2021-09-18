using Brimborium.CodeFlow.RequestHandler;
using Brimborium.WebFlow.WebLogic;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class GnaController : ControllerBase {
        private readonly IRequestHandlerSupport _RequestHandlerSupport;

        public GnaController(
            IRequestHandlerSupport requestHandlerSupport
            ) {
            this._RequestHandlerSupport = requestHandlerSupport;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GnaModel>>> GetAsync(string? pattern) {
            var (context, user, cancellationToken, responseConverter) = this._RequestHandlerSupport.GetRequestHandlerRootContext(this);
            var requestHandler = context.CreateRequestHandler<IGnaQueryRequestHandler>();
            var request = new GnaQueryRequest(pattern ?? string.Empty, user);
            var response = await requestHandler.ExecuteAsync(request, context, cancellationToken);

            return responseConverter.ConvertToActionResultOfT(this, response, convertResponse);

            static IEnumerable<GnaModel> convertResponse(GnaQueryResponse response) {
                return response.Items;
            }
        }

        [HttpPost("", Name = "Post")]
        public async Task<ActionResult> PostAsync(
            GnaModel value
            ) {
            var (context, user, cancellationToken, responseConverter) = this._RequestHandlerSupport.GetRequestHandlerRootContext(this);
            var requestHandler = context.CreateRequestHandler<IGnaUpsertRequestHandler>();
            var request = new GnaUpsertRequest(value.Name, value.Done, user);
            var response = await requestHandler.ExecuteAsync(request, context, cancellationToken);

            return responseConverter.ConvertToActionResult(this, response, (responseT) => new OkResult());
        }

        [HttpPost("{Name}", Name = "PostName")]
        public async Task<ActionResult> PostNameAsync(
            string Name,
            bool Done
            ) {
            var (context, user, cancellationToken, responseConverter) = this._RequestHandlerSupport.GetRequestHandlerRootContext(this);
            var requestHandler = context.CreateRequestHandler<IGnaUpsertRequestHandler>();
            var request = new GnaUpsertRequest(Name, Done, user);
            var response = await requestHandler.ExecuteAsync(request, context, cancellationToken);

            return responseConverter.ConvertToActionResult(this, response, (responseT) => new OkResult());
        }

        //[NonAction]
        //private ActionResult ConvertGnaUpsertResponse(GnaUpsertResponse response) {
        //        return new OkResult();
        //}
    }
}
