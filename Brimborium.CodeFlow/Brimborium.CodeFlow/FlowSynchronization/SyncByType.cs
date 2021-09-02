#pragma warning disable IDE0060 // Remove unused parameter

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public abstract class SyncByType : IDisposable {
        private readonly ConcurrentDictionary<object, SyncById> _SyncById;
        private readonly SyncDictionary _ParentSyncDictionary;
        private bool _DisposedValue;

        protected SyncByType(SyncDictionary syncDictionary) {
            this._ParentSyncDictionary = syncDictionary;
            this._SyncById = new ConcurrentDictionary<object, SyncById>();
        }

        private void Dispose(bool disposing) {
            if (!this._DisposedValue) {
                // if (disposing) { }
                var arrKeys = this._SyncById.Keys.ToArray();
                foreach (var key in arrKeys) {
                    if (this._SyncById.TryRemove(key, out var syncById)) {
                        syncById.Dispose();
                    }
                }
                this._DisposedValue = true;
            }
        }

        ~SyncByType() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Task<IDisposable> LockAsync(object id, SyncLockCollection? synLockCollection, CancellationToken cancellationToken = default) {
            var syncById = this.GetSyncById(id);
            return syncById.LockAsync(id, synLockCollection, cancellationToken);
        }

        public SyncDictionary GetParentSyncDictionary() => this._ParentSyncDictionary;
        internal SyncDictionaryOptions Options => this._ParentSyncDictionary.Options;
        protected internal SyncFactory Factory => this._ParentSyncDictionary.Options.Factory;

        private SyncById GetSyncById(object id) {
            while (true) {
                if (!this._SyncById.TryGetValue(id, out var result)) {
                    lock (this) {
                        result = this.CreateSyncById();
                        if (this._SyncById.TryAdd(id, result)) {
                            return result;
                        } else {
                            result.Dispose();
                            continue;
                        }
                    }
                }
                return result;
            }
        }

        protected virtual SyncById CreateSyncById() {
            return this.Factory.CreateSyncByIdGeneral(this);
        }
    }
}
