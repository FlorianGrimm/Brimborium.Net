using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.Props;

namespace Brimborium.WebDavServer.Engines.Remote
{
    /// <summary>
    /// The remote server document target
    /// </summary>
    public class RemoteDocumentTarget : IDocumentTarget<RemoteCollectionTarget, RemoteDocumentTarget, RemoteMissingTarget>
    {
        private readonly IRemoteTargetActions _targetActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDocumentTarget"/> class.
        /// </summary>
        /// <param name="parent">The parent collection</param>
        /// <param name="name">The name of the remote document</param>
        /// <param name="destinationUrl">The destination URL</param>
        /// <param name="targetActions">The target actions implementation to use</param>
        public RemoteDocumentTarget(RemoteCollectionTarget parent, string name, Uri destinationUrl, IRemoteTargetActions targetActions)
        {
            this._targetActions = targetActions;
            this.Parent = parent;
            this.Name = name;
            this.DestinationUrl = destinationUrl;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public Uri DestinationUrl { get; }

        /// <summary>
        /// Gets the parent remote collection
        /// </summary>
        public RemoteCollectionTarget Parent { get; }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<XName>> SetPropertiesAsync(IEnumerable<IUntypedWriteableProperty> properties, CancellationToken cancellationToken)
        {
            return this._targetActions.SetPropertiesAsync(this, properties, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<RemoteMissingTarget> DeleteAsync(CancellationToken cancellationToken)
        {
            await this._targetActions.DeleteAsync(this, cancellationToken).ConfigureAwait(false);
            return this.Parent.NewMissing(this.Name);
        }
    }
}
