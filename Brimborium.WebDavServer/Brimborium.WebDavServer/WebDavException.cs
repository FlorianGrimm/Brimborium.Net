using System;

using Brimborium.WebDavServer.Model;

namespace Brimborium.WebDavServer
{
    /// <summary>
    /// The WebDAV exception
    /// </summary>
    public class WebDavException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavException"/> class.
        /// </summary>
        /// <param name="statusCode">The WebDAV status code</param>
        public WebDavException(WebDavStatusCode statusCode)
            : base(statusCode.GetReasonPhrase())
        {
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavException"/> class.
        /// </summary>
        /// <param name="statusCode">The WebDAV status code</param>
        /// <param name="innerException">The inner exception</param>
        public WebDavException(WebDavStatusCode statusCode, Exception innerException)
            : base(statusCode.GetReasonPhrase(innerException.Message), innerException)
        {
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavException"/> class.
        /// </summary>
        /// <param name="statusCode">The WebDAV status code</param>
        /// <param name="responseMessage">The reason phrase for the status code</param>
        public WebDavException(WebDavStatusCode statusCode, string responseMessage)
            : base(statusCode.GetReasonPhrase(responseMessage))
        {
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Gets the WebDAV status code
        /// </summary>
        public WebDavStatusCode StatusCode { get; }
    }
}
