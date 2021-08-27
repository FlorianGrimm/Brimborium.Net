using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.FileSystem.Mount;
using Brimborium.WebDavServer.Locking;
using Brimborium.WebDavServer.Props.Store;


namespace Brimborium.WebDavServer.FileSystem.DotNet {
    /// <summary>
    /// A file system implementation using <see cref="System.IO"/>
    /// </summary>
    public class DotNetFileSystem : ILocalFileSystem, IMountPointManager {
        private readonly IPathTraversalEngine _pathTraversalEngine;

        private readonly Dictionary<Uri, IFileSystem> _mountPoints = new Dictionary<Uri, IFileSystem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetFileSystem"/> class.
        /// </summary>
        /// <param name="options">The options for this file system</param>
        /// <param name="mountPoint">The mount point where this file system should be included</param>
        /// <param name="rootFolder">The root folder</param>
        /// <param name="pathTraversalEngine">The engine to traverse paths</param>
        /// <param name="lockManager">The global lock manager</param>
        /// <param name="propertyStoreFactory">The store for dead properties</param>
        public DotNetFileSystem(
            DotNetFileSystemOptions options,
            ICollection? mountPoint,
            string rootFolder,
            IPathTraversalEngine pathTraversalEngine,
            ILockManager? lockManager = null,
            IPropertyStoreFactory? propertyStoreFactory = null) {
            this.LockManager = lockManager;
            this.RootDirectoryPath = rootFolder;
            this._pathTraversalEngine = pathTraversalEngine;
            this.Options = options;
            this.PropertyStore = propertyStoreFactory?.Create(this);
            var rootPath = mountPoint?.Path ?? new Uri(string.Empty, UriKind.Relative);
            var rootDir = new DotNetDirectory(this, mountPoint, new DirectoryInfo(rootFolder), rootPath, mountPoint?.Name ?? rootPath.GetName(), true);
            this.Root = new AsyncLazy<ICollection>(() => Task.FromResult<ICollection>(rootDir));
        }

        /// <inheritdoc />
        public string RootDirectoryPath { get; }

        /// <inheritdoc />
        public bool HasSubfolders { get; } = true;

        /// <inheritdoc />
        public AsyncLazy<ICollection> Root { get; }

        /// <summary>
        /// Gets the file systems options
        /// </summary>
        public DotNetFileSystemOptions Options { get; }

        /// <inheritdoc />
        public IPropertyStore? PropertyStore { get; }

        /// <inheritdoc />
        public ILockManager? LockManager { get; }

        /// <inheritdoc />
        public bool SupportsRangedRead { get; } = true;

        /// <inheritdoc />
        public IEnumerable<Uri> MountPoints => this._mountPoints.Keys;

        /// <inheritdoc />
        public Task<SelectionResult> SelectAsync(string path, CancellationToken ct) {
            return this._pathTraversalEngine.TraverseAsync(this, path, ct);
        }

        /// <inheritdoc />
        public bool TryGetMountPoint(Uri path, [MaybeNullWhen(false)] out IFileSystem destination) {
            return this._mountPoints.TryGetValue(path, out destination);
        }

        /// <inheritdoc />
        public void Mount(Uri source, IFileSystem destination) {
            this._mountPoints.Add(source, destination);
        }

        /// <inheritdoc />
        public void Unmount(Uri source) {
            this._mountPoints.Remove(source);
        }
    }
}
