using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Brimborium.WebDavServer.Model.Headers;

using Microsoft.Extensions.Logging;

namespace Brimborium.WebDavServer.Locking {
    /// <summary>
    /// A background task that removes expired locks
    /// </summary>
    public class LockCleanupTask : ILockCleanupTask, IDisposable {
        private static readonly TimeSpan _deactivated = TimeSpan.FromMilliseconds(-1);

        private readonly ISystemClock _systemClock;

        private readonly MultiValueDictionary<DateTime, ActiveLockItem> _activeLocks = new MultiValueDictionary<DateTime, ActiveLockItem>();

        private readonly object _syncRoot = new object();

        private readonly Timer _timer;

        private readonly ILogger<LockCleanupTask> _logger;

        private ActiveLockItem? _mostRecentExpirationLockItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="LockCleanupTask"/> class.
        /// </summary>
        /// <param name="systemClock">The system clock</param>
        /// <param name="logger">The logger for the cleanup task</param>
        public LockCleanupTask(
            ISystemClock systemClock,
            ILogger<LockCleanupTask> logger) {
            this._systemClock = systemClock;
            this._logger = logger;
            this._timer = new Timer(this.TimerExpirationCallback, null, _deactivated, _deactivated);
        }

        /// <summary>
        /// Adds a lock to be tracked by this cleanup task.
        /// </summary>
        /// <param name="lockManager">The lock manager that created this active lock.</param>
        /// <param name="activeLock">The active lock to track</param>
        public void Add(ILockManager lockManager, IActiveLock activeLock) {
            if (this._logger.IsEnabled(LogLevel.Trace)) {
                this._logger.LogTrace($"Adding lock {activeLock}");
            }

            // Don't track locks with infinite timeout
            if (activeLock.Timeout == TimeoutHeader.Infinite) {
                return;
            }

            lock (this._syncRoot) {
                var newLockItem = new ActiveLockItem(lockManager, activeLock);
                this._activeLocks.Add(activeLock.Expiration, newLockItem);
                if ((this._mostRecentExpirationLockItem is not null)
                    && (newLockItem.Expiration >= this._mostRecentExpirationLockItem.Expiration)) {
                    // New item is not the most recent to expire
                    if (this._logger.IsEnabled(LogLevel.Debug)) {
                        this._logger.LogDebug($"New lock {activeLock.StateToken} item is not the most recent item");
                    }

                    return;
                }

                this._mostRecentExpirationLockItem = newLockItem;
                this.ConfigureTimer(newLockItem);
            }
        }

        /// <summary>
        /// Removes the active lock so that it isn't tracked any more by this cleanup task.
        /// </summary>
        /// <param name="activeLock">The active lock to remove</param>
        public void Remove(IActiveLock activeLock) {
            if (this._logger.IsEnabled(LogLevel.Trace)) {
                this._logger.LogTrace($"Try removing lock {activeLock}");
            }

            lock (this._syncRoot) {
                IReadOnlyCollection<ActiveLockItem> lockItems;
                if (!this._activeLocks.TryGetValue(activeLock.Expiration, out lockItems)) {
                    // Lock item not found
                    if (this._logger.IsEnabled(LogLevel.Debug)) {
                        this._logger.LogDebug($"Lock {activeLock.StateToken} is not tracked any more.");
                    }

                    return;
                }

                var lockItem = lockItems
                    .FirstOrDefault(x => string.Equals(x.ActiveLock.StateToken, activeLock.StateToken, StringComparison.Ordinal));

                if (lockItem == null) {
                    // Lock item not found
                    if (this._logger.IsEnabled(LogLevel.Debug)) {
                        this._logger.LogDebug($"Lock {activeLock.StateToken} is not tracked any more.");
                    }

                    return;
                }

                // Remove lock item
                this._activeLocks.Remove(lockItem.Expiration, lockItem);

                if ((this._mostRecentExpirationLockItem is not null)
                    && (lockItem.ActiveLock.StateToken == this._mostRecentExpirationLockItem.ActiveLock.StateToken)) {
                    // Removed lock item was the most recent
                    this._mostRecentExpirationLockItem = this.FindMostRecentExpirationItem();
                    if (this._mostRecentExpirationLockItem is not null) {
                        // Found a new one and reconfigure timer
                        this.ConfigureTimer(this._mostRecentExpirationLockItem);
                    } else if (this._logger.IsEnabled(LogLevel.Debug)) {
                        this._logger.LogDebug("No more locks to cleanup.");
                    }
                }
            }
        }

        /// <inheritdoc />
        public void Dispose() {
            this._timer.Dispose();
        }

        /// <summary>
        /// The timer callback which removes an expired item
        /// </summary>
        /// <param name="state">The (unused) state</param>
        private async void TimerExpirationCallback(object? state) {
            bool removeResult;
            ActiveLockItem? lockItem;
            lock (this._syncRoot) {
                lockItem = this._mostRecentExpirationLockItem;
                var now = this._systemClock.UtcNow;

                if (this._logger.IsEnabled(LogLevel.Trace)) {
                    this._logger.LogTrace($"Cleanup timer called at {now:O}");
                }

                ActiveLockItem? nextLockItem;

                // The lock item might be null because a different task might've removed it already
                if (lockItem is null) {
                    if (this._logger.IsEnabled(LogLevel.Debug)) {
                        this._logger.LogDebug("Lock was already removed (no lock found).");
                    }

                    removeResult = false;
                    nextLockItem = this.FindMostRecentExpirationItem();
                } else if (lockItem.Expiration > now) {
                    // The expiration might be in the future, because this timer event might be one
                    // that belongs to an already removed item.
                    if (this._logger.IsEnabled(LogLevel.Debug)) {
                        this._logger.LogDebug($"Lock was already removed (different lock {lockItem.ActiveLock.StateToken} found).");
                    }

                    removeResult = false;
                    nextLockItem = this._mostRecentExpirationLockItem;
                } else {
                    if (this._logger.IsEnabled(LogLevel.Debug)) {
                        this._logger.LogDebug($"Lock {lockItem.ActiveLock.StateToken} will be removed.");
                    }

                    removeResult = this._activeLocks.Remove(lockItem.Expiration, lockItem);
                    nextLockItem = this.FindMostRecentExpirationItem();
                }

                this._mostRecentExpirationLockItem = nextLockItem;
                if (this._mostRecentExpirationLockItem != null) {
                    // There is another lock that needs to be tracked.
                    this.ConfigureTimer(this._mostRecentExpirationLockItem);
                } else if (this._logger.IsEnabled(LogLevel.Debug)) {
                    this._logger.LogDebug("No more locks to cleanup.");
                }
            }

            // The remove might've failed because different task could've removed
            // it before the timer event could get its hands on it.
            if (removeResult) {
                if (lockItem is null) {
                    if (this._logger.IsEnabled(LogLevel.Trace)) {
                        this._logger.LogTrace($"Lock lockItem was null.");
                    }
                } else {
                    if (this._logger.IsEnabled(LogLevel.Trace)) {
                        this._logger.LogTrace($"Lock {lockItem.ActiveLock.StateToken} will now be removed from the lock manager.");
                    }
                    var stateToken = new Uri(lockItem.ActiveLock.StateToken, UriKind.RelativeOrAbsolute);
                    await lockItem.LockManager.ReleaseAsync(lockItem.ActiveLock.Path, stateToken, CancellationToken.None).ConfigureAwait(false);
                }
            }
        }

        private ActiveLockItem? FindMostRecentExpirationItem() {
            var mostRecentExpiration = this._activeLocks.Keys.Select(x => (DateTime?)x).FirstOrDefault();
            if (mostRecentExpiration is not null) {
                var lockItems = this._activeLocks[mostRecentExpiration.Value];
                var nextLockItem = lockItems.FirstOrDefault();
                return nextLockItem;
            }

            return null;
        }

        private void ConfigureTimer(ActiveLockItem lockItem) {
            if (this._logger.IsEnabled(LogLevel.Trace)) {
                this._logger.LogTrace($"Lock {lockItem.ActiveLock.StateToken} is the next to expire.");
            }

            var remainingTime = lockItem.Expiration - this._systemClock.UtcNow;
            if (remainingTime < TimeSpan.Zero) {
                remainingTime = TimeSpan.Zero;
            }

            if (this._logger.IsEnabled(LogLevel.Debug)) {
                this._logger.LogDebug($"Locks {lockItem.ActiveLock.StateToken} remaining time is {remainingTime}.");
            }

            if (this._logger.IsEnabled(LogLevel.Debug)) {
                this._logger.LogDebug($"Lock {lockItem.ActiveLock.StateToken} is expected to expire at time {this._systemClock.UtcNow + remainingTime:O}.");
            }

            this._timer.Change(remainingTime, _deactivated);
        }

        private class ActiveLockItem {
            public ActiveLockItem(ILockManager lockManager, IActiveLock activeLock) {
                this.LockManager = lockManager;
                this.ActiveLock = activeLock;
            }

            public DateTime Expiration => this.ActiveLock.Expiration;

            public ILockManager LockManager { get; }

            public IActiveLock ActiveLock { get; }
        }
    }
}
