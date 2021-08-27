using Brimborium.WebDavServer.AspNetCore.Routing;
using Brimborium.WebDavServer.Model;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebDavServer.AspNetCore {
    /// <summary>
    /// The default WebDAV controller
    /// </summary>
    public class WebDavRootControllerBase : ControllerBase {
        private readonly IWebDavContext _context;
        private readonly IWebDavDispatcher _dispatcher;
        private readonly ILogger<WebDavIndirectResult> _responseLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavControllerBase"/> class.
        /// </summary>
        /// <param name="context">The WebDAV request context</param>
        /// <param name="dispatcher">The WebDAV HTTP method dispatcher</param>
        /// <param name="responseLogger">The logger for the <see cref="WebDavIndirectResult"/></param>
        public WebDavRootControllerBase(IWebDavContext context, IWebDavDispatcher dispatcher, ILogger<WebDavIndirectResult> responseLogger) {
            this._context = context;
            this._dispatcher = dispatcher;
            this._responseLogger = responseLogger;
        }

        /// <summary>
        /// Handler for the <c>PROPFIND</c> method
        /// </summary>
        /// <param name="path">The root-relative path to the target of this method</param>
        /// <param name="request">The <see cref="propfind"/> request element (may be null)</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The action result</returns>
        [HttpPropFind]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> PropFindAsync(
            string path,
            [FromBody] propfind request,
            CancellationToken cancellationToken = default(CancellationToken)) {
            await Task.CompletedTask;
#warning handle needed!? use the mount points?
            return this.BadRequest();
            //return new WebDavIndirectResult(this._dispatcher, result, this._responseLogger);
        }

        /// <summary>
        /// Handler for the <c>LOCK</c> method
        /// </summary>
        /// <param name="path">The root-relative path to the target of this method</param>
        /// <param name="lockinfo">The information about the requested lock</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The action result</returns>
        [HttpLock]
        public IActionResult Lock(
            string path,
            [FromBody] lockinfo lockinfo,
            CancellationToken cancellationToken = default(CancellationToken)) {
            return this.BadRequest();
        }

        /// <summary>
        /// Handler for the <c>UNLOCK</c> method
        /// </summary>
        /// <param name="path">The root-relative path to the target of this method</param>
        /// <param name="lockToken">The token of the lock to remove</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The action result</returns>
        [HttpUnlock]
        public IActionResult Unlock(
            string path,
            [FromHeader(Name = "Lock-Token")] string lockToken,
            CancellationToken cancellationToken = default(CancellationToken)) {
            return this.BadRequest();
        }
    }
}
