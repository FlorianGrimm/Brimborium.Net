using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Model.Headers;
using Brimborium.WebDavServer.Props.Dead;

using Microsoft.Extensions.Logging;

namespace Brimborium.WebDavServer.Props.Store.InMemory {
    /// <summary>
    /// The in-memory implementation of a property store
    /// </summary>
    public class InMemoryPropertyStore : PropertyStoreBase {
        private readonly ILogger<InMemoryPropertyStore> _logger;
        private readonly IDictionary<Uri, IDictionary<XName, XElement>> _properties = new Dictionary<Uri, IDictionary<XName, XElement>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryPropertyStore"/> class.
        /// </summary>
        /// <param name="deadPropertyFactory">The factory to create dead properties</param>
        /// <param name="logger">The logger</param>
        public InMemoryPropertyStore(IDeadPropertyFactory deadPropertyFactory, ILogger<InMemoryPropertyStore> logger)
            : base(deadPropertyFactory) {
            this._logger = logger;
        }

        /// <inheritdoc />
        public override int Cost { get; } = 0;

        /// <inheritdoc />
        public override Task<IReadOnlyCollection<XElement>> GetAsync(IEntry entry, CancellationToken cancellationToken) {
            var entries = this.GetAll(entry)
                .Where(x => x.Name != GetETagProperty.PropertyName)
                .ToList();
            return Task.FromResult<IReadOnlyCollection<XElement>>(entries);
        }

        /// <inheritdoc />
        public override Task SetAsync(IEntry entry, IEnumerable<XElement> elements, CancellationToken cancellationToken) {
            var elementsToSet = new List<XElement>();
            foreach (var element in elements) {
                if (element.Name == GetETagProperty.PropertyName) {
                    this._logger.LogWarning("The ETag property must not be set using the property store.");
                    continue;
                }

                elementsToSet.Add(element);
            }

            this.SetAll(entry, elementsToSet);

            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public override Task RemoveAsync(IEntry entry, CancellationToken cancellationToken) {
            this._properties.Remove(entry.Path);
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public override Task<IReadOnlyCollection<bool>> RemoveAsync(IEntry entry, IEnumerable<XName> keys, CancellationToken cancellationToken) {
            var result = new List<bool>();
            if (!this._properties.TryGetValue(entry.Path, out var properties)) {
                result.AddRange(keys.Select(x => false));
            } else {
                foreach (var key in keys) {
                    if (key == GetETagProperty.PropertyName) {
                        this._logger.LogWarning("The ETag property must not be set using the property store.");
                        result.Add(false);
                    } else {
                        result.Add(properties.Remove(key));
                    }
                }

                if (properties.Count == 0) {
                    this._properties.Remove(entry.Path);
                }
            }

            return Task.FromResult<IReadOnlyCollection<bool>>(result);
        }

        /// <inheritdoc />
        protected override Task<EntityTag> GetDeadETagAsync(IEntry entry, CancellationToken cancellationToken) {
            XElement? etagElement;
            if (this._properties.TryGetValue(entry.Path, out var properties)) {
                properties.TryGetValue(GetETagProperty.PropertyName, out etagElement);
            } else {
                etagElement = null;
            }

            if (etagElement == null) {
                etagElement = new EntityTag(false).ToXml();
                this._properties.Add(entry.Path, new Dictionary<XName, XElement>() {
                    [etagElement.Name] = etagElement,
                });
            }

            return Task.FromResult(EntityTag.FromXml(etagElement));
        }

        /// <inheritdoc />
        protected override Task<EntityTag> UpdateDeadETagAsync(IEntry entry, CancellationToken cancellationToken) {
            var etag = EntityTag.FromXml(null);
            var etagElement = etag.ToXml();
            var key = etagElement.Name;

            if (!this._properties.TryGetValue(entry.Path, out var properties)) {
                this._properties.Add(entry.Path, new Dictionary<XName, XElement>() {
                    [key] = etagElement,
                });
            } else {
                properties[key] = etagElement;
            }

            return Task.FromResult(etag);
        }

        private IReadOnlyCollection<XElement> GetAll(IEntry entry) {
            IReadOnlyCollection<XElement> result;
            if (!this._properties.TryGetValue(entry.Path, out var properties)) {
                result = new XElement[0];
            } else {
                result = properties.Values.ToList();
            }

            return result;
        }

        private void SetAll(IEntry entry, IEnumerable<XElement> elements) {
            if (!this._properties.TryGetValue(entry.Path, out var properties)) {
                this._properties.Add(entry.Path, properties = new Dictionary<XName, XElement>());
            }

            var isEtagEntry = entry is IEntityTagEntry;
            foreach (var element in elements) {
                if (isEtagEntry && element.Name == GetETagProperty.PropertyName) {
                    this._logger.LogWarning("The ETag property must not be set using the property store.");
                    continue;
                }

                properties[element.Name] = element;
            }
        }
    }
}
