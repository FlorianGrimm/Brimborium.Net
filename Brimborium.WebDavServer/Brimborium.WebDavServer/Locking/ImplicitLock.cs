using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.Model;

namespace Brimborium.WebDavServer.Locking {
    /// <summary>
    /// Implementation of the <see cref="IImplicitLock"/> interface
    /// </summary>
    public class ImplicitLock : IImplicitLock {
        private readonly ILockManager? _lockManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplicitLock"/> class.
        /// </summary>
        /// <param name="isSuccess"><see langword="false"/> = All <c>If</c> header conditions failed,
        /// <see langword="true"/> = No lock manager, but still OK</param>
        public ImplicitLock(bool isSuccess = false) {
            this.IsSuccessful = isSuccess;
            this.OwnedLocks = new IActiveLock[0];
            this.ConflictingLocks = new IActiveLock[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplicitLock"/> class.
        /// </summary>
        /// <param name="ownedLocks">The locks matched by the <c>If</c> header</param>
        public ImplicitLock(IReadOnlyCollection<IActiveLock> ownedLocks) {
            this.OwnedLocks = ownedLocks;
            this.ConflictingLocks = new IActiveLock[0];
            this.IsSuccessful = true;
            this.IsTemporaryLock = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplicitLock"/> class.
        /// </summary>
        /// <param name="lockManager">The lock manager</param>
        /// <param name="lockResult">Either the implicit lock or the conflicting locks</param>
        public ImplicitLock(ILockManager lockManager, LockResult lockResult) {
            this._lockManager = lockManager;
            if (lockResult.Lock != null) {
                this.OwnedLocks = new[] { lockResult.Lock };
            }

            this.IsSuccessful = lockResult.Lock != null;
            this.OwnedLocks = new IActiveLock[0];
            this.ConflictingLocks = ((IReadOnlyCollection<IActiveLock>?)lockResult.ConflictingLocks?.GetLocks().ToList()) ?? (new IActiveLock[0]);
            this.IsTemporaryLock = true;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IActiveLock> OwnedLocks { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IActiveLock> ConflictingLocks { get; }

        /// <inheritdoc />
        public bool IsTemporaryLock { get; }

        /// <inheritdoc />
        public bool IsSuccessful { get; }

        /// <inheritdoc />
        public IWebDavResult CreateErrorResponse() {
            if (this.IsSuccessful) {
                throw new InvalidOperationException("No error to create a response for.");
            }

            if (this.ConflictingLocks == null) {
                // No "If" header condition succeeded, but we didn't ask for a lock
                return new WebDavResult(WebDavStatusCode.NotFound);
            }

            // An "If" header condition succeeded, but we couldn't find a matching lock.
            // Obtaining a temporary lock failed.
            var error = new error() {
                ItemsElementName = new[] { ItemsChoiceType.locktokensubmitted, },
                Items = new object[]
                {
                    new errorLocktokensubmitted()
                    {
                        href = this.ConflictingLocks.Select(x => x.Href).ToArray(),
                    },
                },
            };

            return new WebDavResult<error>(WebDavStatusCode.Locked, error);
        }

        /// <inheritdoc />
        public Task DisposeAsync(CancellationToken cancellationToken) {
            if (!this.IsTemporaryLock) {
                return Task.FromResult(0);
            }

            // A temporary lock is always on its own
            var l = this.OwnedLocks.Single();

            // Ignore errors, because the only error that may happen
            // is, that the lock already expired.
            if (this._lockManager is not null) {
                return this._lockManager.ReleaseAsync(l.Path, new Uri(l.StateToken, UriKind.RelativeOrAbsolute), cancellationToken);
            } else {
                return Task.CompletedTask;
            }
        }
    }
}
