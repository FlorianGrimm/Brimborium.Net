#if soon
using Brimborium.CodeFlow.RequestHandler;
using Brimborium.WebFlow.Web.API;
using Brimborium.WebFlow.Web.Communication;
using Brimborium.WebFlow.Web.Model;
using Brimborium.WebFlow.WebLogic;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class GnaController : ControllerBase, IGnaController {
        private readonly IRequestHandlerSupport _RequestHandlerSupport;

        public GnaController(
            IRequestHandlerSupport requestHandlerSupport
            ) {
            this._RequestHandlerSupport = requestHandlerSupport;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gna>>> GetAsync(string? pattern) {
            var (context, user, cancellationToken, responseConverter) = this._RequestHandlerSupport.GetRequestHandlerRootContext(this);
            var gnaService = new GnaService(context, user);
            var request = new GnaQueryRequest(pattern ?? string.Empty);
            var response = await gnaService.GnaQueryAsync(request, cancellationToken);
            return responseConverter.ConvertToActionResultOfT(this, response, convertResponse);

            static IEnumerable<Gna> convertResponse(GnaQueryResponse response) {
                return response.Items.Select(i=>new Gna(i.Name, i.Done));
            }
        }

        [HttpPost("", Name = "Post")]
        public async Task<ActionResult> PostAsync(
            GnaModel value
            ) {
            var (context, user, cancellationToken, responseConverter) = this._RequestHandlerSupport.GetRequestHandlerRootContext(this);
            var gnaService = new GnaService(context, user);
            var request = new GnaUpsertRequest(value.Name, value.Done);
            var response = await gnaService.GnaUpsertAsync(request, cancellationToken);
            return responseConverter.ConvertToActionResult(this, response, (responseT) => new OkResult());
        }

        [HttpPost("{Name}", Name = "PostName")]
        public async Task<ActionResult> PostNameAsync(
            string Name,
            bool Done
            ) {
            var (context, user, cancellationToken, responseConverter) = this._RequestHandlerSupport.GetRequestHandlerRootContext(this);
            var gnaService = new GnaService(context, user);
            var request = new GnaUpsertRequest(Name, Done);
            var response = await gnaService.GnaUpsertAsync(request, cancellationToken);
            return responseConverter.ConvertToActionResult(this, response, (responseT) => new OkResult());
        }
    }
}
#endif