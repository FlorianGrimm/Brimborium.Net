using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public sealed class SyncGroupLock : IDisposable {
        private TaskCompletionSource<object>? _WaitingToRun;
        private SyncLockMode _LockMode;
        private bool _DisposedValue;
        private readonly List<SyncLock> _Items;
        private readonly SyncById _Owner;

        public bool ExclusiveLock { get; }

        public SyncGroupLock(SyncById owner, SyncLock item, bool exclusiveLock) {
            this._Items = new List<SyncLock>();
            this._Items.Add(item);
            this._Owner = owner;
            this.ExclusiveLock = exclusiveLock;
        }

        internal Task? SetWaiting() {
            lock (this) {
                if (this._LockMode == SyncLockMode.Initialized) {
                    if (this._WaitingToRun is null) {
                        this._WaitingToRun = new TaskCompletionSource<object>();
                        this._LockMode = SyncLockMode.Waiting;
                        foreach (var item in this._Items) {
                            item.SetWaiting();
                        }
                        System.Threading.Interlocked.MemoryBarrier();
                    }
                    return this._WaitingToRun.Task;
                } else {
                    return null;
                }
            }
        }

        internal bool SetActive() {
            lock (this) {
                if (this._Items.Count == 0) {
                    return false;
                }
                if (this._LockMode == SyncLockMode.Initialized) {
                    this._LockMode = SyncLockMode.Active;
                    System.Threading.Interlocked.MemoryBarrier();
                    // return true;
                } else if (this._LockMode == SyncLockMode.Waiting) {
                    if (this._WaitingToRun is not null) {
                        this._WaitingToRun.SetResult(this);
                        this._LockMode = SyncLockMode.Active;
                        System.Threading.Interlocked.MemoryBarrier();
                    }
                    // return true;
                } else {
                    return false;
                }
                {
                    foreach (var item in this._Items) {
                        item.SetActive(this);
                    }
                    return true;
                }
            }
        }

        internal void Release(SyncLock syncLock) {
            lock (this) {
                bool isFinished = true;
                foreach (var item in this._Items) {
                    if (!item.IsFinished()) {
                        isFinished = false;
                        break;
                    }
                }
                if (isFinished) {
                    this._Owner.StartNextGroup(this);
                }
            }
        }

        internal bool SetFinish() {
            lock (this) {
                if (this._LockMode != SyncLockMode.Finished) {
                    this._WaitingToRun = null;
                    this._LockMode = SyncLockMode.Finished;
                    System.Threading.Interlocked.MemoryBarrier();
                    return true;
                } else {
                    return false;
                }
            }
        }

        public Task GetTask() {
            if ((this._LockMode == SyncLockMode.Active)
                || (this._LockMode == SyncLockMode.Finished)) {
                return Task.CompletedTask;
            }
            while (this._WaitingToRun is null) {
                if (System.Threading.Interlocked.CompareExchange<TaskCompletionSource<object>?>(ref this._WaitingToRun, new TaskCompletionSource<object>(), null) is null) {
                    System.Threading.Interlocked.MemoryBarrier();
                }
            }
            return this._WaitingToRun.Task;
        }

        private void Dispose(bool disposing) {
            if (!_DisposedValue) {
                if (disposing) {
                    this._Items.Clear();
                    this._LockMode = SyncLockMode.Finished;
                } else { 
                    // log this
                }
                this._WaitingToRun = null;
                _DisposedValue = true;
            }
        }

        ~SyncGroupLock() {
            Dispose(disposing: false);
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal Task? AddItem(SyncLock item) {
            lock (this) {
                if (this._LockMode == SyncLockMode.Initialized) {
                    this._Items.Add(item);
                    return null;
                }
                if (this._LockMode == SyncLockMode.Waiting) {
                    this._Items.Add(item);
                    item.SetWaiting();
                    return this.GetTask();
                }
                if (this._LockMode == SyncLockMode.Active) {
                    this._Items.Add(item);
                    item.SetActive(this);
                    return Task.CompletedTask;
                }
                if (this._LockMode == SyncLockMode.Finished) {
                    throw new InvalidOperationException("Group is already finished.");
                }
                throw new InvalidOperationException("Unexpected LockMode.");
            }
        }
    }
}
