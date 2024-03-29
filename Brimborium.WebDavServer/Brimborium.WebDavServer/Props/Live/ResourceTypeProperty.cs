﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.Model;

namespace Brimborium.WebDavServer.Props.Live {
    /// <summary>
    /// The <c>resourcetype</c> property
    /// </summary>
    public abstract class ResourceTypeProperty : ILiveProperty {
        /// <summary>
        /// Gets the XML property name
        /// </summary>
        public static readonly XName PropertyName = WebDavXml.Dav + "resourcetype";

        private readonly XElement? _element;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceTypeProperty"/> class.
        /// </summary>
        /// <param name="element">The element of this resource type</param>
        protected ResourceTypeProperty(XElement? element) {
            this._element = element;
        }

        /// <inheritdoc />
        public XName Name { get; } = PropertyName;

        /// <inheritdoc />
        public string? Language { get; } = null;

        /// <inheritdoc />
        public IReadOnlyCollection<XName> AlternativeNames { get; } = new XName[0];

        /// <inheritdoc />
        public int Cost { get; } = 0;

        /// <summary>
        /// Returns a new document resource type property
        /// </summary>
        /// <returns>a new document resource type property</returns>
        public static ResourceTypeProperty GetDocumentResourceType()
            => new DocumentResourceType();

        /// <summary>
        /// Returns a new collection resource type property
        /// </summary>
        /// <returns>a new collection resource type property</returns>
        public static ResourceTypeProperty GetCollectionResourceType()
            => new CollectionResourceType();

        /// <inheritdoc />
        public Task<bool> IsValidAsync(CancellationToken cancellationToken) {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task<XElement> GetXmlValueAsync(CancellationToken ct) {
            var result = new XElement(this.Name);
            if (this._element != null) {
                result.Add(this._element);
            }

            return Task.FromResult(result);
        }

        private class DocumentResourceType : ResourceTypeProperty {
            public DocumentResourceType()
                : base(null) {
            }
        }

        private class CollectionResourceType : ResourceTypeProperty {
            public CollectionResourceType()
                : base(new XElement(WebDavXml.Dav + "collection")) {
            }
        }
    }
}
