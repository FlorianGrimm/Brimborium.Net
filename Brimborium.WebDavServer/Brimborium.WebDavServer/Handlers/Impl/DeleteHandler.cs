using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Locking;
using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Model.Headers;
using Brimborium.WebDavServer.Utils;

namespace Brimborium.WebDavServer.Handlers.Impl
{
    /// <summary>
    /// The implementation of the <see cref="IDeleteHandler"/> interface
    /// </summary>
    public class DeleteHandler : IDeleteHandler
    {
        private readonly IFileSystem _rootFileSystem;

        private readonly IWebDavContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteHandler"/> class.
        /// </summary>
        /// <param name="rootFileSystem">The root file system</param>
        /// <param name="context">The current WebDAV context</param>
        public DeleteHandler(IFileSystem rootFileSystem, IWebDavContext context)
        {
            this._rootFileSystem = rootFileSystem;
            this._context = context;
        }

        /// <inheritdoc />
        public IEnumerable<string> HttpMethods { get; } = new[] { "DELETE" };

        /// <inheritdoc />
        public async Task<IWebDavResult> DeleteAsync(string path, CancellationToken cancellationToken)
        {
            var selectionResult = await this._rootFileSystem.SelectAsync(path, cancellationToken).ConfigureAwait(false);
            if (selectionResult.IsMissing)
            {
                if (this._context.RequestHeaders.IfNoneMatch != null) {
                    throw new WebDavException(WebDavStatusCode.PreconditionFailed);
                }

                throw new WebDavException(WebDavStatusCode.NotFound);
            }

            var targetEntry = selectionResult.TargetEntry;
            Debug.Assert(targetEntry != null, "targetEntry != null");

            await this._context.RequestHeaders
                .ValidateAsync(selectionResult.TargetEntry, cancellationToken).ConfigureAwait(false);

            var lockRequirements = new Lock(
                new Uri(path, UriKind.Relative),
                this._context.PublicRelativeRequestUrl,
                selectionResult.ResultType == SelectionResultType.FoundCollection,
                new XElement(WebDavXml.Dav + "owner", this._context.User.Identity?.Name ?? string.Empty),
                LockAccessType.Write,
                LockShareMode.Exclusive,
                TimeoutHeader.Infinite);
            var lockManager = this._rootFileSystem.LockManager;
            var tempLock = lockManager == null
                ? new ImplicitLock(true)
                : await lockManager.LockImplicitAsync(this._rootFileSystem, this._context.RequestHeaders.If?.Lists, lockRequirements, cancellationToken)
                                   .ConfigureAwait(false);
            if (!tempLock.IsSuccessful) {
                return tempLock.CreateErrorResponse();
            }

            try
            {
                DeleteResult deleteResult;

                try
                {
                    deleteResult = await targetEntry.DeleteAsync(cancellationToken).ConfigureAwait(false);
                    if (targetEntry.FileSystem.PropertyStore != null)
                    {
                        // Remove dead properties (if there are any)
                        await targetEntry
                            .FileSystem.PropertyStore.RemoveAsync(targetEntry, cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
                catch
                {
                    deleteResult = new DeleteResult(WebDavStatusCode.Forbidden, targetEntry);
                }

                var result = new multistatus()
                {
                    response = new[]
                    {
                        new response()
                        {
                            href = this._context.PublicControllerUrl
                                .Append((deleteResult.FailedEntry ?? targetEntry).Path).OriginalString,
                            ItemsElementName = new[] { ItemsChoiceType2.status, },
                            Items = new object[]
                            {
                                new Status(this._context.RequestProtocol, deleteResult.StatusCode).ToString(),
                            },
                        },
                    },
                };

                if (lockManager != null)
                {
                    var locksToRemove = await lockManager
                        .GetAffectedLocksAsync(path, true, false, cancellationToken)
                        .ConfigureAwait(false);
                    foreach (var activeLock in locksToRemove)
                    {
                        await lockManager.ReleaseAsync(
                                activeLock.Path,
                                new Uri(activeLock.StateToken),
                                cancellationToken)
                            .ConfigureAwait(false);
                    }
                }

                return new WebDavResult<multistatus>(WebDavStatusCode.MultiStatus, result);
            }
            finally
            {
                await tempLock.DisposeAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
