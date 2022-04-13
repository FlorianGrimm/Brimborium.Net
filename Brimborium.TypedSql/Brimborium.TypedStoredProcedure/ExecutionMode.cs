namespace Brimborium.TypedStoredProcedure {
    public enum ExecutionMode {
        Unknown,
        ExecuteNonQuery,
        ExecuteScalar,
        QuerySingleOrDefault,
        QuerySingle,
        Query,
        QueryMultiple,
        Obsolete,
        Unsure,
        Ignore
    }
}
