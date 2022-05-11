namespace Brimborium.Tracking;

public interface ITrackingSetApplyChanges<TValue> {

    Task<TValue> Insert(TValue value, ITrackingTransConnection trackingTransaction);

    Task<TValue> Update(TValue value, ITrackingTransConnection trackingTransaction);

    Task Delete(TValue value, ITrackingTransConnection trackingTransaction);
}