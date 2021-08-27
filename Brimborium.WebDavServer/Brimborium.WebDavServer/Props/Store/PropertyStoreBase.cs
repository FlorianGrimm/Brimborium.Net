using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Model.Headers;
using Brimborium.WebDavServer.Props.Dead;

namespace Brimborium.WebDavServer.Props.Store {
    /// <summary>
    /// Common functionality for a property store implementation
    /// </summary>
    public abstract class PropertyStoreBase : IPropertyStore {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyStoreBase"/> class.
        /// </summary>
        /// <param name="deadPropertyFactory">The factory to create dead properties</param>
        protected PropertyStoreBase(IDeadPropertyFactory deadPropertyFactory) {
            this.DeadPropertyFactory = deadPropertyFactory;
        }

        /// <inheritdoc />
        public abstract int Cost { get; }

        /// <summary>
        /// Gets the dead property factory
        /// </summary>
        protected IDeadPropertyFactory DeadPropertyFactory { get; }

        /// <inheritdoc />
        public virtual async Task<XElement?> GetAsync(IEntry entry, XName name, CancellationToken cancellationToken) {
            var elements = await this.GetAsync(entry, cancellationToken).ConfigureAwait(false);
            return elements.FirstOrDefault(x => x.Name == name);
        }

        /// <inheritdoc />
        public virtual Task SetAsync(IEntry entry, XElement element, CancellationToken cancellationToken) {
            return this.SetAsync(entry, new[] { element }, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<bool> RemoveAsync(IEntry entry, XName name, CancellationToken cancellationToken) {
            return (await this.RemoveAsync(entry, new[] { name }, cancellationToken).ConfigureAwait(false)).Single();
        }

        /// <inheritdoc />
        public abstract Task<IReadOnlyCollection<XElement>> GetAsync(IEntry entry, CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract Task SetAsync(IEntry entry, IEnumerable<XElement> properties, CancellationToken cancellationToken);

        /// <inheritdoc />
        public abstract Task<IReadOnlyCollection<bool>> RemoveAsync(IEntry entry, IEnumerable<XName> names, CancellationToken cancellationToken);

        /// <inheritdoc />
        public virtual async Task RemoveAsync(IEntry entry, CancellationToken cancellationToken) {
            var elements = await this.GetAsync(entry, cancellationToken).ConfigureAwait(false);
            var names = elements.Where(x => x.Name != GetETagProperty.PropertyName).Select(x => x.Name).ToList();
            if (elements.Count != names.Count) {
                // Has ETag, so force the update of an ETag
                await this.UpdateETagAsync(entry, cancellationToken).ConfigureAwait(false);
            }

            await this.RemoveAsync(entry, names, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public IDeadProperty Create(IEntry entry, XName name) {
            return this.DeadPropertyFactory.Create(this, entry, name);
        }

        /// <inheritdoc />
        public virtual async Task<IDeadProperty> LoadAsync(IEntry entry, XName name, CancellationToken cancellationToken) {
            var element = await this.GetAsync(entry, name, cancellationToken).ConfigureAwait(false);
            if (element == null) {
                return this.Create(entry, name);
            }

            return this.CreateProperty(entry, element);
        }

        /// <inheritdoc />
        public virtual async Task<IReadOnlyCollection<IDeadProperty>> LoadAsync(IEntry entry, CancellationToken cancellationToken) {
            var elements = await this.GetAsync(entry, cancellationToken).ConfigureAwait(false);
            return elements.Select(x => this.CreateProperty(entry, x)).ToList();
        }

        /// <inheritdoc />
        public Task<EntityTag> GetETagAsync(IEntry entry, CancellationToken cancellationToken) {
            var etagEntry = entry as IEntityTagEntry;
            if (etagEntry != null) {
                return Task.FromResult(etagEntry.ETag);
            }

            return this.GetDeadETagAsync(entry, cancellationToken);
        }

        /// <inheritdoc />
        public Task<EntityTag> UpdateETagAsync(IEntry entry, CancellationToken cancellationToken) {
            var etagEntry = entry as IEntityTagEntry;
            if (etagEntry != null) {
                return etagEntry.UpdateETagAsync(cancellationToken);
            }

            return this.UpdateDeadETagAsync(entry, cancellationToken);
        }

        /// <summary>
        /// Gets a <see cref="GetETagProperty"/> from the property store
        /// </summary>
        /// <param name="entry">The entry to get the <c>getetag</c> property from</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The entity tag for the <paramref name="entry"/></returns>
        protected abstract Task<EntityTag> GetDeadETagAsync(IEntry entry, CancellationToken cancellationToken);

        /// <summary>
        /// Updates a <see cref="GetETagProperty"/> in the property store
        /// </summary>
        /// <param name="entry">The entry to update the <c>getetag</c> property for</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The updated entity tag for the <paramref name="entry"/></returns>
        protected abstract Task<EntityTag> UpdateDeadETagAsync(IEntry entry, CancellationToken cancellationToken);

        private IDeadProperty CreateProperty(IEntry entry, XElement element) {
            return this.DeadPropertyFactory.Create(this, entry, element);
        }
    }
}
