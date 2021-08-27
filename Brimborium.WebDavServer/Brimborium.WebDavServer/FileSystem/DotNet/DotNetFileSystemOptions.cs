using Brimborium.WebDavServer.Utils;

namespace Brimborium.WebDavServer.FileSystem.DotNet {
    /// <summary>
    /// The options for the <see cref="DotNetFileSystemFactory"/> and <see cref="DotNetFileSystem"/>
    /// </summary>
    public class DotNetFileSystemOptions {
        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetFileSystemOptions"/> class.
        /// </summary>
        public DotNetFileSystemOptions() {
#warning here
            var info = SystemInfo.GetHomePath();
            this.RootPath = info.RootPath;
            this.AnonymousUserName = string.Empty;
        }

        /// <summary>
        /// Gets or sets the home path for all users
        /// </summary>
        public string RootPath { get; set; }

        /// <summary>
        /// Gets or sets the path name for the anonymous user
        /// </summary>
        public string AnonymousUserName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether infinite path depth is allowed
        /// </summary>
        public bool AllowInfiniteDepth { get; set; } = true;
    }
}
