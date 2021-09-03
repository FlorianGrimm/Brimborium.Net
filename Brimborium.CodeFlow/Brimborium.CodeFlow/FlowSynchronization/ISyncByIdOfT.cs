namespace Brimborium.CodeFlow.FlowSynchronization {
    public interface ISyncById<T> {
        T GetItem();
        bool IsItemSet();
        void SetItem(T item);
    }
}