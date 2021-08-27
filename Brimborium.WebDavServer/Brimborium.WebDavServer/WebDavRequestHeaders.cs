using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using Brimborium.WebDavServer.Model.Headers;

namespace Brimborium.WebDavServer {
    /// <summary>
    /// Implementation of the <see cref="IWebDavRequestHeaders"/> interface
    /// </summary>
    public class WebDavRequestHeaders : IWebDavRequestHeaders {
        private static readonly string[] _empty = new string[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavRequestHeaders"/> class.
        /// </summary>
        /// <param name="headers">The headers to parse</param>
        /// <param name="context">The WebDAV request context</param>
        public WebDavRequestHeaders(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers, IWebDavContext context) {
            this.Headers = headers.ToDictionary(x => x.Key, x => (IReadOnlyCollection<string>)x.Value.ToList(), StringComparer.OrdinalIgnoreCase);
            this.Depth = this.ParseHeader("Depth", args => DepthHeader.Parse(args.Single()), DepthHeader.Zero);
            this.Overwrite = this.ParseHeader("Overwrite", args => OverwriteHeader.Parse(args.Single()), null);
            this.Range = this.ParseHeader("Range", RangeHeader.Parse, (RangeHeader?)null);
            this.If = this.ParseHeader("If", args => IfHeader.Parse(args.Single(), EntityTagComparer.Strong, context), (IfHeader?)null);
            this.IfMatch = this.ParseHeader("If-Match", IfMatchHeader.Parse, (IfMatchHeader?)null);
            this.IfNoneMatch = this.ParseHeader("If-None-Match", IfNoneMatchHeader.Parse, (IfNoneMatchHeader?)null);
            this.IfModifiedSince = this.ParseHeader("If-Modified-Since", args => IfModifiedSinceHeader.Parse(args.Single()), (IfModifiedSinceHeader?)null);
            this.IfUnmodifiedSince = this.ParseHeader("If-Unmodified-Since", args => IfUnmodifiedSinceHeader.Parse(args.Single()), (IfUnmodifiedSinceHeader?)null);
            this.Timeout = this.ParseHeader("Timeout", TimeoutHeader.Parse, (TimeoutHeader?)null);
            this.ContentLength = this.ParseHeader("Content-Length", args => (long?)XmlConvert.ToInt64(args.Single()), (long?)null);
        }

        /// <inheritdoc />
        public long? ContentLength { get; }

        /// <inheritdoc />
        public DepthHeader? Depth { get; }

        /// <inheritdoc />
        public bool? Overwrite { get; }

        /// <inheritdoc />
        public IfHeader? If { get; }

        /// <inheritdoc />
        public IfMatchHeader? IfMatch { get; }

        /// <inheritdoc />
        public IfNoneMatchHeader? IfNoneMatch { get; }

        /// <inheritdoc />
        public IfModifiedSinceHeader? IfModifiedSince { get; }

        /// <inheritdoc />
        public IfUnmodifiedSinceHeader? IfUnmodifiedSince { get; }

        /// <inheritdoc />
        public RangeHeader? Range { get; }

        /// <inheritdoc />
        public TimeoutHeader? Timeout { get; }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Headers { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> this[string name] {
            get {
                if (this.Headers.TryGetValue(name, out var v)) {
                    return v;
                }

                return _empty;
            }
        }

        private T ParseHeader<T>(string name, Func<IReadOnlyCollection<string>, T> createFunc, T defaultValue) {
            if (this.Headers.TryGetValue(name, out var v)) {
                if (v.Count != 0) {
                    return createFunc(v);
                }
            }

            return defaultValue;
        }
    }
}
