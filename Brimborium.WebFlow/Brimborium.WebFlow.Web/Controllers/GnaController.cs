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
        private readonly IRequestHandlerContextBuilder _RequestHandlerContextBuilder;

        public GnaController(
            IRequestHandlerContextBuilder requestHandlerContextBuilder
            ) {
            this._RequestHandlerContextBuilder = requestHandlerContextBuilder;
        }

        [HttpGet]
        public async Task<IEnumerable<GnaModel>> GetAsync() {
            using (var context = this._RequestHandlerContextBuilder.GetRequestHandlerRootContext(this)) {
                var requestHandler = context.CreateRequestHandler<IGnaQueryRequestHandler>();
                //requestHandler.GetRequestHandlerTypeInfo()
                //var result2 = await context.CallRequestHandlerAsync<IGnaQueryRequestHandler>(new GnaQueryRequest(""));
                var result = await requestHandler.ExecuteAsync(new GnaQueryRequest(""), context, this.HttpContext.RequestAborted);
                return result.Items;
            }
        }
    }
}
