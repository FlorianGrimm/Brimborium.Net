namespace Brimborium.RowVersion.Extensions;

public static class CompareExtension {
    /*
    public static Func<T, T, int> ChainCompareDTValidRangeQ<T>(Func<T, T, int> before)
        where T : IDTValidRangeQ {
        return compare;
        int compare(T a, T b) {
            var result = before(a, b);
            if (result == 0) {
                return CompareDTValidRangeQ(a, b);
            } else {
                return result;
            }
        }
    }
    */

    public static int CompareDTValidRange<T>(T a, T b)
        where T : IDTValidRange {
        var rangeValidFrom = a.ValidFrom.Subtract(b.ValidFrom);
        if (rangeValidFrom.Ticks < 0) {
            return -1;
        } else if (rangeValidFrom.Ticks > 0) {
            return +1;
        } else {
            return DateTime.Compare(a.ValidTo, b.ValidTo);
        }
    }

    public static int CompareDTValidRangeQ<T>(T a, T b)
        where T : IDTValidRangeQ {
        var rangeValidFrom = a.ValidFrom.Subtract(b.ValidFrom);
        if (rangeValidFrom.Ticks < 0) {
            return -1;
        } else if (rangeValidFrom.Ticks > 0) {
            return +1;
        } else {
            if (a.ValidTo.HasValue) {
                if (b.ValidTo.HasValue) {
                    // a && b
                    return DateTime.Compare(a.ValidTo.Value, b.ValidTo.Value);
                } else {
                    // a && !b
                    return -1;
                }
            } else {
                if (b.ValidTo.HasValue) {
                    // !a && b
                    return +1;
                } else {
                    // !a && !b
                    return 0;
                }
            }
        }
    }
}
