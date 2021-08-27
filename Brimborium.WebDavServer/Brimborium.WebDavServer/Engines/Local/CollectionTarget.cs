using System;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.FileSystem;

namespace Brimborium.WebDavServer.Engines.Local {
    /// <summary>
    /// The local file system collection target
    /// </summary>
    public class CollectionTarget : EntryTarget, ICollectionTarget<CollectionTarget, DocumentTarget, MissingTarget> {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionTarget"/> class.
        /// </summary>
        /// <param name="destinationUrl">The destination URL for this collection</param>
        /// <param name="parent">The parent collection</param>
        /// <param name="collection">The underlying collection</param>
        /// <param name="created">Was this collection created by the <see cref="RecursiveExecutionEngine{TCollection,TDocument,TMissing}"/></param>
        /// <param name="targetActions">The target actions implementation to use</param>
        public CollectionTarget(
            Uri destinationUrl,
            CollectionTarget? parent,
            ICollection collection,
            bool created,
            ITargetActions<CollectionTarget, DocumentTarget, MissingTarget> targetActions)
            : base(targetActions, parent, destinationUrl, collection) {
            this.Collection = collection;
            this.Created = created;
        }

        /// <summary>
        /// Gets the underlying collection
        /// </summary>
        public ICollection Collection { get; }

        /// <inheritdoc />
        public bool Created { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="CollectionTarget"/> class.
        /// </summary>
        /// <param name="destinationUrl">The destination URL for this collection</param>
        /// <param name="collection">The underlying collection</param>
        /// <param name="targetActions">The target actions implementation to use</param>
        /// <returns>The created collection target object</returns>
        public static CollectionTarget NewInstance(
            Uri destinationUrl,
            ICollection collection,
            ITargetActions<CollectionTarget, DocumentTarget, MissingTarget> targetActions
            ) {
            CollectionTarget? parentTarget;
            if (collection.Parent != null) {
                var collUrl = destinationUrl.GetParent();
                parentTarget = new CollectionTarget(collUrl, null, collection.Parent, false, targetActions);
            } else {
                parentTarget = null;
            }

            var target = new CollectionTarget(destinationUrl, parentTarget, collection, false, targetActions);
            return target;
        }

        /// <inheritdoc />
        public async Task<MissingTarget> DeleteAsync(CancellationToken cancellationToken) {
            var name = this.Collection.Name;
            await this.Collection.DeleteAsync(cancellationToken).ConfigureAwait(false);
            return new MissingTarget(this.DestinationUrl, name, this.Parent!, this.TargetActions);
        }

        /// <inheritdoc />
        public async Task<ITarget> GetAsync(string name, CancellationToken cancellationToken) {
            var result = await this.Collection.GetChildAsync(name, cancellationToken).ConfigureAwait(false);
            if (result == null) {
                return new MissingTarget(this.DestinationUrl.Append(name, false), name, this, this.TargetActions);
            }

            var doc = result as IDocument;
            if (doc != null) {
                return new DocumentTarget(this, this.DestinationUrl.Append(doc), doc, this.TargetActions);
            }

            var coll = (ICollection)result;
            return new CollectionTarget(this.DestinationUrl.Append(coll), this, coll, false, this.TargetActions);
        }

        /// <inheritdoc />
        public MissingTarget NewMissing(string name) {
            return new MissingTarget(this.DestinationUrl.Append(name, false), name, this, this.TargetActions);
        }
    }
}
