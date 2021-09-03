namespace Brimborium.CodeFlow.FlowSynchronization {

    public class SyncByType<T> : SyncByType {
        private readonly ISyncItemFactory<T> _SyncItemFactory;

        public SyncByType(SyncDictionary syncDictionary, ISyncItemFactory<T> syncFactory)
            : base(syncDictionary) {
            this._SyncItemFactory = syncFactory;
        }

        public ISyncItemFactory<T> SyncItemFactory => this._SyncItemFactory;

        protected override SyncById CreateSyncById(object id) {
            return this.Factory.CreateSyncById<T>(this, id);
        }
    }

}
