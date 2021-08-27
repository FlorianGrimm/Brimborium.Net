using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Routing;

namespace Brimborium.WebDavServer.AspNetCore.Routing
{
    /// <summary>
    /// The WebDAV HTTP PROPFIND method
    /// </summary>
    public class HttpPropFindAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> _supportedMethods = new[] { "PROPFIND" };

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpPropFindAttribute"/> class.
        /// </summary>
        public HttpPropFindAttribute()
            : base(_supportedMethods)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpPropFindAttribute"/> class.
        /// </summary>
        /// <param name="template">The route template. May not be null.</param>
        public HttpPropFindAttribute(string template)
            : base(_supportedMethods, template)
        {
            if (template == null) {
                throw new ArgumentNullException(nameof(template));
            }
        }
    }
}
