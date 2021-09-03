using System;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public class SyncFactory {
        public SyncFactory() {
        }

        public virtual SyncByType<T> CreateSyncByType<T>(SyncDictionary syncDictionary, Type byType, ISyncItemFactory<T> syncFactory, TimeSpan lifeTimeSpan) {
            return new SyncByType<T>(syncDictionary, byType, syncFactory);
        }

        public virtual SyncById<T> CreateSyncById<T>(SyncByType<T> syncByType, object id) {
            return new SyncById<T>(syncByType, id);
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
