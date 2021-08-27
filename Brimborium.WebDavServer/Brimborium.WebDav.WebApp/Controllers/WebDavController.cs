using Brimborium.WebDavServer;
using Brimborium.WebDavServer.AspNetCore;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Brimborium.WebDav.WebApp.Controllers {
    [Route("webdav/{*path}")]
    [IgnoreAntiforgeryToken]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class WebDavController : WebDavControllerBase {
        public WebDavController(IWebDavContext context, IWebDavDispatcher dispatcher, ILogger<WebDavIndirectResult> responseLogger)
            : base(context, dispatcher, responseLogger) {
        }
    }

    //[Route("")]
    //[IgnoreAntiforgeryToken]
    //[Authorize]
    //[ApiExplorerSettings(IgnoreApi = true)]
    //public class WebDavRootController : Controller {
    //    //WebDavRootControllerBase {
    //    //public WebDavRootController(IWebDavContext context, IWebDavDispatcher dispatcher, ILogger<WebDavIndirectResult> responseLogger)
    //    //    : base(context, dispatcher, responseLogger) {
    //    //}
    //    [HttpGet]
    //    public IActionResult Get() { 
    //        return this.View
    //    }
    //}
}
