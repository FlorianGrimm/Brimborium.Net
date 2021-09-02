﻿namespace Brimborium.CodeFlow.FlowSynchronization {

    public class SyncByType<T> : SyncByType {
        private readonly ISyncItemFactory<T> _SyncItemFactory;

        public SyncByType(SyncDictionary syncDictionary, ISyncItemFactory<T> syncFactory)
            : base(syncDictionary) {
            this._SyncItemFactory = syncFactory;
        }

        protected override SyncById CreateSyncById() {
            return this.Factory.CreateSyncById<T>(this);
        }
    }

}