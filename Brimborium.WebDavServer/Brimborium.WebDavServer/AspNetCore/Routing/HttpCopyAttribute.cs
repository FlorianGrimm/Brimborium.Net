﻿using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Routing;

namespace Brimborium.WebDavServer.AspNetCore.Routing
{
    /// <summary>
    /// The WebDAV HTTP COPY method
    /// </summary>
    public class HttpCopyAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> _supportedMethods = new[] { "COPY" };

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCopyAttribute"/> class.
        /// </summary>
        public HttpCopyAttribute()
            : base(_supportedMethods)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCopyAttribute"/> class.
        /// </summary>
        /// <param name="template">The route template. May not be null.</param>
        public HttpCopyAttribute(string template)
            : base(_supportedMethods, template)
        {
            if (template == null) {
                throw new ArgumentNullException(nameof(template));
            }
        }
    }
}
