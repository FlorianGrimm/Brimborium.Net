using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public record SyncDictionaryOptions(
        SyncFactory Factory,
        bool AllowUnregistered,
        TimeSpan DefaultLifeTimeSpan
    );
    public class SyncDictionary : IDisposable {
        private readonly SyncDictionaryOptions _Options;
        private readonly SyncTimer? _SyncTimer;
        private ConcurrentDictionary<Type, SyncByType> _SyncByType;
        private bool _DisposedValue;

        public SyncDictionary(
            SyncDictionaryOptions options,
            SyncTimer? syncTimer
            ) : base() {
            this._SyncByType = new ConcurrentDictionary<Type, SyncByType>();
            this._Options = options;
            this._SyncTimer = syncTimer;
        }
        
        internal SyncDictionaryOptions Options => this._Options;

        private SyncByType GetSyncByType(Type type) {
            while (true) {
                if (!this._SyncByType.TryGetValue(type, out var syncByType)) {
                    if (this.Options.AllowUnregistered) {
                        ISyncItemFactory<object> syncFactory = this.Options.Factory.GetSyncFactory(type);
                        syncByType = this.Options.Factory.CreateSyncByTypeGeneral(type, this, syncFactory);
                        if (!this._SyncByType.TryAdd(type, syncByType)) {
                            syncByType.Dispose();
                            continue;
                        }
                    } else {
                        throw new ArgumentException($"Type ${type.FullName} not allowed");
                    }
                }
                return syncByType;
            }
        }

        public Task<IDisposable> LockAsync(Type type, string id, SyncLockCollection? synLockCollection, CancellationToken cancellationToken = default) {
            var syncByType = this.GetSyncByType(type);
            return syncByType.LockAsync(id, synLockCollection, cancellationToken);
        }

        private void Dispose(bool disposing) {
            if (!this._DisposedValue) {
                // if (disposing) { }
                var arrKeys = this._SyncByType.Keys.ToArray();
                foreach (var key in arrKeys) {
                    if (this._SyncByType.TryRemove(key, out var syncById)) {
                        syncById.Dispose();
                    }
                }
                this._DisposedValue = true;
            }
        }

        ~SyncDictionary() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public bool Add<T>(ISyncItemFactory<T> creator) {
            while (true) {
                if (!this._SyncByType.TryGetValue(typeof(T), out var syncByType)) {
                    syncByType = this.Options.Factory.CreateSyncByType<T>(this, creator);
                    if (!this._SyncByType.TryAdd(typeof(T), syncByType)) {
                        syncByType.Dispose();
                        continue;
                    }
                    return true;
                }
                return false;
            }
        }

        internal void StopTimeoutDispose(SyncById syncById) {
        }

        internal void StartTimeoutDispose(SyncById syncById) {
        }
    }

}
