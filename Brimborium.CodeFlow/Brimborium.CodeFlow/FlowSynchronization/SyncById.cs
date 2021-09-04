using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public abstract class SyncById : IDisposable, ISyncById {
        internal readonly SyncByType SyncByType;
        private bool _DisposedValue;
        private SyncGroupLock? _CurrentLock;
        private SyncGroupLock? _LastLock;
        private Queue<SyncGroupLock>? _Queue;

        public readonly IIdentity Id;

        protected SyncById(SyncByType syncByType, IIdentity id) {
            this.SyncByType = syncByType;
            this.Id = id;
            this._CurrentLock = null;
            this._Queue = null;
        }


        public async Task<ISyncLock> LockAsync(
            bool exclusiveLock,
            SyncLockCollection? synLockCollection,
            CancellationToken cancellationToken = default) {
            this.StopTimeoutDispose();
            SyncLock result = this.CreateSyncLock(exclusiveLock);
            try {
                var task = this.SetCurrentOrEnqueue(result, exclusiveLock);
                if (task is null) {
                    // quick path no waits
                    await this.OnLock();
                } else {
                    if (cancellationToken == CancellationToken.None) {
                        await task;
                    } else {
                        await task.ContinueWith(t => { }, cancellationToken);
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                }
                if (synLockCollection is not null) {
                    synLockCollection.Add(result);
                }
                return result;
            } catch {
                result.Dispose();
                throw;
            }
        }

        protected abstract SyncLock CreateSyncLock(bool exclusiveLock);

        private Task? SetCurrentOrEnqueue(
                SyncLock item,
                bool exclusiveLock
            ) {
            lock (this) {
                if (this._CurrentLock is null) {
                    var grp = new SyncGroupLock(this, item, exclusiveLock);
                    this._CurrentLock = grp;
                    this._LastLock = grp;
                    grp.SetActive();
                    return null;
                }
                {
                    if ((!exclusiveLock) && (this._LastLock is not null) && (!this._LastLock.ExclusiveLock)) {
                        this._LastLock.AddItem(item);
                        return null;
                    }
                }
                {
                    if ((!exclusiveLock) && (this._CurrentLock is not null) && (!this._CurrentLock.ExclusiveLock) && ((this._Queue is null) || (this._Queue.Count == 0))) {
                        this._CurrentLock.AddItem(item);
                        return null;
                    }
                }
                {
                    var queue = this._Queue ??= new Queue<SyncGroupLock>();
                    if (!exclusiveLock && (this._LastLock is not null) && !this._LastLock.ExclusiveLock) {
                        return this._LastLock.AddItem(item);
                    } else {
                        var grp = new SyncGroupLock(this, item, exclusiveLock);
                        this._LastLock = grp;
                        var task = grp.SetWaiting();
                        queue.Enqueue(grp);
                        return task;
                    }
                }
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (!this._DisposedValue) {
                if (disposing) {
                    if (this._Queue is not null) {
                        while (this._Queue.TryDequeue(out var syncGroupLock)) {
                            syncGroupLock.Dispose();
                        }
                        this._Queue = null;
                    }
                } else {
                }

                this._DisposedValue = true;
            }
        }

        ~SyncById() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal void StartNextGroup(SyncGroupLock syncGroupLockFinished) {
            lock (this) {
                System.Threading.Interlocked.CompareExchange(ref this._CurrentLock, null, syncGroupLockFinished);
                System.Threading.Interlocked.CompareExchange(ref this._LastLock, null, syncGroupLockFinished);
                if (this._CurrentLock is null) {
                    this.OnRelease();
                    if (this._Queue != null) {
                        if (this._Queue.TryDequeue(out var syncGroupLockNext)) {
                            this._CurrentLock = syncGroupLockNext;
                            this.OnLock();
                            syncGroupLockNext.SetActive();
                            return;
                        }
                    }
                    this.StartTimeoutDispose();
                }
            }
        }

        private bool _TimeoutDisposeIsStarted;

        private void StopTimeoutDispose() {
            if (_TimeoutDisposeIsStarted) {
                _TimeoutDisposeIsStarted = false;
                this.SyncByType.StopTimeoutDispose(this);
            }
        }
        private void StartTimeoutDispose() {
            if (!_TimeoutDisposeIsStarted) {
                _TimeoutDisposeIsStarted = true;
                this.SyncByType.StartTimeoutDispose(this);
            }
        }

        protected virtual Task OnLock() {
            return Task.CompletedTask;
        }

        protected virtual Task OnRelease() {
            return Task.CompletedTask;
        }
    }

}
