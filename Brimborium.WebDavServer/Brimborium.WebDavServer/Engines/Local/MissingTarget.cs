using System;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.FileSystem;

namespace Brimborium.WebDavServer.Engines.Local {
    /// <summary>
    /// The missing local file system target
    /// </summary>
    public class MissingTarget : IMissingTarget<CollectionTarget, DocumentTarget, MissingTarget> {
        private readonly ITargetActions<CollectionTarget, DocumentTarget, MissingTarget> _TargetActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingTarget"/> class.
        /// </summary>
        /// <param name="destinationUrl">The destination URL for this entry</param>
        /// <param name="name">The name of the missing target</param>
        /// <param name="parent">The parent collection</param>
        /// <param name="targetActions">The target actions implementation to use</param>
        public MissingTarget(
            Uri destinationUrl,
            string name,
            CollectionTarget parent,
            ITargetActions<CollectionTarget, DocumentTarget, MissingTarget> targetActions) {
            this._TargetActions = targetActions;
            this.DestinationUrl = destinationUrl;
            this.Name = name;
            this.Parent = parent;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// Gets the parent collection
        /// </summary>
        public CollectionTarget Parent { get; }

        /// <inheritdoc />
        public Uri DestinationUrl { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="MissingTarget"/> class.
        /// </summary>
        /// <param name="destinationUrl">The destination URL for this entry</param>
        /// <param name="parent">The parent collection</param>
        /// <param name="name">The name of the missing target</param>
        /// <param name="targetActions">The target actions implementation to use</param>
        /// <returns>The newly created missing target object</returns>
        public static MissingTarget NewInstance(
            Uri destinationUrl,
            ICollection parent,
            string name,
            ITargetActions<CollectionTarget, DocumentTarget, MissingTarget> targetActions) {
            var collUrl = destinationUrl.GetCollectionUri();
            var collTarget = new CollectionTarget(collUrl, null, parent, false, targetActions);
            var target = new MissingTarget(destinationUrl, name, collTarget, targetActions);
            return target;
        }

        /// <inheritdoc />
        public async Task<CollectionTarget> CreateCollectionAsync(CancellationToken cancellationToken) {
            if (this.Parent is null) {
                throw new InvalidOperationException("this.Parent is null");
            }
            var coll = await this.Parent.Collection.CreateCollectionAsync(this.Name, cancellationToken).ConfigureAwait(false);
            return new CollectionTarget(this.DestinationUrl, this.Parent, coll, true, this._TargetActions);
        }
    }
}
