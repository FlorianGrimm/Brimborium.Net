using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.FileSystem.Mount;
using Brimborium.WebDavServer.Locking;
using Brimborium.WebDavServer.Props.Store;


namespace Brimborium.WebDavServer.FileSystem.InMemory {
    /// <summary>
    /// An in-memory file system implementation
    /// </summary>
    public class InMemoryFileSystem : IFileSystem, IMountPointManager {
        private readonly IPathTraversalEngine _pathTraversalEngine;

        private readonly Dictionary<Uri, IFileSystem> _mountPoints = new Dictionary<Uri, IFileSystem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryFileSystem"/> class.
        /// </summary>
        /// <param name="mountPoint">The mount point where this file system should be included</param>
        /// <param name="pathTraversalEngine">The engine to traverse paths</param>
        /// <param name="systemClock">Interface for the access to the systems clock</param>
        /// <param name="lockManager">The global lock manager</param>
        /// <param name="propertyStoreFactory">The store for dead properties</param>
        public InMemoryFileSystem(
            ICollection? mountPoint,
            IPathTraversalEngine pathTraversalEngine,
            ISystemClock systemClock,
            ILockManager? lockManager = null,
            IPropertyStoreFactory? propertyStoreFactory = null) {
            this.SystemClock = systemClock;
            this.LockManager = lockManager;
            this._pathTraversalEngine = pathTraversalEngine;
            var rootPath = mountPoint?.Path ?? new Uri(string.Empty, UriKind.Relative);
            this.RootCollection = new InMemoryDirectory(this, mountPoint, rootPath, mountPoint?.Name ?? rootPath.GetName(), true);
            this.Root = new AsyncLazy<ICollection>(() => Task.FromResult<ICollection>(this.RootCollection));
            this.PropertyStore = propertyStoreFactory?.Create(this);
        }

        /// <summary>
        /// Gets the root collection
        /// </summary>
        public InMemoryDirectory RootCollection { get; }

        /// <summary>
        /// Gets the systems clock
        /// </summary>
        public ISystemClock SystemClock { get; }

        /// <inheritdoc />
        public AsyncLazy<ICollection> Root { get; }

        /// <inheritdoc />
        public IPropertyStore? PropertyStore { get; }

        /// <inheritdoc />
        public ILockManager? LockManager { get; }

        /// <inheritdoc />
        public bool SupportsRangedRead { get; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the file system is read-only.
        /// </summary>
        public bool IsReadOnly { get; set; }

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
