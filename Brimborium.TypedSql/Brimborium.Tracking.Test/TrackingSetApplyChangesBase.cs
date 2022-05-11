namespace Brimborium.Tracking.Test;

public abstract class TrackingSetApplyChangesBase<TValue, TPrimaryKey>
    : ITrackingSetApplyChanges<TValue>
    where TValue : class, Brimborium.RowVersion.Entity.IEntityWithVersion
    where TPrimaryKey : IEquatable<TPrimaryKey> {
    private string _TypeName;

    protected TrackingSetApplyChangesBase() {
        this._TypeName = typeof(TValue).Name ?? string.Empty;
    }

    protected abstract TPrimaryKey ExtractKey(TValue value);

    public abstract Task<TValue> Insert(TValue value, ITrackingTransConnection trackingTransaction);

    public abstract Task<TValue> Update(TValue value, ITrackingTransConnection trackingTransaction);

    public abstract Task Delete(TValue value, ITrackingTransConnection trackingTransaction);

    protected TValue ValidateUpsertDataManipulationResult(
        TValue value,
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
            throw new InvalidOperationException($"RowVersionMismatch {value.EntityVersion}!={result.DataResult.EntityVersion}");
        }
        throw new InvalidOperationException($"Unknown error {result.OperationResult.ResultValue} {this._TypeName} {this.ExtractKey(value)}");
    }

    protected void ValidateDelete(
        TValue value,
        List<TPrimaryKey> result
        ) {
        if (result.Count == 1) {
            var pkValue = this.ExtractKey(value);
            if (pkValue.Equals(result[0])) {
                return;
            } else {
                throw new InvalidOperationException($"Unknown error {this._TypeName}: {result[0]} != {pkValue}");
            }
        } else {
            throw new InvalidOperationException($"Cannot delete {this._TypeName}: {result.FirstOrDefault()}");
        }
    }
}
