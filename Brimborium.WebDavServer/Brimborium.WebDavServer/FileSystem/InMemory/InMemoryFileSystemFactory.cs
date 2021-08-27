#pragma warning disable IDE0041 // Use 'is null' check

using System;
using System.Collections.Generic;
using System.Security.Principal;

using Brimborium.WebDavServer.Locking;
using Brimborium.WebDavServer.Props.Store;
using Brimborium.WebDavServer.Utils;

namespace Brimborium.WebDavServer.FileSystem.InMemory {
    /// <summary>
    /// An in-memory implementation of the <see cref="IFileSystemFactory"/>
    /// </summary>
    public class InMemoryFileSystemFactory : IFileSystemFactory {
        private readonly Dictionary<FileSystemKey, InMemoryFileSystem> _fileSystems = new Dictionary<FileSystemKey, InMemoryFileSystem>();

        private readonly IPathTraversalEngine _pathTraversalEngine;

        private readonly ISystemClock _systemClock;

        private readonly ILockManager? _lockManager;

        private readonly IPropertyStoreFactory? _propertyStoreFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryFileSystemFactory"/> class.
        /// </summary>
        /// <param name="pathTraversalEngine">The engine to traverse paths</param>
        /// <param name="systemClock">Interface for the access to the systems clock</param>
        /// <param name="lockManager">The global lock manager</param>
        /// <param name="propertyStoreFactory">The store for dead properties</param>
        public InMemoryFileSystemFactory(
            IPathTraversalEngine pathTraversalEngine,
            ISystemClock systemClock,
            ILockManager? lockManager = null,
            IPropertyStoreFactory? propertyStoreFactory = null) {
            this._pathTraversalEngine = pathTraversalEngine;
            this._systemClock = systemClock;
            this._lockManager = lockManager;
            this._propertyStoreFactory = propertyStoreFactory;
        }

        /// <inheritdoc />
        public virtual IFileSystem CreateFileSystem(ICollection? mountPoint, IPrincipal principal) {
            var userName = ((principal.Identity is not null) && (!principal.Identity.IsAnonymous()))
                ? (principal.Identity.Name ?? string.Empty)
                : string.Empty;

            var key = new FileSystemKey(userName, mountPoint?.Path.OriginalString ?? string.Empty);
            if (!this._fileSystems.TryGetValue(key, out var fileSystem)) {
                fileSystem = new InMemoryFileSystem(mountPoint, this._pathTraversalEngine, this._systemClock, this._lockManager, this._propertyStoreFactory);
                this._fileSystems.Add(key, fileSystem);
                this.InitializeFileSystem(mountPoint, principal, fileSystem);
            } else {
                this.UpdateFileSystem(mountPoint, principal, fileSystem);
            }

            return fileSystem;
        }

        /// <summary>
        /// Called when file system will be initialized
        /// </summary>
        /// <param name="mountPoint">The mount point</param>
        /// <param name="principal">The principal the file system was created for</param>
        /// <param name="fileSystem">The created file system</param>
        protected virtual void InitializeFileSystem(ICollection? mountPoint, IPrincipal principal, InMemoryFileSystem fileSystem) {
        }

        /// <summary>
        /// Called when the file system will be updated
        /// </summary>
        /// <param name="mountPoint">The mount point</param>
        /// <param name="principal">The principal the file system was created for</param>
        /// <param name="fileSystem">The created file system</param>
        protected virtual void UpdateFileSystem(ICollection? mountPoint, IPrincipal principal, InMemoryFileSystem fileSystem) {
        }

        private class FileSystemKey : IEquatable<FileSystemKey> {
            private static readonly IEqualityComparer<string> _comparer = StringComparer.OrdinalIgnoreCase;

            private readonly string _userName;

            private readonly string _mountPoint;

            public FileSystemKey(string userName, string mountPoint) {
                this._userName = userName;
                this._mountPoint = mountPoint;
            }

            public bool Equals(FileSystemKey? other) {
                if (ReferenceEquals(null, other)) {
                    return false;
                }

                if (ReferenceEquals(this, other)) {
                    return true;
                }

                return _comparer.Equals(this._userName, other._userName) && _comparer.Equals(this._mountPoint, other._mountPoint);
            }

            public override bool Equals(object? obj) {
                if (ReferenceEquals(null, obj)) {
                    return false;
                }

                if (ReferenceEquals(this, obj)) {
                    return true;
                }

                if (obj.GetType() != this.GetType()) {
                    return false;
                }

                return this.Equals((FileSystemKey)obj);
            }

            public override int GetHashCode() {
                unchecked {
                    return ((this._userName != null ? _comparer.GetHashCode(this._userName) : 0) * 397) ^ (this._mountPoint != null ? _comparer.GetHashCode(this._mountPoint) : 0);
                }
            }
        }
    }
}
