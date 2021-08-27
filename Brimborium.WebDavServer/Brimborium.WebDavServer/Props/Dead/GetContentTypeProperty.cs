using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Props.Generic;
using Brimborium.WebDavServer.Props.Store;

namespace Brimborium.WebDavServer.Props.Dead {
    /// <summary>
    /// The implementation of the <c>getcontenttype</c> property
    /// </summary>
    public class GetContentTypeProperty : GenericStringProperty, IDeadProperty {
        /// <summary>
        /// The XML name of the property
        /// </summary>
        public static readonly XName PropertyName = WebDavXml.Dav + "getcontenttype";

        private readonly IMimeTypeDetector? _MimeTypeDetector;

        private readonly IEntry _Entry;

        private readonly IPropertyStore _Store;

        private string? _Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetContentTypeProperty"/> class.
        /// </summary>
        /// <param name="entry">The entry to instantiate this property for</param>
        /// <param name="store">The property store to store this property</param>
        /// <param name="mimeTypeDetector">The mime type detector</param>
        /// <param name="cost">The cost of querying the display names property</param>
        public GetContentTypeProperty(IEntry entry, IPropertyStore store, IMimeTypeDetector? mimeTypeDetector = null, int? cost = null)
            : base(PropertyName, null, cost ?? store.Cost, null, null, WebDavXml.Dav + "contenttype") {
            this._MimeTypeDetector = mimeTypeDetector;
            this._Entry = entry;
            this._Store = store;
        }

        /// <inheritdoc />
        public override async Task<string> GetValueAsync(CancellationToken ct) {
            if (this._Value != null) {
                return this._Value;
            }

            var storedValue = await this._Store.GetAsync(this._Entry, this.Name, ct).ConfigureAwait(false);
            if (storedValue != null) {
                this.Language = storedValue.Attribute(XNamespace.Xml + "lang")?.Value;
                return this._Value = storedValue.Value;
            }

            if (this._MimeTypeDetector == null || !this._MimeTypeDetector.TryDetect(this._Entry, out var mimeType)) {
                return Utils.MimeTypesMap.DefaultMimeType;
            }

            return mimeType;
        }

        /// <inheritdoc />
        public override async Task SetValueAsync(string value, CancellationToken ct) {
            this._Value = value;
            var element = await this.GetXmlValueAsync(ct).ConfigureAwait(false);
            await this._Store.SetAsync(this._Entry, element, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void Init(XElement initialValue) {
            this._Value = this.Converter.FromElement(initialValue);
        }

        /// <inheritdoc />
        public bool IsDefaultValue(XElement element) {
            return false;
        }
    }
}
