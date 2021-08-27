using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Linq;

using Brimborium.WebDavServer.AspNetCore.Logging;

namespace Brimborium.WebDavServer.AspNetCore
{
    /// <summary>
    /// A <see cref="IWebDavResponse"/> implementation that buffers the output of a <see cref="IWebDavResult"/>
    /// </summary>
    public class LoggingWebDavResponse : IWebDavResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingWebDavResponse"/> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher implementation for the WebDAV server</param>
        public LoggingWebDavResponse(IWebDavDispatcher dispatcher)
        {
            this.Dispatcher = dispatcher;
            this.ContentType = "text/xml";
        }

        /// <inheritdoc />
        public IWebDavDispatcher Dispatcher { get; }

        /// <inheritdoc />
        public IDictionary<string, string[]> Headers { get; } = new Dictionary<string, string[]>();

        /// <inheritdoc />
        public string ContentType { get; set; }

        /// <inheritdoc />
        public Stream Body { get; } = new MemoryStream();

        /// <summary>
        /// Loads the <see cref="Body"/> into a <see cref="XDocument"/>
        /// </summary>
        /// <returns>The <see cref="XDocument"/> from the <see cref="Body"/></returns>
        public XDocument? Load()
        {
            this.Body.Position = 0;
            if (this.Body.Length == 0) {
                return null;
            }

            if (!RequestLogMiddleware.IsXml(this.ContentType)) {
                return null;
            }

            try
            {
                return XDocument.Load(this.Body);
            }
            catch
            {
                return null;
            }
        }
    }
}
