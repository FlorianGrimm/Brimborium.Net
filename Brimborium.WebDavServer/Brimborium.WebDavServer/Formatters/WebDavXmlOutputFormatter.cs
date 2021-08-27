using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

using Brimborium.WebDavServer.Model;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Brimborium.WebDavServer.Formatters {
    /// <summary>
    /// The default implementation of the <see cref="IWebDavOutputFormatter"/> interface
    /// </summary>
    public class WebDavXmlOutputFormatter : IWebDavOutputFormatter {
        private static readonly Encoding _defaultEncoding = new UTF8Encoding(false);

        private readonly ILogger<WebDavXmlOutputFormatter> _logger;

        private readonly string _namespacePrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavXmlOutputFormatter"/> class.
        /// </summary>
        /// <param name="options">The formatter options</param>
        /// <param name="logger">The logger</param>
        public WebDavXmlOutputFormatter(IOptions<WebDavFormatterOptions> options, ILogger<WebDavXmlOutputFormatter> logger) {
            this._logger = logger;
            this.Encoding = _defaultEncoding;

            var contentType = options.Value.ContentType ?? "text/xml";
            this.ContentType = $"{contentType}; charset=\"{this.Encoding.WebName}\"";

            this._namespacePrefix = options.Value.NamespacePrefix;
        }

        /// <inheritdoc />
        public string ContentType { get; }

        /// <inheritdoc />
        public Encoding Encoding { get; }

        /// <inheritdoc />
        public async Task SerializeAsync<T>(Stream output, T data) {
            var writerSettings = new XmlWriterSettings { Encoding = Encoding };

            var ns = new XmlSerializerNamespaces();
            if (!string.IsNullOrEmpty(this._namespacePrefix)) {
                ns.Add(this._namespacePrefix, WebDavXml.Dav.NamespaceName);

                var xelem = data as XElement;
                if (xelem != null && xelem.GetPrefixOfNamespace(WebDavXml.Dav) != this._namespacePrefix) {
                    xelem.SetAttributeValue(XNamespace.Xmlns + this._namespacePrefix, WebDavXml.Dav.NamespaceName);
                }
            }

            if (this._logger.IsEnabled(LogLevel.Debug)) {
                var debugOutput = new StringWriter();
                SerializerInstance<T>.Serializer.Serialize(debugOutput, data, ns);
                this._logger.LogDebug(debugOutput.ToString());
            }
            var ms = new MemoryStream();
            using (var writer = XmlWriter.Create(ms, writerSettings)) {
                SerializerInstance<T>.Serializer.Serialize(writer, data, ns);
                writer.Flush();
            }
            ms.Position = 0;
            await ms.CopyToAsync(output);
        }

        private static class SerializerInstance<T> {
            public static readonly XmlSerializer Serializer = new XmlSerializer(typeof(T));
        }
    }
}
