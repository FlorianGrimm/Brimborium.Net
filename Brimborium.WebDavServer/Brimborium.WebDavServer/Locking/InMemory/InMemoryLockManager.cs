using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Brimborium.WebDavServer.Locking.InMemory {
    /// <summary>
    /// An in-memory implementation of a lock manager
    /// </summary>
    public class InMemoryLockManager : LockManagerBase {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        private IImmutableDictionary<string, IActiveLock> _locks = ImmutableDictionary<string, IActiveLock>.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryLockManager"/> class.
        /// </summary>
        /// <param name="options">The options of the lock manager</param>
        /// <param name="cleanupTask">The clean-up task for expired locks</param>
        /// <param name="systemClock">The system clock interface</param>
        /// <param name="logger">The logger</param>
        public InMemoryLockManager(IOptions<InMemoryLockManagerOptions> options, ILockCleanupTask cleanupTask, ISystemClock systemClock, ILogger<InMemoryLockManager> logger)
            : base(cleanupTask, systemClock, logger, options.Value) {
        }

        /// <inheritdoc />
        protected override async Task<ILockManagerTransaction> BeginTransactionAsync(CancellationToken cancellationToken) {
            await this._semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            return new InMemoryTransaction(this);
        }

        private class InMemoryTransaction : ILockManagerTransaction {
            private readonly InMemoryLockManager _lockManager;

            private IImmutableDictionary<string, IActiveLock> _locks;

            /// <summary>
            /// Initializes a new instance of the <see cref="InMemoryTransaction"/> class.
            /// </summary>
            /// <param name="lockManager">The lock manager that stores the locks</param>
            public InMemoryTransaction(InMemoryLockManager lockManager) {
                this._lockManager = lockManager;
                this._locks = lockManager._locks;
            }

            /// <inheritdoc />
            public Task<IReadOnlyCollection<IActiveLock>> GetActiveLocksAsync(CancellationToken cancellationToken) {
                return Task.FromResult<IReadOnlyCollection<IActiveLock>>(this._locks.Values.ToList());
            }

            /// <inheritdoc />
            public Task<bool> AddAsync(IActiveLock activeLock, CancellationToken cancellationToken) {
                if (this._locks.ContainsKey(activeLock.StateToken)) {
                    return Task.FromResult(false);
                }

                this._locks = this._locks.Add(activeLock.StateToken, activeLock);
                return Task.FromResult(true);
            }

            /// <inheritdoc />
            public Task<bool> UpdateAsync(IActiveLock activeLock, CancellationToken cancellationToken) {
                var hadKey = this._locks.ContainsKey(activeLock.StateToken);
                if (hadKey) {
                    this._locks = this._locks.Remove(activeLock.StateToken);
                }

                this._locks = this._locks.Add(activeLock.StateToken, activeLock);
                return Task.FromResult(hadKey);
            }

            /// <inheritdoc />
            public Task<bool> RemoveAsync(string stateToken, CancellationToken cancellationToken) {
                if (!this._locks.ContainsKey(stateToken)) {
                    return Task.FromResult(false);
                }

                this._locks = this._locks.Remove(stateToken);
                return Task.FromResult(true);
            }

            /// <inheritdoc />
            public Task<IActiveLock?> GetAsync(string stateToken, CancellationToken cancellationToken) {
                this._locks.TryGetValue(stateToken, out var activeLock);
                return Task.FromResult<IActiveLock?>(activeLock);
            }

            /// <inheritdoc />
            public Task CommitAsync(CancellationToken cancellationToken) {
                this._lockManager._locks = this._locks;
                return Task.FromResult(0);
            }

            /// <inheritdoc />
            public void Dispose() {
                this._lockManager._semaphore.Release();
            }
        }
    }
}
