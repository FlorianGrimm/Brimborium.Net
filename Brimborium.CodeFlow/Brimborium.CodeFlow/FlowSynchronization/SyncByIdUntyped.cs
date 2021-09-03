namespace Brimborium.CodeFlow.FlowSynchronization {
    public sealed class SyncByIdUntyped : SyncById {
        public SyncByIdUntyped(SyncByType syncByType, object id) : base(syncByType, id) { }

        protected override SyncLock CreateSyncLock() => new SyncLockUntyped(this);

        ~SyncByIdUntyped() {
            this.Dispose(disposing: false);
        }
    }
}
