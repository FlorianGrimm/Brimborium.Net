namespace Brimborium.CodeFlow.FlowSynchronization {
    public interface IState<T> {
        bool GetIsValueSet { get; }

        T GetValue();
        void SetValue(T value);
    }
}