﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Props.Converters;

namespace Brimborium.WebDavServer.Props.Live
{
    /// <summary>
    /// The <c>getcontentlength</c> property
    /// </summary>
    public class ContentLengthProperty : ITypedReadableProperty<long>, ILiveProperty
    {
        /// <summary>
        /// The XML property name
        /// </summary>
        public static readonly XName PropertyName = WebDavXml.Dav + "getcontentlength";

        private static readonly LongConverter _converter = new LongConverter();

        private readonly long _propValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentLengthProperty"/> class.
        /// </summary>
        /// <param name="propValue">The initial value</param>
        public ContentLengthProperty(long propValue)
        {
            this._propValue = propValue;
        }

        /// <inheritdoc />
        public XName Name { get; } = PropertyName;

        /// <inheritdoc />
        public string? Language { get; } = null;

        /// <inheritdoc />
        public IReadOnlyCollection<XName> AlternativeNames { get; } = new[] { WebDavXml.Dav + "contentlength" };

        /// <inheritdoc />
        public int Cost { get; } = 0;

        /// <inheritdoc />
        public Task<bool> IsValidAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public async Task<XElement> GetXmlValueAsync(CancellationToken ct)
        {
            return _converter.ToElement(this.Name, await this.GetValueAsync(ct).ConfigureAwait(false));
        }

        /// <inheritdoc />
        public Task<long> GetValueAsync(CancellationToken ct)
        {
            return Task.FromResult(this._propValue);
        }
    }
}
