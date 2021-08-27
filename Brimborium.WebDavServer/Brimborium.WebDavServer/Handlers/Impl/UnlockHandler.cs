using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.Locking;
using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Model.Headers;

namespace Brimborium.WebDavServer.Handlers.Impl
{
    /// <summary>
    /// The implementation of the <see cref="IUnlockHandler"/> interface
    /// </summary>
    public class UnlockHandler : IUnlockHandler
    {
        private readonly IWebDavContext _context;

        private readonly ILockManager? _lockManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnlockHandler"/> class.
        /// </summary>
        /// <param name="context">The WebDAV request context</param>
        /// <param name="lockManager">The global lock manager</param>
        public UnlockHandler(IWebDavContext context, ILockManager? lockManager = null)
        {
            this._context = context;
            this._lockManager = lockManager;
            this.HttpMethods = this._lockManager == null ? new string[0] : new[] { "UNLOCK" };
        }

        /// <inheritdoc />
        public IEnumerable<string> HttpMethods { get; }

        /// <inheritdoc />
        public async Task<IWebDavResult> UnlockAsync(string path, LockTokenHeader stateToken, CancellationToken cancellationToken)
        {
            if (this._lockManager == null) {
                throw new NotSupportedException();
            }

            var releaseStatus = await this._lockManager.ReleaseAsync(path, stateToken.StateToken, cancellationToken).ConfigureAwait(false);
            if (releaseStatus != LockReleaseStatus.Success)
            {
                var href = new Uri(this._context.PublicControllerUrl, path);
                href = new Uri("/" + this._context.PublicRootUrl.MakeRelativeUri(href).OriginalString);
                return new WebDavResult<error>(
                    WebDavStatusCode.Conflict,
                    new error()
                    {
                        ItemsElementName = new[] { ItemsChoiceType.locktokenmatchesrequesturi, },
                        Items = new object[]
                        {
                            new errorNoconflictinglock()
                            {
                                href = new[] { href.OriginalString },
                            },
                        },
                    });
            }

            return new WebDavResult(WebDavStatusCode.NoContent);
        }
    }
}
