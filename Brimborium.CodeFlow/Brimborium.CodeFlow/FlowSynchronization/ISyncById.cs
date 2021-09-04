using System;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public interface ISyncById: IDisposable {
    }
    public interface ISyncById<T> {
        IState<T> GetState();
        bool IsStateSet();
        void SetState(IState<T> state);
    }
}