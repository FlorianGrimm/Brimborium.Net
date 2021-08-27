using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.Props;

namespace Brimborium.WebDavServer.Engines.Remote
{
    /// <summary>
    /// The remote server collection target
    /// </summary>
    public class RemoteCollectionTarget : ICollectionTarget<RemoteCollectionTarget, RemoteDocumentTarget, RemoteMissingTarget>
    {
        private readonly RemoteCollectionTarget? _parent;

        private readonly IRemoteTargetActions _targetActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteCollectionTarget"/> class.
        /// </summary>
        /// <param name="parent">The parent collection</param>
        /// <param name="name">The name of the remote collection</param>
        /// <param name="destinationUrl">The destination URL</param>
        /// <param name="created">Was the collection created by the <see cref="RecursiveExecutionEngine{TCollection,TDocument,TMissing}"/></param>
        /// <param name="targetActions">The target actions implementation to use</param>
        public RemoteCollectionTarget(RemoteCollectionTarget? parent, string name, Uri destinationUrl, bool created, IRemoteTargetActions targetActions)
        {
            this._parent = parent;
            this._targetActions = targetActions;
            this.Name = name;
            this.DestinationUrl = destinationUrl;
            this.Created = created;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public Uri DestinationUrl { get; }

        /// <inheritdoc />
        public bool Created { get; }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<XName>> SetPropertiesAsync(IEnumerable<IUntypedWriteableProperty> properties, CancellationToken cancellationToken)
        {
            return this._targetActions.SetPropertiesAsync(this, properties, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<RemoteMissingTarget> DeleteAsync(CancellationToken cancellationToken)
        {
            await this._targetActions.DeleteAsync(this, cancellationToken).ConfigureAwait(false);
            Debug.Assert(this._parent != null, "_parent != null");
            return this._parent.NewMissing(this.Name);
        }

        /// <inheritdoc />
        public Task<ITarget> GetAsync(string name, CancellationToken cancellationToken)
        {
            return this._targetActions.GetAsync(this, name, cancellationToken);
        }

        /// <inheritdoc />
        public RemoteMissingTarget NewMissing(string name)
        {
            return new RemoteMissingTarget(this, this.DestinationUrl.Append(name, false), name, this._targetActions);
        }
    }
}
