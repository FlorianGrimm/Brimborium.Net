namespace Brimborium.CodeFlow.FlowSynchronization {
    public class SyncById<T> : SyncById {
        public SyncById(SyncByType syncByType) : base(syncByType)
        {
        }

        ~SyncById() {
            this.Dispose(disposing: false);
        }
    }
}
