namespace Brimborium.Tracking.Test;

public interface IDataManipulationResult<T> {
    T DataResult { get; }
    OperationResult OperationResult { get; }
}

public struct OperationResult {
    public OperationResult(ResultValue resultValue) {
        this.ResultValue = resultValue;
        this.Message = string.Empty;
    }
    public ResultValue ResultValue { get; set; }
    public string Message { get; set; }
    // public int MessageCode { get; set; }
}

/// <summary>
/// Result of Database operation
/// </summary>
public enum ResultValue {
    /// <summary>
    /// Faulted .e.g Validation
    /// </summary>
    Faulted = -100,
    /// <summary>
    /// update and record doesn't exists.
    /// </summary>
    NotFound = -2,
    /// <summary>
    /// record found but RowVersionMismatch
    /// </summary>
    RowVersionMismatch = -1,
    /// <summary>
    /// record found - no changes
    /// </summary>
    NoNeedToUpdate = 0,
    /// <summary>
    /// record inserted
    /// </summary>
    Inserted = 1,
    /// <summary>
    /// record updated
    /// </summary>
    Updated = 2,
    /// <summary>
    /// record deleted
    /// </summary>
    Deleted = 3
}