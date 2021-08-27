using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Props.Generic;
using Brimborium.WebDavServer.Props.Store;

namespace Brimborium.WebDavServer.Props.Dead
{
    /// <summary>
    /// The implementation of the <c>getcontentlanguage</c> property
    /// </summary>
    public class GetContentLanguageProperty : GenericStringProperty, IDeadProperty
    {
        /// <summary>
        /// The XML name of the property
        /// </summary>
        public static readonly XName PropertyName = WebDavXml.Dav + "getcontentlanguage";

        private readonly IEntry _entry;

        private readonly IPropertyStore _store;

        private readonly string _defaultContentLanguage;

        private string? _value;

        private bool _isLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetContentLanguageProperty"/> class.
        /// </summary>
        /// <param name="entry">The entry to instantiate this property for</param>
        /// <param name="store">The property store to store this property</param>
        /// <param name="defaultContentLanguage">The content language to return when none was specified</param>
        /// <param name="cost">The cost of querying the display names property</param>
        public GetContentLanguageProperty(IEntry entry, IPropertyStore store, string defaultContentLanguage = "en", int? cost = null)
            : base(PropertyName, null, cost ?? store.Cost, null, null, WebDavXml.Dav + "contentlanguage")
        {
            this._entry = entry;
            this._store = store;
            this._defaultContentLanguage = defaultContentLanguage;
        }

        /// <summary>
        /// Tries to get the value of this property
        /// </summary>
        /// <param name="ct">The cancellation token</param>
        /// <returns>A tuple where the first item indicates whether the value was read from the property store and
        /// the second item is the value to be returned as value for this property</returns>
        public async Task<ValueTuple<bool, string>> TryGetValueAsync(CancellationToken ct)
        {
            var result = await this.GetValueAsync(ct).ConfigureAwait(false);
            return ValueTuple.Create(this._value != null, result);
        }

        /// <inheritdoc />
        public override async Task<string> GetValueAsync(CancellationToken ct)
        {
            if (this._value != null || this._isLoaded) {
                return this._value ?? this._defaultContentLanguage;
            }

            var storedValue = await this._store.GetAsync(this._entry, this.Name, ct).ConfigureAwait(false);
            if (storedValue != null)
            {
                this.Language = storedValue.Attribute(XNamespace.Xml + "lang")?.Value;
                return this._value = storedValue.Value;
            }

            this._isLoaded = true;
            return this._value ?? this._defaultContentLanguage;
        }

        /// <inheritdoc />
        public override async Task SetValueAsync(string value, CancellationToken ct)
        {
            this._value = value;
            var element = await this.GetXmlValueAsync(ct).ConfigureAwait(false);
            await this._store.SetAsync(this._entry, element, ct).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void Init(XElement initialValue)
        {
            this._value = this.Converter.FromElement(initialValue);
        }

        /// <inheritdoc />
        public bool IsDefaultValue(XElement element)
        {
            return false;
        }
    }
}
