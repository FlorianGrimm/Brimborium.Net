using System.Runtime.CompilerServices;

namespace Brimborium.Extensions.Logging.LocalFile;

internal record struct FileGroupingYMD(
    int Year,
    int Month,
    int Day
) {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileGroupingYMD FromDateTimeOffset(DateTimeOffset rimestamp)
        => new FileGroupingYMD(
            rimestamp.Year,
            rimestamp.Month,
            rimestamp.Day
        );
}

internal record struct FileGroupingYMDH(
    int Year,
    int Month,
    int Day,
    int Hour
) {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileGroupingYMDH FromDateTimeOffset(DateTimeOffset rimestamp)
        => new FileGroupingYMDH(
            rimestamp.Year,
            rimestamp.Month,
            rimestamp.Day,
            rimestamp.Hour
        );
}

internal record struct FileGroupingYMDHM(
    int Year,
    int Month,
    int Day,
    int Hour,
    int Minute
) {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileGroupingYMDHM FromDateTimeOffset(DateTimeOffset rimestamp)
        => new FileGroupingYMDHM(
            rimestamp.Year,
            rimestamp.Month,
            rimestamp.Day,
            rimestamp.Hour,
            rimestamp.Minute
        );
}
