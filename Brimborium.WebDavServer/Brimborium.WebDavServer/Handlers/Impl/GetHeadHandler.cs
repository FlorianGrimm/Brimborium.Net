using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Handlers.Impl.GetResults;
using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Utils;

namespace Brimborium.WebDavServer.Handlers.Impl
{
    /// <summary>
    /// The implementation of the <see cref="IGetHandler"/> and <see cref="IHeadHandler"/> interfaces
    /// </summary>
    public class GetHeadHandler : IGetHandler, IHeadHandler
    {
        private readonly IWebDavContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetHeadHandler"/> class.
        /// </summary>
        /// <param name="fileSystem">The root file system</param>
        /// <param name="context">The WebDAV context</param>
        public GetHeadHandler(IFileSystem fileSystem, IWebDavContext context)
        {
            this._context = context;
            this.FileSystem = fileSystem;
        }

        /// <inheritdoc />
        public IEnumerable<string> HttpMethods { get; } = new[] { "GET", "HEAD" };

        /// <summary>
        /// Gets the root file system
        /// </summary>
        public IFileSystem FileSystem { get; }

        /// <inheritdoc />
        public Task<IWebDavResult> GetAsync(string path, CancellationToken cancellationToken)
        {
            return this.HandleAsync(path, true, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IWebDavResult> HeadAsync(string path, CancellationToken cancellationToken)
        {
            return this.HandleAsync(path, false, cancellationToken);
        }

        private async Task<IWebDavResult> HandleAsync(
            string path,
            bool returnFile,
            CancellationToken cancellationToken)
        {
            var selectionResult = await this.FileSystem.SelectAsync(path, cancellationToken).ConfigureAwait(false);

            if (selectionResult.IsMissing)
            {
                if (this._context.RequestHeaders.IfNoneMatch != null) {
                    throw new WebDavException(WebDavStatusCode.PreconditionFailed);
                }

                throw new WebDavException(WebDavStatusCode.NotFound);
            }

            await this._context.RequestHeaders
                .ValidateAsync(selectionResult.TargetEntry, cancellationToken).ConfigureAwait(false);

            if (selectionResult.ResultType == SelectionResultType.FoundCollection)
            {
                if (returnFile) {
                    throw new NotSupportedException();
                }

                Debug.Assert(selectionResult.Collection != null, "selectionResult.Collection != null");
                return new WebDavCollectionResult(selectionResult.Collection);
            }

            Debug.Assert(selectionResult.Document != null, "selectionResult.Document != null");

            var doc = selectionResult.Document;
            var rangeHeader = this._context.RequestHeaders.Range;
            if (rangeHeader != null)
            {
                if (rangeHeader.Unit != "bytes") {
                    throw new NotSupportedException();
                }

                var rangeItems = rangeHeader.Normalize(doc.Length);
                if (rangeItems.Any(x => x.Length < 0 || x.To >= doc.Length))
                {
                    return new WebDavResult(WebDavStatusCode.RequestedRangeNotSatisfiable)
                    {
                        Headers =
                        {
                            ["Content-Range"] = new[] { $"bytes */{doc.Length}" },
                        },
                    };
                }

                return new WebDavPartialDocumentResult(doc, returnFile, rangeItems);
            }

            return new WebDavFullDocumentResult(doc, returnFile);
        }
    }
}
