using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Props.Generic;
using Brimborium.WebDavServer.Props.Store;

namespace Brimborium.WebDavServer.Props.Dead {
    /// <summary>
    /// The <c>displayname</c> property
    /// </summary>
    public class DisplayNameProperty : GenericStringProperty, IDeadProperty {
        /// <summary>
        /// The XML name of the property
        /// </summary>
        public static readonly XName PropertyName = WebDavXml.Dav + "displayname";

        private readonly IEntry _Entry;

        private readonly IPropertyStore _Store;

        private readonly bool _HideExtension;

        private string? _Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayNameProperty"/> class.
        /// </summary>
        /// <param name="entry">The entry to instantiate this property for</param>
        /// <param name="store">The property store to store this property</param>
        /// <param name="hideExtension">Hide the extension from the display name</param>
        /// <param name="cost">The cost of querying the display names property</param>
        public DisplayNameProperty(IEntry entry, IPropertyStore store, bool hideExtension, int? cost = null)
            : base(PropertyName, null, cost ?? store.Cost, null, null) {
            this._Entry = entry;
            this._Store = store;
            this._HideExtension = hideExtension;
        }

        /// <inheritdoc />
        public override async Task<string> GetValueAsync(CancellationToken ct) {
            if (this._Value != null) {
                return this._Value;
            }

            if (this._Store != null) {
                var storedValue = await this._Store.GetAsync(this._Entry, this.Name, ct).ConfigureAwait(false);
                if (storedValue != null) {
                    this.Language = storedValue.Attribute(XNamespace.Xml + "lang")?.Value;
                    return this._Value = storedValue.Value;
                }
            }

            var newName = this._Value = this.GetDefaultName();
            return newName;
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
            return element == null
                   || (!element.HasAttributes && this.Converter.FromElement(element) == this.GetDefaultName());
        }

        private string GetDefaultName() {
            return this._HideExtension ? Path.GetFileNameWithoutExtension(this._Entry.Name) : this._Entry.Name;
        }
    }
}
