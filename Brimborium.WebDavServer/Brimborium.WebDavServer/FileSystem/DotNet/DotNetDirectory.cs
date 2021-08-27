using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Props.Store;


namespace Brimborium.WebDavServer.FileSystem.DotNet {
    /// <summary>
    /// A .NET <see cref="System.IO"/> based implementation of a WebDAV collection
    /// </summary>
    public sealed class DotNetDirectory : DotNetEntry, ICollection, IRecusiveChildrenCollector {
        private readonly IFileSystemPropertyStore? _FileSystemPropertyStore;

        private readonly bool _IsRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetDirectory"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system this collection belongs to</param>
        /// <param name="parent">The parent collection</param>
        /// <param name="info">The directory information</param>
        /// <param name="path">The root-relative path of this collection</param>
        /// <param name="name">The entry name (<see langword="null"/> when <see cref="FileSystemInfo.Name"/> of <see cref="DotNetEntry.Info"/> should be used)</param>
        /// <param name="isRoot">Is this the file systems root directory?</param>
        public DotNetDirectory(
            DotNetFileSystem fileSystem,
            ICollection? parent,
            DirectoryInfo info,
            Uri path,
            string? name,
            bool isRoot = false)
            : base(fileSystem, parent, info, path, name) {
            this._IsRoot = isRoot;
            this._FileSystemPropertyStore = fileSystem.PropertyStore as IFileSystemPropertyStore;
            this.DirectoryInfo = info;
        }

        /// <summary>
        /// Gets the collections directory information
        /// </summary>
        public DirectoryInfo DirectoryInfo { get; }

        /// <inheritdoc />
        public async Task<IEntry?> GetChildAsync(string name, CancellationToken ct) {
            var newPath = System.IO.Path.Combine(this.DirectoryInfo.FullName, name);

            FileSystemInfo item = new FileInfo(newPath);
            if (!item.Exists) {
                item = new DirectoryInfo(newPath);
            }

            if (!item.Exists) {
                return null;
            }

            var entry = this.CreateEntry(item);

            var coll = entry as ICollection;
            if (coll != null) {
                return await coll.GetMountTargetEntryAsync(this.DotNetFileSystem);
            }

            return entry;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<IEntry>> GetChildrenAsync(CancellationToken ct) {
            var result = new List<IEntry>();
            foreach (var info in this.DirectoryInfo.EnumerateFileSystemInfos()) {
                ct.ThrowIfCancellationRequested();
                var entry = this.CreateEntry(info);
                var ignoreEntry = this._FileSystemPropertyStore?.IgnoreEntry(entry) ?? false;
                if (!ignoreEntry) {
                    var coll = entry as ICollection;
                    if (coll != null) {
                        entry = await coll.GetMountTargetAsync(this.DotNetFileSystem);
                    }

                    result.Add(entry);
                }
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<IDocument> CreateDocumentAsync(string name, CancellationToken cancellationToken) {
            var fullFileName = System.IO.Path.Combine(this.DirectoryInfo.FullName, name);
            var info = new FileInfo(fullFileName);
            info.Create().Dispose();
            if (this.FileSystem.PropertyStore != null) {
                await this.FileSystem.PropertyStore.UpdateETagAsync(this, cancellationToken).ConfigureAwait(false);
            }

            return (IDocument)this.CreateEntry(new FileInfo(fullFileName));
        }

        /// <inheritdoc />
        public async Task<ICollection> CreateCollectionAsync(string name, CancellationToken cancellationToken) {
            var fullDirPath = System.IO.Path.Combine(this.DirectoryInfo.FullName, name);

            var info = new DirectoryInfo(fullDirPath);
            if (info.Exists) {
                throw new IOException("Collection already exists.");
            }

            info.Create();

            if (this.FileSystem.PropertyStore != null) {
                await this.FileSystem.PropertyStore.UpdateETagAsync(this, cancellationToken).ConfigureAwait(false);
            }

            return (ICollection)this.CreateEntry(new DirectoryInfo(fullDirPath));
        }

        /// <inheritdoc />
        public override async Task<DeleteResult> DeleteAsync(CancellationToken cancellationToken) {
            if (this._IsRoot) {
                throw new UnauthorizedAccessException("Cannot remove the file systems root collection");
            }

            var propStore = this.FileSystem.PropertyStore;
            if (propStore != null) {
                await propStore.RemoveAsync(this, cancellationToken).ConfigureAwait(false);
            }

            this.DirectoryInfo.Delete(true);

            return new DeleteResult(WebDavStatusCode.OK, null);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IEntry> GetEntries(int maxDepth) {
            return this.EnumerateEntries(maxDepth);
        }

        private IEntry CreateEntry(FileSystemInfo fsInfo) {
            var fileInfo = fsInfo as FileInfo;
            if (fileInfo != null) {
                return new DotNetFile(this.DotNetFileSystem, this, fileInfo, this.Path.Append(fileInfo.Name, false));
            }

            var dirInfo = (DirectoryInfo)fsInfo;
            var dirPath = this.Path.AppendDirectory(dirInfo.Name);
            return new DotNetDirectory(this.DotNetFileSystem, this, dirInfo, dirPath, null);
        }
    }
}
