using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Props.Store;

namespace Brimborium.WebDavServer.Props.Dead {
    /// <summary>
    /// The generic dead property
    /// </summary>
    public class DeadProperty : IUntypedWriteableProperty, IDeadProperty {
        private readonly IPropertyStore _store;

        private readonly IEntry _entry;

        private XElement? _cachedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeadProperty"/> class.
        /// </summary>
        /// <param name="store">The property store for the dead properties</param>
        /// <param name="entry">The file system entry</param>
        /// <param name="name">The XML name of the dead property</param>
        public DeadProperty(IPropertyStore store, IEntry entry, XName name) {
            this.Name = name;
            this._store = store;
            this._entry = entry;
            this.Language = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeadProperty"/> class.
        /// </summary>
        /// <param name="store">The property store for the dead properties</param>
        /// <param name="entry">The file system entry</param>
        /// <param name="element">The element to intialize this property with</param>
        public DeadProperty(IPropertyStore store, IEntry entry, XElement element) {
            this._store = store;
            this._entry = entry;
            this.Name = element.Name;
            this._cachedValue = element;
            this.Language = element.Attribute(XNamespace.Xml + "lang")?.Value;
        }

        /// <inheritdoc />
        public XName Name { get; }

        /// <inheritdoc />
        public string? Language { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<XName> AlternativeNames { get; } = new XName[0];

        /// <inheritdoc />
        public int Cost => this._store.Cost;

        /// <inheritdoc />
        public Task SetXmlValueAsync(XElement element, CancellationToken ct) {
            this._cachedValue = element;
            return this._store.SetAsync(this._entry, element, ct);
        }

        /// <inheritdoc />
        public async Task<XElement> GetXmlValueAsync(CancellationToken ct) {
            XElement? result;
            if (this._cachedValue == null) {
                result = await this._store.GetAsync(this._entry, this.Name, ct).ConfigureAwait(false);
            } else {
                result = this._cachedValue;
            }

            if (result == null) {
                throw new InvalidOperationException("Cannot get value from uninitialized property");
            }

            return result;
        }

        /// <inheritdoc />
        public bool IsDefaultValue(XElement element) {
            return false;
        }

        /// <inheritdoc />
        public void Init(XElement initialValue) {
            var lang = initialValue.Attribute(XNamespace.Xml + "lang")?.Value;
            this.Language = lang;
            this._cachedValue = initialValue;
        }
    }
}
