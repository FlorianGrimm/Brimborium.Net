using TrackingSet.Contracts.Entity;

namespace TrackingSet.Repository.Service;

public sealed class TrackingSetData : TrackingSet<DataPK, DataEntity> {
    public TrackingSetData(DBContext context, ITrackingSetApplyChanges<DataEntity> trackingApplyChanges)
        : base(
            extractKey: DataUtiltiy.Instance,
            comparer: DataUtiltiy.Instance,
            trackingContext: context,
            trackingApplyChanges: trackingApplyChanges) {

    }
}

public sealed class TrackingSetApplyChangesData : TrackingSetApplyChangesBase<DataEntity, DataPK> {
    private static TrackingSetApplyChangesData? _Instance;
    public static TrackingSetApplyChangesData Instance => _Instance ??= new TrackingSetApplyChangesData();

    private TrackingSetApplyChangesData() : base() { }

    protected override DataPK ExtractKey(DataEntity value) => value.GetPrimaryKey();

    public override Task<DataEntity> Insert(DataEntity value, ITrackingTransConnection trackingTransaction) {
        return this.Upsert(value, trackingTransaction);
    }

    public override Task<DataEntity> Update(DataEntity value, ITrackingTransConnection trackingTransaction) {
        return this.Upsert(value, trackingTransaction);
    }

    private async Task<DataEntity> Upsert(DataEntity value, ITrackingTransConnection trackingTransaction) {
        var sqlAccess = (ISqlAccess)trackingTransaction;
        var result = await sqlAccess.ExecuteDataUpsertAsync(value);
        return this.ValidateUpsertDataManipulationResult(value, result);
    }

    public override async Task Delete(DataEntity value, ITrackingTransConnection trackingTransaction) {
        var sqlAccess = (ISqlAccess)trackingTransaction;
        var result = await sqlAccess.ExecuteDataDeletePKAsync(value);
        this.ValidateDelete(value, result);
    }
}


public sealed class DataUtiltiy
    : IEqualityComparer<DataPK>
    , IExtractKey<DataEntity, DataPK> {
    private static DataUtiltiy? _Instance;
    public static DataUtiltiy Instance => (_Instance ??= new DataUtiltiy());
    private DataUtiltiy() { }

    public DataPK ExtractKey(DataEntity that) => that.GetPrimaryKey();

    bool IEqualityComparer<DataPK>.Equals(DataPK? x, DataPK? y) {
        if (object.ReferenceEquals(x, y)) {
            return true;
        } else if ((x is null) || (y is null)) {
            return false;
        } else {
            return x.Equals(y);
        }
    }

    int IEqualityComparer<DataPK>.GetHashCode(DataPK obj) {
        return obj.GetHashCode();
    }
}