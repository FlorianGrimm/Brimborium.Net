using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Brimborium.CodeFlow.RequestHandler;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace Brimborium.WebFlow.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class GnaController : ControllerBase {
        private readonly IScopeRequestHandlerFactory _RequestHandlerFactory;
        private readonly IRequestHandlerContextHolder _RequestHandlerContextHolder;

        public GnaController(
            IScopeRequestHandlerFactory requestHandlerFactory,
            IRequestHandlerContextHolder requestHandlerContextHolder
            ) {
            this._RequestHandlerFactory = requestHandlerFactory;
            this._RequestHandlerContextHolder = requestHandlerContextHolder;
        }

        [HttpGet]
        public IEnumerable<string> Get() {
            var context = this.GetRequestHandlerContext(this._RequestHandlerContextHolder);
            //var ctxt = this._RequestHandlerFactory.GetRequestHandlerRootContext
            return "a,b".Split(',');
        }
    }
}
