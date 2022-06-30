namespace Brimborium.Tracking.Service;

public abstract class TrackingSetApplyChangesBase<TKey, TValue>
    : ITrackingSetApplyChanges<TKey, TValue>
    where TKey : notnull, IEquatable<TKey>
    where TValue : class, RowVersion.Entity.IEntityWithVersion {
    private string _TypeName;
    private readonly IExtractKey<TKey, TValue> _ExtractKey;

    protected TrackingSetApplyChangesBase(
        IExtractKey<TKey, TValue> extractKey
    ) {
        this._TypeName = typeof(TValue).Name ?? string.Empty;
        this._ExtractKey = extractKey;
    }

    protected virtual bool TryExtractKey(TValue value, out TKey key) {
        return this._ExtractKey.TryExtractKey(value, out key);
    }

    public abstract Task<TValue> Insert(TrackingObject<TKey, TValue> to, ITrackingTransConnection trackingTransaction);

    public abstract Task<TValue> Update(TrackingObject<TKey, TValue> to, ITrackingTransConnection trackingTransaction);

    public abstract Task Delete(TrackingObject<TKey, TValue> to, ITrackingTransConnection trackingTransaction);

    protected virtual TValue ValidateUpsertDataManipulationResult(
        TrackingObject<TKey, TValue> to,
        IDataManipulationResult<TValue> result
        ) {
        if (result.OperationResult.ResultValue == ResultValue.Updated) {
            return result.DataResult;
        }
        if (result.OperationResult.ResultValue == ResultValue.Inserted) {
            return result.DataResult;
        }
        if (result.OperationResult.ResultValue == ResultValue.NoNeedToUpdate) {
            // Project: Log??
            return result.DataResult;
        }
        if (result.OperationResult.ResultValue == ResultValue.RowVersionMismatch) {
            throw new InvalidModificationException($"RowVersionMismatch {to.Value.EntityVersion}!={result.DataResult.EntityVersion}");
        }
        {
#warning here
            this.TryExtractKey(to.Value, out var key);
            throw new InvalidModificationException($"Unknown error {result.OperationResult.ResultValue} {this._TypeName} {key}");
        }
    }

    protected void ValidateDelete(
        TValue value,
        List<TKey> result
        ) {
#warning here
        if (result.Count == 1) {
            if (this.TryExtractKey(value, out var pkValue)) {
                if (pkValue.Equals(result[0])) {
                    return;
                } else {
                    throw new InvalidModificationException($"Unknown error {this._TypeName}: {result[0]} != {pkValue}");
                }
            } else {
                throw new InvalidModificationException($"Unknown error {this._TypeName}: {result[0]} != {pkValue}");
            }

        } else {
            throw new InvalidModificationException($"Cannot delete {this._TypeName}: {result.FirstOrDefault()}");
        }
    }
}
