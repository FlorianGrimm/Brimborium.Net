using System;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.Model.Headers;


namespace Brimborium.WebDavServer.FileSystem.InMemory {
    /// <summary>
    /// Am in-memory implementation of a WebDAV entry (collection or document)
    /// </summary>
    public abstract class InMemoryEntry : IEntry, IEntityTagEntry {
        private readonly ICollection? _Parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryEntry"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system this entry belongs to</param>
        /// <param name="parent">The parent collection</param>
        /// <param name="path">The root-relative path of this entry</param>
        /// <param name="name">The name of the entry</param>
        protected InMemoryEntry(InMemoryFileSystem fileSystem, ICollection? parent, Uri path, string name) {
            this._Parent = parent;
            this.Name = name;
            this.FileSystem = this.InMemoryFileSystem = fileSystem;
            this.Path = path;
            this.CreationTimeUtc = this.LastWriteTimeUtc = DateTime.UtcNow;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IFileSystem FileSystem { get; }

        /// <inheritdoc />
        public ICollection? Parent => this._Parent;

        /// <inheritdoc />
        public Uri Path { get; }

        /// <inheritdoc />
        public DateTime LastWriteTimeUtc { get; protected set; }

        /// <inheritdoc />
        public DateTime CreationTimeUtc { get; protected set; }

        /// <inheritdoc />
        public EntityTag ETag { get; protected set; } = new EntityTag(false);

        /// <summary>
        /// Gets the file system
        /// </summary>
        protected InMemoryFileSystem InMemoryFileSystem { get; }

        /// <summary>
        /// Gets the parent collection
        /// </summary>
        protected InMemoryDirectory? InMemoryParent => this._Parent as InMemoryDirectory;

        /// <inheritdoc />
        public Task<EntityTag> UpdateETagAsync(CancellationToken cancellationToken) {
            if (this.InMemoryFileSystem.IsReadOnly) {
                throw new UnauthorizedAccessException("Failed to modify a read-only file system");
            }

            return Task.FromResult(this.ETag = new EntityTag(false));
        }

        /// <inheritdoc />
        public abstract Task<DeleteResult> DeleteAsync(CancellationToken cancellationToken);

        /// <inheritdoc />
        public Task SetLastWriteTimeUtcAsync(DateTime lastWriteTime, CancellationToken cancellationToken) {
            if (this.InMemoryFileSystem.IsReadOnly) {
                throw new UnauthorizedAccessException("Failed to modify a read-only file system");
            }

            this.LastWriteTimeUtc = lastWriteTime;
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public Task SetCreationTimeUtcAsync(DateTime creationTime, CancellationToken cancellationToken) {
            if (this.InMemoryFileSystem.IsReadOnly) {
                throw new UnauthorizedAccessException("Failed to modify a read-only file system");
            }

            this.CreationTimeUtc = creationTime;
            return Task.FromResult(0);
        }
    }
}
