using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebDavServer.Engines.Remote
{
    /// <summary>
    /// The missing remote target
    /// </summary>
    public class RemoteMissingTarget : IMissingTarget<RemoteCollectionTarget, RemoteDocumentTarget, RemoteMissingTarget>
    {
        private readonly IRemoteTargetActions _targetActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteMissingTarget"/> class.
        /// </summary>
        /// <param name="parent">The parent collection</param>
        /// <param name="destinationUrl">The destination URL</param>
        /// <param name="name">The name of the missing remote targe</param>
        /// <param name="targetActions">The target actions implementation to use</param>
        public RemoteMissingTarget(RemoteCollectionTarget parent, Uri destinationUrl, string name, IRemoteTargetActions targetActions)
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
        public Task<RemoteCollectionTarget> CreateCollectionAsync(CancellationToken cancellationToken)
        {
            return this._targetActions.CreateCollectionAsync(this.Parent, this.Name, cancellationToken);
        }
    }
}
