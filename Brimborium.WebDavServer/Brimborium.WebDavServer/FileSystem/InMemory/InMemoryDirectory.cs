using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Model.Headers;


namespace Brimborium.WebDavServer.FileSystem.InMemory {
    /// <summary>
    /// An in-memory implementation of a WebDAV collection
    /// </summary>
    public class InMemoryDirectory : InMemoryEntry, ICollection, IRecusiveChildrenCollector {
        private readonly Dictionary<string, InMemoryEntry> _children = new Dictionary<string, InMemoryEntry>(StringComparer.OrdinalIgnoreCase);

        private readonly bool _isRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryDirectory"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system this collection belongs to</param>
        /// <param name="parent">The parent collection</param>
        /// <param name="path">The root-relative path of this collection</param>
        /// <param name="name">The name of the collection</param>
        /// <param name="isRoot">Is this the file systems root directory?</param>
        public InMemoryDirectory(
            InMemoryFileSystem fileSystem,
            ICollection? parent,
            Uri path,
            string name,
            bool isRoot = false)
            : base(fileSystem, parent, path, name) {
            this._isRoot = isRoot;
        }

        /// <inheritdoc />
        public override async Task<DeleteResult> DeleteAsync(CancellationToken cancellationToken) {
            if (this.InMemoryFileSystem.IsReadOnly) {
                throw new UnauthorizedAccessException("Failed to modify a read-only file system");
            }

            if (this._isRoot) {
                throw new UnauthorizedAccessException("Cannot remove the file systems root collection");
            }

            if (this.InMemoryParent == null) {
                throw new InvalidOperationException("The collection must belong to a collection");
            }

            if (this.InMemoryParent.Remove(this.Name)) {
                var propStore = this.FileSystem.PropertyStore;
                if (propStore != null) {
                    await propStore.RemoveAsync(this, cancellationToken).ConfigureAwait(false);
                }

                return new DeleteResult(WebDavStatusCode.OK, null);
            }

            return new DeleteResult(WebDavStatusCode.NotFound, this);
        }

        /// <inheritdoc />
        public async Task<IEntry?> GetChildAsync(string name, CancellationToken ct) {
            this._children.TryGetValue(name, out var entry);

            var coll = entry as ICollection;
            if (coll != null) {
                return await coll.GetMountTargetEntryAsync(this.InMemoryFileSystem);
            }

            return entry;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<IEntry>> GetChildrenAsync(CancellationToken ct) {
            var result = new List<IEntry>();
            foreach (var child in this._children.Values) {
                var coll = child as ICollection;
                if (coll != null) {
                    result.Add(await coll.GetMountTargetAsync(this.InMemoryFileSystem).ConfigureAwait(false));
                } else {
                    result.Add(child);
                }
            }

            return result;
        }

        /// <inheritdoc />
        public Task<IDocument> CreateDocumentAsync(string name, CancellationToken ct) {
            return Task.FromResult<IDocument>(this.CreateDocument(name));
        }

        /// <inheritdoc />
        public Task<ICollection> CreateCollectionAsync(string name, CancellationToken ct) {
            var newItem = this.CreateCollection(name);
            return Task.FromResult<ICollection>(newItem);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IEntry> GetEntries(int maxDepth) {
            return this.EnumerateEntries(maxDepth);
        }

        /// <summary>
        /// Creates a document
        /// </summary>
        /// <param name="name">The name of the document to create</param>
        /// <returns>The created document</returns>
        /// <exception cref="UnauthorizedAccessException">The file system is read-only</exception>
        /// <exception cref="IOException">Document or collection with the same name already exists</exception>
        public InMemoryFile CreateDocument(string name) {
            if (this.InMemoryFileSystem.IsReadOnly) {
                throw new UnauthorizedAccessException("Failed to modify a read-only file system");
            }

            if (this._children.ContainsKey(name)) {
                throw new IOException("Document or collection with the same name already exists");
            }

            var newItem = new InMemoryFile(this.InMemoryFileSystem, this, this.Path.Append(name, false), name);
            this._children.Add(newItem.Name, newItem);
            this.ETag = new EntityTag(false);
            return newItem;
        }

        /// <summary>
        /// Creates a new collection
        /// </summary>
        /// <param name="name">The name of the collection to create</param>
        /// <returns>The created collection</returns>
        /// <exception cref="UnauthorizedAccessException">The file system is read-only</exception>
        /// <exception cref="IOException">Document or collection with the same name already exists</exception>
        public InMemoryDirectory CreateCollection(string name) {
            if (this.InMemoryFileSystem.IsReadOnly) {
                throw new UnauthorizedAccessException("Failed to modify a read-only file system");
            }

            if (this._children.ContainsKey(name)) {
                throw new IOException("Document or collection with the same name already exists");
            }

            var newItem = new InMemoryDirectory(this.InMemoryFileSystem, this, this.Path.AppendDirectory(name), name);
            this._children.Add(newItem.Name, newItem);
            this.ETag = new EntityTag(false);
            return newItem;
        }

        internal bool Remove(string name) {
            if (this.InMemoryFileSystem.IsReadOnly) {
                throw new UnauthorizedAccessException("Failed to modify a read-only file system");
            }

            return this._children.Remove(name);
        }
    }
}
