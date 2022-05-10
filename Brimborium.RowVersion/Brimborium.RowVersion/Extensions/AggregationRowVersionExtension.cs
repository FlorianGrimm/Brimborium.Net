namespace Brimborium.RowVersion.Extensions;

public static class AggregationEntityVersionExtension {
    public static AggregationEntityVersion ToAggregationEntityVersion<T>(this IEnumerable<T> that) where T : IEntityWithVersion {
        var result = new AggregationEntityVersion();
        foreach (var item in that) {
            item.EntityVersion.MaxEntityVersion(ref result);
        }
        return result;
    }

    public static string CalculateArguments<T>(T args) {
        var b = JsonSerializer.SerializeToUtf8Bytes(args);
        return MD5Extension.GetMD5HashFromByteArray(b);
    }

    public static string ToCacheKey(this AggregationEntityVersion that) {
        return $"{that.EntityVersion.ToString("x16")}:{that.CountVersion.ToString("x8")}:{that.Arguments}";
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void MaxEntityVersion(this long entityVersionNext, ref long entityVersion) {
        if (entityVersion < entityVersionNext) {
            entityVersion = entityVersionNext;
        }
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static void MaxEntityVersion(this long entityVersionNext, ref AggregationEntityVersion aggregationEntityVersion) {
        if (aggregationEntityVersion.EntityVersion < entityVersionNext) {
            aggregationEntityVersion.EntityVersion = entityVersionNext;
        }
        aggregationEntityVersion.CountVersion++;
    }
}
