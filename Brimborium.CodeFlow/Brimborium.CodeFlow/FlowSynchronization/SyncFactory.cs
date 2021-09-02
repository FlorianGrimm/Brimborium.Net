using System;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public class SyncFactory {
        public SyncFactory() {
        }

        public virtual SyncByType CreateSyncByTypeGeneral(Type type, SyncDictionary syncDictionary, ISyncItemFactory<object> syncFactory) {
            return new SyncByType<object>(syncDictionary, syncFactory);
        }

        public virtual SyncByType<T> CreateSyncByType<T>(SyncDictionary syncDictionary, ISyncItemFactory<T> syncFactory) {
            return new SyncByType<T>(syncDictionary, syncFactory);
        }

        public virtual SyncById CreateSyncByIdGeneral(SyncByType syncByType) {
            return new SyncById(syncByType);
        }

        public virtual SyncById<T> CreateSyncById<T>(SyncByType syncByType) {
            return new SyncById<T>(syncByType);
        }

        public virtual ISyncItemFactory<object> GetSyncFactory(Type type) {
            return new GenaralSyncItemFactory(type);
        }

        class GenaralSyncItemFactory : ISyncItemFactory<object> {
            private readonly Type _Type;

            public GenaralSyncItemFactory(Type type) {
                this._Type = type;
            }

            public Task<object> CreateItem(object id) {
                object result = System.Activator.CreateInstance(this._Type)
                    ?? throw new InvalidOperationException();
                return Task.FromResult<object>(result);
            }
        }
    }
}
