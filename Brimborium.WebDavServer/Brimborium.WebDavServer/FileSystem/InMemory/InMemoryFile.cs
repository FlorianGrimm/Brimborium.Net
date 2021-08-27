using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Model.Headers;

namespace Brimborium.WebDavServer.FileSystem.InMemory
{
    /// <summary>
    /// An in-memory implementation of a WebDAV document
    /// </summary>
    public class InMemoryFile : InMemoryEntry, IDocument
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryFile"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system this document belongs to</param>
        /// <param name="parent">The parent collection</param>
        /// <param name="path">The root-relative path of this document</param>
        /// <param name="name">The name of this document</param>
        public InMemoryFile(InMemoryFileSystem fileSystem, ICollection parent, Uri path, string name)
            : this(fileSystem, parent, path, name, new byte[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryFile"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system this document belongs to</param>
        /// <param name="parent">The parent collection</param>
        /// <param name="path">The root-relative path of this document</param>
        /// <param name="name">The name of this document</param>
        /// <param name="data">The initial data of this document</param>
        public InMemoryFile(InMemoryFileSystem fileSystem, ICollection parent, Uri path, string name, byte[] data)
            : base(fileSystem, parent, path, name)
        {
            this.Data = new MemoryStream(data);
        }

        /// <inheritdoc />
        public long Length => this.Data.Length;

        /// <summary>
        /// Gets or sets the underlying data
        /// </summary>
        public MemoryStream Data { get; set; }

        /// <inheritdoc />
        public override async Task<DeleteResult> DeleteAsync(CancellationToken cancellationToken)
        {
            if (this.InMemoryFileSystem.IsReadOnly) {
                throw new UnauthorizedAccessException("Failed to modify a read-only file system");
            }

            if (this.InMemoryParent == null) {
                throw new InvalidOperationException("The document must belong to a collection");
            }

            if (this.InMemoryParent.Remove(this.Name))
            {
                var propStore = this.FileSystem.PropertyStore;
                if (propStore != null)
                {
                    await propStore.RemoveAsync(this, cancellationToken).ConfigureAwait(false);
                }

                return new DeleteResult(WebDavStatusCode.OK, null);
            }

            return new DeleteResult(WebDavStatusCode.NotFound, this);
        }

        /// <inheritdoc />
        public Task<Stream> OpenReadAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<Stream>(new MemoryStream(this.Data.ToArray()));
        }

        /// <inheritdoc />
        public Task<Stream> CreateAsync(CancellationToken cancellationToken)
        {
            if (this.InMemoryFileSystem.IsReadOnly) {
                throw new UnauthorizedAccessException("Failed to modify a read-only file system");
            }

            return Task.FromResult<Stream>(this.Data = new MyMemoryStream(this));
        }

        /// <inheritdoc />
        public async Task<IDocument> CopyToAsync(ICollection collection, string name, CancellationToken cancellationToken)
        {
            var coll = (InMemoryDirectory)collection;
            coll.Remove(name);

            var doc = (InMemoryFile)await coll.CreateDocumentAsync(name, cancellationToken).ConfigureAwait(false);
            doc.Data = new MemoryStream(this.Data.ToArray());
            doc.CreationTimeUtc = this.CreationTimeUtc;
            doc.LastWriteTimeUtc = this.LastWriteTimeUtc;
            doc.ETag = this.ETag;

            var sourcePropStore = this.FileSystem.PropertyStore;
            var destPropStore = collection.FileSystem.PropertyStore;
            if (sourcePropStore != null && destPropStore != null)
            {
                var sourceProps = await sourcePropStore.GetAsync(this, cancellationToken).ConfigureAwait(false);
                await destPropStore.RemoveAsync(doc, cancellationToken).ConfigureAwait(false);
                await destPropStore.SetAsync(doc, sourceProps, cancellationToken).ConfigureAwait(false);
            }
            else if (destPropStore != null)
            {
                await destPropStore.RemoveAsync(doc, cancellationToken).ConfigureAwait(false);
            }

            return doc;
        }

        /// <inheritdoc />
        public async Task<IDocument> MoveToAsync(ICollection collection, string name, CancellationToken cancellationToken)
        {
            if (this.InMemoryFileSystem.IsReadOnly) {
                throw new UnauthorizedAccessException("Failed to modify a read-only file system");
            }

            var sourcePropStore = this.FileSystem.PropertyStore;
            var destPropStore = collection.FileSystem.PropertyStore;

            IReadOnlyCollection<XElement>? sourceProps;
            if (sourcePropStore != null && destPropStore != null)
            {
                sourceProps = await sourcePropStore.GetAsync(this, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                sourceProps = null;
            }

            var coll = (InMemoryDirectory)collection;
            var doc = (InMemoryFile)await coll.CreateDocumentAsync(name, cancellationToken).ConfigureAwait(false);
            doc.Data = new MemoryStream(this.Data.ToArray());
            doc.CreationTimeUtc = this.CreationTimeUtc;
            doc.LastWriteTimeUtc = this.LastWriteTimeUtc;
            doc.ETag = this.ETag;
            Debug.Assert(this.InMemoryParent != null, "InMemoryParent != null");
            if (this.InMemoryParent == null) {
                throw new InvalidOperationException("The document must belong to a collection");
            }

            if (!this.InMemoryParent.Remove(this.Name)) {
                throw new InvalidOperationException("Failed to remove the document from the source collection.");
            }

            if (destPropStore != null)
            {
                await destPropStore.RemoveAsync(doc, cancellationToken).ConfigureAwait(false);

                if (sourceProps != null)
                {
                    await destPropStore.SetAsync(doc, sourceProps, cancellationToken).ConfigureAwait(false);
                }
            }

            return doc;
        }

        private class MyMemoryStream : MemoryStream
        {
            private readonly InMemoryFile _file;

            public MyMemoryStream(InMemoryFile file)
            {
                this._file = file;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this._file.Data = new MemoryStream(this.ToArray());
                    this._file.ETag = new EntityTag(false);
                }

                base.Dispose(disposing);
            }
        }
    }
}
