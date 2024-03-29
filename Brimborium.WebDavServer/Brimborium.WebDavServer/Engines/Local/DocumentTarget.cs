﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.FileSystem;

namespace Brimborium.WebDavServer.Engines.Local
{
    /// <summary>
    /// The local file system document target
    /// </summary>
    public class DocumentTarget : EntryTarget, IDocumentTarget<CollectionTarget, DocumentTarget, MissingTarget>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentTarget"/> class.
        /// </summary>
        /// <param name="parent">The parent collection</param>
        /// <param name="destinationUrl">The destination URL for this collection</param>
        /// <param name="document">The underlying document</param>
        /// <param name="targetActions">The target actions implementation to use</param>
        public DocumentTarget(
            CollectionTarget parent,
            Uri destinationUrl,
            IDocument document,
            ITargetActions<CollectionTarget, DocumentTarget, MissingTarget> targetActions)
            : base(targetActions, parent, destinationUrl, document)
        {
            this.Document = document;
        }

        /// <summary>
        /// Gets the underlying document
        /// </summary>
        public IDocument Document { get; }

        /// <summary>
        /// Gets the parent collection target
        /// </summary>
        public new CollectionTarget Parent => base.Parent!;

        /// <summary>
        /// Creates a new instance of the <see cref="DocumentTarget"/> class.
        /// </summary>
        /// <param name="destinationUrl">The destination URL for this document</param>
        /// <param name="document">The underlying document</param>
        /// <param name="targetActions">The target actions implementation to use</param>
        /// <returns>The created document target object</returns>
        public static DocumentTarget NewInstance(
            Uri destinationUrl,
            IDocument document,
            ITargetActions<CollectionTarget, DocumentTarget, MissingTarget> targetActions)
        {
            var collUrl = destinationUrl.GetCollectionUri();
            Debug.Assert(document.Parent != null, "document.Parent != null");
            if (document.Parent == null) {
                throw new InvalidOperationException("A document must always have a parent collection.");
            }

            var collTarget = new CollectionTarget(collUrl, null, document.Parent, false, targetActions);
            var docTarget = new DocumentTarget(collTarget, destinationUrl, document, targetActions);
            return docTarget;
        }

        /// <inheritdoc />
        public async Task<MissingTarget> DeleteAsync(CancellationToken cancellationToken)
        {
            await this.Document.DeleteAsync(cancellationToken).ConfigureAwait(false);
            return new MissingTarget(this.DestinationUrl, this.Name, this.Parent, this.TargetActions);
        }
    }
}
