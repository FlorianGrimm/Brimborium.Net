using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.FlowSynchronization {

    public class SyncByType<T> : SyncByType {
        private readonly ISyncItemFactory<T> _SyncItemFactory;

        public SyncByType(SyncDictionary syncDictionary, Type byType, ISyncItemFactory<T> syncFactory)
            : base(syncDictionary, byType) {
            this._SyncItemFactory = syncFactory;
        }

        public ISyncItemFactory<T> SyncItemFactory => this._SyncItemFactory;

        protected override SyncById CreateSyncById(object id) {
            return this.Factory.CreateSyncById<T>(this, id);
        }
    }

}
