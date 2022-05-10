namespace Brimborium.RowVersion.Entity;

public struct AggregationEntityVersion {
    public long EntityVersion { get; set; }
    public int CountVersion { get; set; }
    public string Arguments { get; set; }
}

public sealed class AggregationRowVersionHolder {
    public AggregationRowVersionHolder() {
        this.Value = new AggregationEntityVersion();
    }
    public AggregationEntityVersion Value;
}
