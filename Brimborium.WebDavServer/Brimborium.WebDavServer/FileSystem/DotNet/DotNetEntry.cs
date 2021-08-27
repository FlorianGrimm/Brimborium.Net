using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace Brimborium.WebDavServer.FileSystem.DotNet {
    /// <summary>
    /// A .NET <see cref="System.IO"/> based implementation of a WebDAV entry (collection or document)
    /// </summary>
    public abstract class DotNetEntry : IEntry {
        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetEntry"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system this entry belongs to</param>
        /// <param name="parent">The parent collection</param>
        /// <param name="info">The file system information</param>
        /// <param name="path">The root-relative path of this entry</param>
        /// <param name="name">The entry name (<see langword="null"/> when <see cref="FileSystemInfo.Name"/> of <see cref="Info"/> should be used)</param>
        protected DotNetEntry(DotNetFileSystem fileSystem, ICollection? parent, FileSystemInfo info, Uri path, string? name) {
            this.Parent = parent;
            this.Info = info;
            this.DotNetFileSystem = fileSystem;
            this.Path = path;
            this.Name = name ?? info.Name;
        }

        /// <summary>
        /// Gets the file system information of this entry
        /// </summary>
        public FileSystemInfo Info { get; }

        /// <summary>
        /// Gets the file system this entry belongs to
        /// </summary>
        public DotNetFileSystem DotNetFileSystem { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IFileSystem FileSystem => this.DotNetFileSystem;

        /// <inheritdoc />
        public ICollection? Parent { get; }

        /// <inheritdoc />
        public Uri Path { get; }

        /// <inheritdoc />
        public DateTime LastWriteTimeUtc => this.Info.LastWriteTimeUtc;

        /// <inheritdoc />
        public DateTime CreationTimeUtc => this.Info.CreationTimeUtc;

        /// <inheritdoc />
        public abstract Task<DeleteResult> DeleteAsync(CancellationToken cancellationToken);

        /// <inheritdoc />
        public Task SetLastWriteTimeUtcAsync(DateTime lastWriteTime, CancellationToken cancellationToken) {
            this.Info.LastWriteTimeUtc = lastWriteTime;
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public Task SetCreationTimeUtcAsync(DateTime creationTime, CancellationToken cancellationToken) {
            this.Info.CreationTimeUtc = creationTime;
            return Task.FromResult(0);
        }
    }
}
