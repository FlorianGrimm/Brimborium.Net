using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public class SyncById : IDisposable {
        private SemaphoreSlim? _SemaphoreSlim;
        internal readonly SyncByType SyncByType;
        private bool _DisposedValue;
        private SyncLock? _CurrentLock;

        public SyncById(SyncByType syncByType) {
            this.SyncByType = syncByType;
            this._CurrentLock = null;
        }

        public async Task<IDisposable> LockAsync(object id, SyncLockCollection? synLockCollection, CancellationToken cancellationToken = default) {
            var semaphoreSlim = this.GetSemaphoreSlim();
            await semaphoreSlim.WaitAsync(cancellationToken);
            var result = new SyncLock(this, id);
            this._CurrentLock = result;
            if (synLockCollection is not null) {
                synLockCollection.Add(result);
            }
            this.StopTimeoutDispose();
            this.OnLock(result);
            return result;
        }

        private SemaphoreSlim GetSemaphoreSlim() {
            if (this._SemaphoreSlim is null) {
                lock (this) {
                    if (this._SemaphoreSlim is null) {
                        var semaphoreSlim = new SemaphoreSlim(1, 1);
                        var oldValue = System.Threading.Interlocked.CompareExchange(ref this._SemaphoreSlim, semaphoreSlim, null);
                        if (oldValue is null) {
                            // OK 
                            System.Threading.Interlocked.MemoryBarrier();
                            return this._SemaphoreSlim;
                        } else {
                            semaphoreSlim.Dispose();
                            return this._SemaphoreSlim;
                        }
                    }
                }
            }
            return this._SemaphoreSlim;
        }

        protected virtual void Dispose(bool disposing) {
            if (!this._DisposedValue) {
                if (disposing) {
                }
                using (var semaphoreSlim = this._SemaphoreSlim) {
                    this._SemaphoreSlim = null;
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

        internal void Release(SyncLock syncLock) {
            var old = System.Threading.Interlocked.CompareExchange(ref this._CurrentLock, null, syncLock);
            if (ReferenceEquals(old, syncLock)) {
                var semaphore = this._SemaphoreSlim;
                if (semaphore is not null) {
                    semaphore.Release();
                    this.StartTimeoutDispose();
                    this.OnRelease(syncLock);
                }
            }
        }

        private void StopTimeoutDispose() {
            this.SyncByType.GetParentSyncDictionary().StopTimeoutDispose(this);
        }

        private void StartTimeoutDispose() {
            this.SyncByType.GetParentSyncDictionary().StartTimeoutDispose(this);
        }

        protected virtual void OnLock(SyncLock result) { }

        protected virtual void OnRelease(SyncLock syncLock) { }
    }
}
