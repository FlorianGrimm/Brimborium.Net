using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.Model;

using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;

namespace Brimborium.WebDavServer.AspNetCore
{
    /// <summary>
    /// An <see cref="IActionResult"/> implementation that takes a <see cref="IWebDavResult"/>
    /// </summary>
    public class WebDavIndirectResult : StatusCodeResult
    {
        private static readonly IEnumerable<MediaType> _supportedMediaTypes = new[] { "text/xml", "application/xml" }.Select(x => new MediaType(x)).ToList();

        private readonly IWebDavDispatcher _dispatcher;

        private readonly IWebDavResult _result;

        private readonly ILogger<WebDavIndirectResult>? _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavIndirectResult"/> class.
        /// </summary>
        /// <param name="dispatcher">The WebDAV HTTP method dispatcher</param>
        /// <param name="result">The result of the WebDAV operation</param>
        /// <param name="logger">The logger for a <see cref="WebDavIndirectResult"/></param>
        public WebDavIndirectResult(IWebDavDispatcher dispatcher, IWebDavResult result, ILogger<WebDavIndirectResult>? logger)
            : base((int)result.StatusCode)
        {
            this._dispatcher = dispatcher;
            this._result = result;
            this._logger = logger;
        }

        /// <inheritdoc />
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;

            // Sets the HTTP status code
            await base.ExecuteResultAsync(context).ConfigureAwait(false);

            // Sets the reason phrase
            var responseFeature = context.HttpContext.Features.Get<IHttpResponseFeature>();
            if (responseFeature != null) {
                responseFeature.ReasonPhrase = this._result.StatusCode.GetReasonPhrase();
            }

            if (this._logger?.IsEnabled(LogLevel.Debug) ?? false)
            {
                var loggingResponse = new LoggingWebDavResponse(this._dispatcher);
                await this._result.ExecuteResultAsync(loggingResponse, context.HttpContext.RequestAborted).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(loggingResponse.ContentType))
                {
                    var mediaType = new MediaType(loggingResponse.ContentType);
                    if (_supportedMediaTypes.Any(x => mediaType.IsSubsetOf(x)))
                    {
                        var doc = loggingResponse.Load();
                        if (doc != null) {
                            this._logger.LogDebug(doc.ToString(SaveOptions.OmitDuplicateNamespaces));
                        }
                    }
                }
            }

            // Writes the XML response
            await this._result.ExecuteResultAsync(new WebDavResponse(this._dispatcher, response), context.HttpContext.RequestAborted).ConfigureAwait(false);
        }
    }
}
