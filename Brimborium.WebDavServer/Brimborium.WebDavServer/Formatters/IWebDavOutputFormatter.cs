﻿using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Brimborium.WebDavServer.Formatters {
    /// <summary>
    /// A formatter for WebDAV responses
    /// </summary>
    public interface IWebDavOutputFormatter {
        /// <summary>
        /// Gets the content type
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Gets the encoding
        /// </summary>
        Encoding Encoding { get; }

        /// <summary>
        /// Serializes the <paramref name="data"/> to the <paramref name="output"/>
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="data"/></typeparam>
        /// <param name="output">The stream to serialize to</param>
        /// <param name="data">The data to serialize</param>
        Task SerializeAsync<T>(Stream output, T data);
    }
}
