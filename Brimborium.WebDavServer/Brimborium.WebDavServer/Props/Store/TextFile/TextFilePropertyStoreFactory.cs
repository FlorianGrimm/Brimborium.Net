using System.IO;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Props.Dead;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Brimborium.WebDavServer.Props.Store.TextFile {
    /// <summary>
    /// The factory for the <see cref="TextFilePropertyStore"/>
    /// </summary>
    public class TextFilePropertyStoreFactory : IPropertyStoreFactory {
        private readonly IDeadPropertyFactory _DeadPropertyFactory;

        private readonly IWebDavContext _WebDavContext;

        private readonly TextFilePropertyStoreOptions _Options;

        private readonly ILogger<TextFilePropertyStore> _Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFilePropertyStoreFactory"/> class.
        /// </summary>
        /// <param name="options">The options for the text file property store</param>
        /// <param name="deadPropertyFactory">The factory for the dead properties</param>
        /// <param name="webDavContext">The WebDAV request context</param>
        /// <param name="logger">The logger for the property store factory</param>
        public TextFilePropertyStoreFactory(IOptions<TextFilePropertyStoreOptions> options, IDeadPropertyFactory deadPropertyFactory, IWebDavContext webDavContext, ILogger<TextFilePropertyStore> logger) {
            this._Options = options?.Value ?? new TextFilePropertyStoreOptions();
            this._Logger = logger;
            this._DeadPropertyFactory = deadPropertyFactory;
            this._WebDavContext = webDavContext;
        }

        /// <inheritdoc />
        public IPropertyStore Create(IFileSystem fileSystem) {
            string fileName = ".properties";
            bool storeInRoot;
            string rootPath;
            ILocalFileSystem? localFs;
            if (this._Options.StoreInTargetFileSystem && (localFs = fileSystem as ILocalFileSystem) != null) {
                rootPath = localFs.RootDirectoryPath;
                storeInRoot = !localFs.HasSubfolders;
                if (storeInRoot) {
                    var userName = (this._WebDavContext.User.Identity is not null) && this._WebDavContext.User.Identity.IsAuthenticated
                        ? this._WebDavContext.User.Identity.Name ?? string.Empty
                        : "anonymous";
                    var p = userName.IndexOf('\\');
                    if (p != -1) {
                        userName = userName.Substring(p + 1);
                    }

                    fileName = $"{userName}.json";
                }
            } else if (string.IsNullOrEmpty(this._Options.RootFolder)) {
                var userHomePath = Utils.SystemInfo.GetUserHomePath(this._WebDavContext.User);
                rootPath = Path.Combine(userHomePath, ".webdav");
                storeInRoot = true;
            } else {
                var userName = (this._WebDavContext.User.Identity is not null) && this._WebDavContext.User.Identity.IsAuthenticated
                                   ? this._WebDavContext.User.Identity.Name ?? string.Empty
                                   : "anonymous";
                rootPath = Path.Combine(this._Options.RootFolder, userName);
                storeInRoot = true;
            }

            Directory.CreateDirectory(rootPath);

            return new TextFilePropertyStore(this._Options, this._DeadPropertyFactory, rootPath, storeInRoot, fileName, this._Logger);
        }
    }
}
