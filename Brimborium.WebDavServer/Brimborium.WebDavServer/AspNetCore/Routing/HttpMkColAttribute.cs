using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Routing;

namespace Brimborium.WebDavServer.AspNetCore.Routing
{
    /// <summary>
    /// The WebDAV HTTP MKCOL method
    /// </summary>
    public class HttpMkColAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> _supportedMethods = new[] { "MKCOL" };

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMkColAttribute"/> class.
        /// </summary>
        public HttpMkColAttribute()
            : base(_supportedMethods)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMkColAttribute"/> class.
        /// </summary>
        /// <param name="template">The route template. May not be null.</param>
        public HttpMkColAttribute(string template)
            : base(_supportedMethods, template)
        {
            if (template == null) {
                throw new ArgumentNullException(nameof(template));
            }
        }
    }
}
