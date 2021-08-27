using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Model.Headers;
using Brimborium.WebDavServer.Props.Converters;
using Brimborium.WebDavServer.Props.Store;

namespace Brimborium.WebDavServer.Props.Dead {
    /// <summary>
    /// The implementation of the <c>getetag</c> property
    /// </summary>
    public class GetETagProperty : ITypedReadableProperty<EntityTag>, IDeadProperty {
        /// <summary>
        /// The XML name of the property
        /// </summary>
        public static readonly XName PropertyName = WebDavXml.Dav + "getetag";

        private readonly IPropertyStore? _PropertyStore;

        private readonly IEntry _Entry;

        private readonly IEntityTagEntry? _EtagEntry;

        private XElement? _Element;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetETagProperty"/> class.
        /// </summary>
        /// <param name="propertyStore">The property store to store this property</param>
        /// <param name="entry">The entry to instantiate this property for</param>
        /// <param name="cost">The cost of querying the display names property</param>
        public GetETagProperty(IPropertyStore? propertyStore, IEntry entry, int? cost = null) {
            this._PropertyStore = propertyStore;
            this._Entry = entry;
            this._EtagEntry = entry as IEntityTagEntry;
            this.Name = PropertyName;
            this.Cost = cost ?? (this._EtagEntry != null ? 0 : (int?)null) ?? this._PropertyStore?.Cost ?? 0;
        }

        /// <inheritdoc />
        public XName Name { get; }

        /// <inheritdoc />
        public string? Language { get; } = null;

        /// <inheritdoc />
        public IReadOnlyCollection<XName> AlternativeNames { get; } = new[] { WebDavXml.Dav + "etag" };

        /// <inheritdoc />
        public int Cost { get; }

        /// <summary>
        /// Gets the entity tag converter
        /// </summary>
        public IPropertyConverter<EntityTag> Converter { get; } = new EntityTagConverter();

        /// <inheritdoc />
        public async Task<XElement> GetXmlValueAsync(CancellationToken ct) {
            if (this._EtagEntry != null) {
                return this.Converter.ToElement(this.Name, this._EtagEntry.ETag);
            }

            if (this._Element == null) {
                if (this._PropertyStore != null) {
                    var etag = await this._PropertyStore.GetETagAsync(this._Entry, ct).ConfigureAwait(false);
                    this._Element = this.Converter.ToElement(this.Name, etag);
                } else {
                    this._Element = new EntityTag(false).ToXml();
                }
            }

            return this._Element;
        }

        /// <inheritdoc />
        public void Init(XElement initialValue) {
            this._Element = initialValue;
        }

        /// <inheritdoc />
        public async Task<EntityTag> GetValueAsync(CancellationToken ct) {
            return this.Converter.FromElement(await this.GetXmlValueAsync(ct).ConfigureAwait(false));
        }

        /// <inheritdoc />
        public bool IsDefaultValue(XElement element) {
            return false;
        }
    }
}
