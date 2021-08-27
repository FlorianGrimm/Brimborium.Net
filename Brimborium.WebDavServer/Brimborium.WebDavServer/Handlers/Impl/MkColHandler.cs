using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Locking;
using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Model.Headers;
using Brimborium.WebDavServer.Props;

namespace Brimborium.WebDavServer.Handlers.Impl {
    /// <summary>
    /// Implementation of the <see cref="IMkColHandler"/> interface.
    /// </summary>
    public class MkColHandler : IMkColHandler {
        private readonly IFileSystem _rootFileSystem;
        private readonly IWebDavContext _context;
        private readonly IEntryPropertyInitializer _entryPropertyInitializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MkColHandler"/> class.
        /// </summary>
        /// <param name="rootFileSystem">The root file system</param>
        /// <param name="context">The WebDAV request context</param>
        /// <param name="entryPropertyInitializer">The property initializer</param>
        public MkColHandler(IFileSystem rootFileSystem, IWebDavContext context, IEntryPropertyInitializer entryPropertyInitializer) {
            this._rootFileSystem = rootFileSystem;
            this._context = context;
            this._entryPropertyInitializer = entryPropertyInitializer;
        }

        /// <inheritdoc />
        public IEnumerable<string> HttpMethods { get; } = new[] { "MKCOL" };

        /// <inheritdoc />
        public async Task<IWebDavResult> MkColAsync(string path, CancellationToken cancellationToken) {
            var selectionResult = await this._rootFileSystem.SelectAsync(path, cancellationToken).ConfigureAwait(false);
            if (!selectionResult.IsMissing) {
                throw new WebDavException(WebDavStatusCode.Forbidden);
            }

            Debug.Assert(selectionResult.MissingNames != null, "selectionResult.PathEntries != null");
            if (selectionResult.MissingNames.Count != 1) {
                throw new WebDavException(WebDavStatusCode.Conflict);
            }

            if (this._context.RequestHeaders.IfNoneMatch != null) {
                throw new WebDavException(WebDavStatusCode.PreconditionFailed);
            }

            var lockRequirements = new Lock(
                new Uri(path, UriKind.Relative),
                this._context.PublicRelativeRequestUrl,
                false,
                new XElement(WebDavXml.Dav + "owner", this._context.User.Identity?.Name ?? string.Empty),
                LockAccessType.Write,
                LockShareMode.Shared,
                TimeoutHeader.Infinite);
            var lockManager = this._rootFileSystem.LockManager;
            var tempLock = lockManager == null
                ? new ImplicitLock(true)
                : await lockManager.LockImplicitAsync(this._rootFileSystem, this._context.RequestHeaders.If?.Lists, lockRequirements, cancellationToken)
                                   .ConfigureAwait(false);
            if (!tempLock.IsSuccessful) {
                return tempLock.CreateErrorResponse();
            }

            try {
                var newName = selectionResult.MissingNames.Single();
                var collection = selectionResult.Collection;
                Debug.Assert(collection != null, "collection != null");
                try {
                    var newCollection = await collection.CreateCollectionAsync(newName, cancellationToken)
                        .ConfigureAwait(false);
                    if (newCollection.FileSystem.PropertyStore != null) {
                        await this._entryPropertyInitializer.CreatePropertiesAsync(
                                newCollection,
                                newCollection.FileSystem.PropertyStore,
                                this._context,
                                cancellationToken)
                            .ConfigureAwait(false);
                    }
                } catch (Exception ex) {
                    throw new WebDavException(WebDavStatusCode.Forbidden, ex);
                }

                return new WebDavResult(WebDavStatusCode.Created);
            } finally {
                await tempLock.DisposeAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
