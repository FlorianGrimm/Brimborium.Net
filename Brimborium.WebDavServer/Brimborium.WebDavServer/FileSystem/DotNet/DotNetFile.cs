﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.Model;

namespace Brimborium.WebDavServer.FileSystem.DotNet {
    /// <summary>
    /// A .NET <see cref="System.IO"/> based implementation of a WebDAV document
    /// </summary>
    public class DotNetFile : DotNetEntry, IDocument {
        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetFile"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system this document belongs to</param>
        /// <param name="parent">The parent collection</param>
        /// <param name="info">The file information</param>
        /// <param name="path">The root-relative path of this document</param>
        public DotNetFile(DotNetFileSystem fileSystem, ICollection parent, FileInfo info, Uri path)
            : base(fileSystem, parent, info, path, null) {
            this.FileInfo = info;
        }

        /// <summary>
        /// Gets the file information
        /// </summary>
        public FileInfo FileInfo { get; }

        /// <inheritdoc />
        public long Length => this.FileInfo.Length;

        /// <inheritdoc />
        public Task<Stream> OpenReadAsync(CancellationToken cancellationToken) {
            return Task.FromResult<Stream>(this.FileInfo.OpenRead());
        }

        /// <inheritdoc />
        public async Task<Stream> CreateAsync(CancellationToken cancellationToken) {
            if (this.FileSystem.PropertyStore != null) {
                await this.FileSystem.PropertyStore.UpdateETagAsync(this, cancellationToken).ConfigureAwait(false);
            }

            return this.FileInfo.Open(FileMode.Create, FileAccess.Write);
        }

        /// <inheritdoc />
        public override async Task<DeleteResult> DeleteAsync(CancellationToken cancellationToken) {
            this.FileInfo.Delete();

            var propStore = this.FileSystem.PropertyStore;
            if (propStore != null) {
                await propStore.RemoveAsync(this, cancellationToken).ConfigureAwait(false);
            }

            return new DeleteResult(WebDavStatusCode.OK, null);
        }

        /// <inheritdoc />
        public async Task<IDocument> CopyToAsync(ICollection collection, string name, CancellationToken cancellationToken) {
            var dir = (DotNetDirectory)collection;
            var targetFileName = System.IO.Path.Combine(dir.DirectoryInfo.FullName, name);
            File.Copy(this.FileInfo.FullName, targetFileName, true);
            var fileInfo = new FileInfo(targetFileName);
            var doc = new DotNetFile(dir.DotNetFileSystem, dir, fileInfo, dir.Path.Append(fileInfo.Name, false));

            var sourcePropStore = this.FileSystem.PropertyStore;
            var destPropStore = collection.FileSystem.PropertyStore;
            if (sourcePropStore != null && destPropStore != null) {
                var sourceProps = await sourcePropStore.GetAsync(this, cancellationToken).ConfigureAwait(false);
                await destPropStore.RemoveAsync(doc, cancellationToken).ConfigureAwait(false);
                await destPropStore.SetAsync(doc, sourceProps, cancellationToken).ConfigureAwait(false);
            } else if (destPropStore != null) {
                await destPropStore.RemoveAsync(doc, cancellationToken).ConfigureAwait(false);
            }

            return doc;
        }

        /// <inheritdoc />
        public async Task<IDocument> MoveToAsync(ICollection collection, string name, CancellationToken cancellationToken) {
            var sourcePropStore = this.FileSystem.PropertyStore;
            var destPropStore = collection.FileSystem.PropertyStore;

            IReadOnlyCollection<XElement>? sourceProps;
            if (sourcePropStore != null && destPropStore != null) {
                sourceProps = await sourcePropStore.GetAsync(this, cancellationToken).ConfigureAwait(false);
            } else {
                sourceProps = null;
            }

            var dir = (DotNetDirectory)collection;
            var targetFileName = System.IO.Path.Combine(dir.DirectoryInfo.FullName, name);
            if (File.Exists(targetFileName)) {
                File.Delete(targetFileName);
            }

            File.Move(this.FileInfo.FullName, targetFileName);
            var fileInfo = new FileInfo(targetFileName);
            var doc = new DotNetFile(dir.DotNetFileSystem, dir, fileInfo, dir.Path.Append(fileInfo.Name, false));

            if (destPropStore != null) {
                await destPropStore.RemoveAsync(doc, cancellationToken).ConfigureAwait(false);

                if (sourceProps != null) {
                    await destPropStore.SetAsync(doc, sourceProps, cancellationToken).ConfigureAwait(false);
                }
            }

            return doc;
        }
    }
}
