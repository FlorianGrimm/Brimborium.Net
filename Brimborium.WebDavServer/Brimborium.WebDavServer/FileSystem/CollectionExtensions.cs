using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.FileSystem.Mount;

namespace Brimborium.WebDavServer.FileSystem {
    /// <summary>
    /// Extension methods for the collections
    /// </summary>
    public static class CollectionExtensions {
        /// <summary>
        /// Returns the target if the collection is a mount point or the collection itself
        /// </summary>
        /// <param name="collection">The collection to found the mount destination for</param>
        /// <param name="mountPointProvider">The mount point provider</param>
        /// <returns>The <paramref name="collection"/> or the destination collection if a mount point existed</returns>
        public static async Task<ICollection> GetMountTargetAsync(this ICollection collection, IMountPointProvider? mountPointProvider) {
            if (mountPointProvider != null && mountPointProvider.TryGetMountPoint(collection.Path, out var fileSystem)) {
                return await fileSystem.Root;
            }

            return collection;
        }

        /// <summary>
        /// Returns the target if the collection is a mount point or the collection itself
        /// </summary>
        /// <param name="collection">The collection to found the mount destination for</param>
        /// <param name="mountPointProvider">The mount point provider</param>
        /// <returns>The <paramref name="collection"/> or the destination collection if a mount point existed</returns>
        public static async Task<IEntry> GetMountTargetEntryAsync(this ICollection collection, IMountPointProvider? mountPointProvider) {
            if (mountPointProvider != null && mountPointProvider.TryGetMountPoint(collection.Path, out var fileSystem)) {
                return await fileSystem.Root;
            }

            return collection;
        }

        /// <summary>
        /// Gets all entries of a collection recursively
        /// </summary>
        /// <param name="collection">The collection to get the entries from</param>
        /// <param name="children">Child items for the given <paramref name="collection"/></param>
        /// <param name="maxDepth">The maximum depth (0 = only entries of the <paramref name="collection"/>, but not of its sub collections)</param>
        /// <returns>An async enumerable to collect all the entries recursively</returns>
        public static IAsyncEnumerable<IEntry> EnumerateEntries(this ICollection collection, IReadOnlyCollection<IEntry> children, int maxDepth) {
            return new FileSystemEntries(collection, children, 0, maxDepth);
        }

        /// <summary>
        /// Gets all entries of a collection recursively
        /// </summary>
        /// <param name="collection">The collection to get the entries from</param>
        /// <param name="maxDepth">The maximum depth (0 = only entries of the <paramref name="collection"/>, but not of its sub collections)</param>
        /// <returns>An async enumerable to collect all the entries recursively</returns>
        public static IAsyncEnumerable<IEntry> EnumerateEntries(this ICollection collection, int maxDepth) {
            return new FileSystemEntries(collection, null, 0, maxDepth);
        }

        /// <summary>
        /// Gets the collection as node
        /// </summary>
        /// <param name="collection">The collection to get the node for</param>
        /// <param name="maxDepth">The maximum depth to be used to get the child nodes</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The collection node</returns>
        public static async Task<ICollectionNode> GetNodeAsync(this ICollection collection, int maxDepth, CancellationToken cancellationToken) {
            var subNodeQueue = new Queue<NodeInfo>();
            var result = new NodeInfo(collection);
            var current = result;

            if (maxDepth > 0) {
                await using (var entries = EnumerateEntries(collection, maxDepth - 1).GetAsyncEnumerator()) {
                    while (await entries.MoveNextAsync(cancellationToken).ConfigureAwait(false)) {
                        var entry = entries.Current;
                        var parent = entry.Parent;
                        while (parent != current.Collection) {
                            current = subNodeQueue.Dequeue();
                        }

                        var doc = entry as IDocument;
                        if (doc == null) {
                            var coll = (ICollection)entry;
                            var info = new NodeInfo(coll);
                            current.SubNodes.Add(info);
                            subNodeQueue.Enqueue(info);
                        } else {
                            current.Documents.Add(doc);
                        }
                    }
                }
            }

            return result;
        }

        private class FileSystemEntries : IAsyncEnumerable<IEntry> {
            private readonly ICollection _collection;

            private readonly IReadOnlyCollection<IEntry>? _children;

            private readonly int _remainingDepth;

            private readonly int _startDepth;

            public FileSystemEntries(ICollection collection, IReadOnlyCollection<IEntry>? children, int startDepth, int remainingDepth) {
                this._collection = collection;
                this._children = children;
                this._startDepth = startDepth;
                this._remainingDepth = remainingDepth;
            }

            public IAsyncEnumerator<IEntry> GetAsyncEnumerator(CancellationToken cancellationToken = default) {
                return new FileSystemEntriesEnumerator(this._collection, this._children, this._startDepth, this._remainingDepth, cancellationToken);
            }

            public IAsyncEnumerator<IEntry> GetEnumerator() {
                return new FileSystemEntriesEnumerator(this._collection, this._children, this._startDepth, this._remainingDepth, CancellationToken.None);
            }

            private class FileSystemEntriesEnumerator : IAsyncEnumerator<IEntry> {
                private readonly Queue<CollectionInfo> _collections = new Queue<CollectionInfo>();

                private readonly int _maxDepth;
                private readonly CancellationToken _CancellationToken;
                private ICollection? _collection;

                private int _currentDepth;

                private IEnumerator<IEntry>? _entries;

                private IEntry? _Current;

                public FileSystemEntriesEnumerator(ICollection collection, IReadOnlyCollection<IEntry>? children, int startDepth, int maxDepth, CancellationToken cancellationToken) {
                    this._maxDepth = maxDepth;
                    this._CancellationToken = cancellationToken;
                    this._currentDepth = startDepth;
                    this._collections.Enqueue(new CollectionInfo(collection, children, startDepth));
                    this._Current = null;
                }

                public IEntry Current => this._Current ?? throw new ArgumentOutOfRangeException();

                public void Dispose() {
                    this._Current = null;
                    this._entries?.Dispose();
                    this._entries = null;
                }

                public ValueTask DisposeAsync() {
                    this._Current = null;
                    this._entries?.Dispose();
                    this._entries = null;
                    return ValueTask.CompletedTask;
                }

                /*
                public async Task<bool> MoveNext(CancellationToken cancellationToken)
                {
                    var resultFound = false;
                    var hasCurrent = false;

                    while (!resultFound)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (this._entries == null)
                        {
                            var nextCollectionInfo = this._collections.Dequeue();
                            this._collection = nextCollectionInfo.Collection;
                            this._currentDepth = nextCollectionInfo.Depth;
                            var children = nextCollectionInfo.Children ?? await this._collection.GetChildrenAsync(cancellationToken).ConfigureAwait(false);
                            this._entries = children.GetEnumerator();
                        }

                        if (this._entries.MoveNext())
                        {
                            var coll = this._entries.Current as ICollection;
                            if (this._currentDepth < this._maxDepth && coll != null)
                            {
                                IReadOnlyCollection<IEntry> children;
                                try
                                {
                                    children = await coll.GetChildrenAsync(cancellationToken).ConfigureAwait(false);
                                }
                                catch (Exception)
                                {
                                    // Ignore errors
                                    children = new IEntry[0];
                                }

                                var collectionInfo = new CollectionInfo(coll, children, this._currentDepth + 1);
                                this._collections.Enqueue(collectionInfo);
                            }

                            if (this._currentDepth >= 0)
                            {
                                this._Current = this._entries.Current;
                                resultFound = true;
                                hasCurrent = true;
                            }
                        }
                        else
                        {
                            this._Current = null;
                            this._entries.Dispose();
                            this._entries = null;
                            resultFound = this._collections.Count == 0;
                        }
                    }

                    return hasCurrent;
                }
                */

                public async ValueTask<bool> MoveNextAsync() {
                    var resultFound = false;
                    var hasCurrent = false;

                    while (!resultFound) {
                        this._CancellationToken.ThrowIfCancellationRequested();

                        if (this._entries == null) {
                            var nextCollectionInfo = this._collections.Dequeue();
                            this._collection = nextCollectionInfo.Collection;
                            this._currentDepth = nextCollectionInfo.Depth;
                            var children = nextCollectionInfo.Children ?? await this._collection.GetChildrenAsync(this._CancellationToken).ConfigureAwait(false);
                            this._entries = children.GetEnumerator();
                        }

                        if (this._entries.MoveNext()) {
                            var coll = this._entries.Current as ICollection;
                            if (this._currentDepth < this._maxDepth && coll != null) {
                                IReadOnlyCollection<IEntry> children;
                                try {
                                    children = await coll.GetChildrenAsync(CancellationToken.None).ConfigureAwait(false);
                                } catch (Exception) {
                                    // Ignore errors
                                    children = new IEntry[0];
                                }

                                var collectionInfo = new CollectionInfo(coll, children, this._currentDepth + 1);
                                this._collections.Enqueue(collectionInfo);
                            }

                            if (this._currentDepth >= 0) {
                                this._Current = this._entries.Current;
                                resultFound = true;
                                hasCurrent = true;
                            }
                        } else {
                            this._Current = null;
                            this._entries.Dispose();
                            this._entries = null;
                            resultFound = this._collections.Count == 0;
                        }
                    }

                    return hasCurrent;
                }

                private struct CollectionInfo {
                    public CollectionInfo(ICollection collection, IReadOnlyCollection<IEntry>? children, int depth) {
                        this.Collection = collection;
                        this.Children = children;
                        this.Depth = depth;
                    }

                    public ICollection Collection { get; }

                    public IReadOnlyCollection<IEntry>? Children { get; }

                    public int Depth { get; }
                }
            }
        }

        private class NodeInfo : ICollectionNode {
            public NodeInfo(ICollection collection) {
                this.Collection = collection;
            }

            public string Name => this.Collection.Name;

            public ICollection Collection { get; }

            public List<IDocument> Documents { get; } = new List<IDocument>();

            public List<NodeInfo> SubNodes { get; } = new List<NodeInfo>();

            IReadOnlyCollection<ICollectionNode> ICollectionNode.Nodes => this.SubNodes;

            IReadOnlyCollection<IDocument> ICollectionNode.Documents => this.Documents;
        }
    }
}
