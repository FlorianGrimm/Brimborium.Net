using System;
using System.Collections.Generic;

namespace Brimborium.WebDavServer.Engines.Remote
{
    /// <summary>
    /// The exception for a failed remote operation
    /// </summary>
    public class RemoteTargetException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteTargetException"/> class.
        /// </summary>
        public RemoteTargetException()
        {
            this.Href = new Uri[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteTargetException"/> class.
        /// </summary>
        /// <param name="message">The error message</param>
        public RemoteTargetException(string message)
            : base(message)
        {
            this.Href = new Uri[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteTargetException"/> class.
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="innerException">The inner exception</param>
        public RemoteTargetException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.Href = new Uri[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteTargetException"/> class.
        /// </summary>
        /// <param name="href">The <c>href</c> of the failed operation</param>
        public RemoteTargetException(IReadOnlyCollection<Uri> href)
        {
            this.Href = href;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteTargetException"/> class.
        /// </summary>
        /// <param name="href">The <c>href</c>s of the failed operation</param>
        public RemoteTargetException(params Uri[] href)
        {
            this.Href = href;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteTargetException"/> class.
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="href">The <c>href</c>s of the failed operation</param>
        public RemoteTargetException(string message, IReadOnlyCollection<Uri> href)
            : base(message)
        {
            this.Href = href;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteTargetException"/> class.
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="href">The <c>href</c>s of the failed operation</param>
        public RemoteTargetException(string message, params Uri[] href)
            : base(message)
        {
            this.Href = href;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteTargetException"/> class.
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="href">The <c>href</c>s of the failed operation</param>
        /// <param name="innerException">The inner exception</param>
        public RemoteTargetException(string message, IReadOnlyCollection<Uri> href, Exception innerException)
            : base(message, innerException)
        {
            this.Href = href;
        }

        /// <summary>
        /// Gets the <c>href</c>s of the failed operation
        /// </summary>
        public IReadOnlyCollection<Uri> Href { get; }
    }
}
