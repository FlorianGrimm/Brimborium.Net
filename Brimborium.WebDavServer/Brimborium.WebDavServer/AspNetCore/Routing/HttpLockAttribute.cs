﻿using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Routing;

namespace Brimborium.WebDavServer.AspNetCore.Routing
{
    /// <summary>
    /// The WebDAV HTTP LOCK method
    /// </summary>
    public class HttpLockAttribute : HttpMethodAttribute
    {
        private static readonly IEnumerable<string> _supportedMethods = new[] { "LOCK" };

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLockAttribute"/> class.
        /// </summary>
        public HttpLockAttribute()
            : base(_supportedMethods)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpLockAttribute"/> class.
        /// </summary>
        /// <param name="template">The route template. May not be null.</param>
        public HttpLockAttribute(string template)
            : base(_supportedMethods, template)
        {
            if (template == null) {
                throw new ArgumentNullException(nameof(template));
            }
        }
    }
}
