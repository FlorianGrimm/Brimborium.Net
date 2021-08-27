using System.IO;
using System.Security.Principal;

using Brimborium.WebDavServer.Locking;
using Brimborium.WebDavServer.Props.Store;


using Microsoft.Extensions.Options;

namespace Brimborium.WebDavServer.FileSystem.DotNet {
    /// <summary>
    /// The factory creating/getting the file systems that use <see cref="System.IO"/> for its implementation
    /// </summary>
    public class DotNetFileSystemFactory : IFileSystemFactory {
        private readonly IPathTraversalEngine _pathTraversalEngine;

        private readonly IPropertyStoreFactory? _propertyStoreFactory;

        private readonly ILockManager? _lockManager;

        private readonly DotNetFileSystemOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetFileSystemFactory"/> class.
        /// </summary>
        /// <param name="options">The options for this file system</param>
        /// <param name="pathTraversalEngine">The engine to traverse paths</param>
        /// <param name="propertyStoreFactory">The store for dead properties</param>
        /// <param name="lockManager">The global lock manager</param>
        public DotNetFileSystemFactory(
            IOptions<DotNetFileSystemOptions> options,
            IPathTraversalEngine pathTraversalEngine,
            IPropertyStoreFactory? propertyStoreFactory = null,
            ILockManager? lockManager = null) {
            this._pathTraversalEngine = pathTraversalEngine;
            this._propertyStoreFactory = propertyStoreFactory;
            this._lockManager = lockManager;
            this._options = options.Value;
        }

        /// <inheritdoc />
        public virtual IFileSystem CreateFileSystem(ICollection? mountPoint, IPrincipal principal) {
            //var rootFileSystemPath = Utils.SystemInfo.GetUserHomePath(
            //    principal,
            //    homePath: this._options.RootPath,
            //    anonymousUserName: this._options.AnonymousUserName);

            var rootFileSystemPath = this._options.RootPath;

            Directory.CreateDirectory(rootFileSystemPath);

            return new DotNetFileSystem(this._options, mountPoint, rootFileSystemPath, this._pathTraversalEngine, this._lockManager, this._propertyStoreFactory);
        }
    }
}
