using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Props.Dead;
using Brimborium.WebDavServer.Props.Live;
using Brimborium.WebDavServer.Props.Store;

namespace Brimborium.WebDavServer.Props {
    /// <summary>
    /// The asynchronously enumerable properties for a <see cref="IEntry"/>
    /// </summary>
    public class EntryProperties : IAsyncEnumerable<IUntypedReadableProperty> {
        private readonly IEntry _entry;

        private readonly IEnumerable<IUntypedReadableProperty> _predefinedProperties;

        private readonly IPropertyStore? _propertyStore;

        private readonly int? _maxCost;

        private readonly bool _returnInvalidProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryProperties"/> class.
        /// </summary>
        /// <param name="entry">The entry whose properties are to enumerate</param>
        /// <param name="predefinedProperties">The predefined properties for the entry</param>
        /// <param name="propertyStore">The property store to get the remaining dead properties for</param>
        /// <param name="maxCost">The maximum cost of the properties to return</param>
        /// <param name="returnInvalidProperties">Do we want to get invalid live properties?</param>
        public EntryProperties(
            IEntry entry,
            IEnumerable<IUntypedReadableProperty> predefinedProperties,
            IPropertyStore? propertyStore,
            int? maxCost,
            bool returnInvalidProperties) {
            this._entry = entry;
            this._predefinedProperties = predefinedProperties;
            this._propertyStore = propertyStore;
            this._maxCost = maxCost;
            this._returnInvalidProperties = returnInvalidProperties;
        }

        public IAsyncEnumerator<IUntypedReadableProperty> GetAsyncEnumerator(CancellationToken cancellationToken = default) {
            return new PropertiesEnumerator(this._entry, this._predefinedProperties, this._propertyStore, this._maxCost, this._returnInvalidProperties, cancellationToken);
        }

        /// <inheritdoc />
        public IAsyncEnumerator<IUntypedReadableProperty> GetEnumerator() {
            return new PropertiesEnumerator(this._entry, this._predefinedProperties, this._propertyStore, this._maxCost, this._returnInvalidProperties, CancellationToken.None);
        }

        private class PropertiesEnumerator : IAsyncEnumerator<IUntypedReadableProperty> {
            private readonly IEntry _entry;

            private readonly IPropertyStore? _propertyStore;

            private readonly int? _maxCost;

            private readonly bool _returnInvalidProperties;
            private readonly CancellationToken _CancellationToken;
            private readonly IEnumerator<IUntypedReadableProperty> _predefinedPropertiesEnumerator;

            private readonly Dictionary<XName, IUntypedReadableProperty> _emittedProperties = new Dictionary<XName, IUntypedReadableProperty>();

            private bool _predefinedPropertiesFinished;

            private IEnumerator<IDeadProperty>? _deadPropertiesEnumerator;

            private IUntypedReadableProperty? _Current;

            public PropertiesEnumerator(
                IEntry entry,
                IEnumerable<IUntypedReadableProperty> predefinedProperties,
                IPropertyStore? propertyStore,
                int? maxCost,
                bool returnInvalidProperties,
                CancellationToken cancellationToken) {
                this._entry = entry;
                this._propertyStore = propertyStore;
                this._maxCost = maxCost;
                this._returnInvalidProperties = returnInvalidProperties;
                this._CancellationToken = cancellationToken;
                var emittedProperties = new HashSet<XName>();
                var predefinedPropertiesList = new List<IUntypedReadableProperty>();
                foreach (var property in predefinedProperties) {
                    if (emittedProperties.Add(property.Name)) {
                        predefinedPropertiesList.Add(property);
                    }
                }

                this._predefinedPropertiesEnumerator = predefinedPropertiesList.GetEnumerator();
            }

            public IUntypedReadableProperty Current => this._Current ?? throw new ArgumentOutOfRangeException();


            public async ValueTask<bool> MoveNextAsync() {
                while (true) {
                    var result = await this.GetNextPropertyAsync(_CancellationToken).ConfigureAwait(false);
                    if (result == null) {
                        this._Current = null;
                        return false;
                    }

                    IUntypedReadableProperty? oldProperty;
                    if (this._emittedProperties.TryGetValue(result.Name, out oldProperty)) {
                        // Property was already emitted - don't return it again.
                        // The predefined dead properties are reading their values from the property store
                        // themself and don't need to be intialized again.
                        continue;
                    }

                    if (!this._returnInvalidProperties) {
                        var liveProp = result as ILiveProperty;
                        if (liveProp != null && !await liveProp.IsValidAsync(_CancellationToken).ConfigureAwait(false)) {
                            // The properties value is not valid
                            continue;
                        }
                    }

                    this._emittedProperties.Add(result.Name, result);
                    this._Current = result;
                    return true;
                }
            }

            public void Dispose() {
                this._predefinedPropertiesEnumerator.Dispose();
                this._deadPropertiesEnumerator?.Dispose();
            }

            public ValueTask DisposeAsync() {
                this._predefinedPropertiesEnumerator.Dispose();
                this._deadPropertiesEnumerator?.Dispose();
                return ValueTask.CompletedTask;
            }

            private async Task<IUntypedReadableProperty?> GetNextPropertyAsync(CancellationToken cancellationToken) {
                if (!this._predefinedPropertiesFinished) {
                    if (this._predefinedPropertiesEnumerator.MoveNext()) {
                        return this._predefinedPropertiesEnumerator.Current;
                    }

                    this._predefinedPropertiesFinished = true;

                    if (this._propertyStore == null || (this._maxCost.HasValue && this._propertyStore.Cost > this._maxCost)) {
                        return null;
                    }

                    var deadProperties = await this._propertyStore.LoadAsync(this._entry, cancellationToken).ConfigureAwait(false);
                    this._deadPropertiesEnumerator = deadProperties.GetEnumerator();
                }

                if (this._propertyStore == null || (this._maxCost.HasValue && this._propertyStore.Cost > this._maxCost)) {
                    return null;
                }

                Debug.Assert(this._deadPropertiesEnumerator != null, "_deadPropertiesEnumerator != null");
                if (this._deadPropertiesEnumerator == null) {
                    throw new InvalidOperationException("Internal error: The dead properties enumerator was not initialized");
                }

                if (!this._deadPropertiesEnumerator.MoveNext()) {
                    return null;
                }

                return this._deadPropertiesEnumerator.Current;
            }
        }
    }
}
