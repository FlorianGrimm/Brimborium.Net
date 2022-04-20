#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
using System;
using System.Collections.Generic;

namespace Brimborium.Tracking.Test;

public class Test1TrackingContext : TrackingContext {
    public Test1TrackingContext() {
        this.Ebbes = new TrackingSet<EbbesKey, Ebbes>(
            extractKey: EbbesUtiltiy.Instance.ExtractKey,
            comparer: EbbesUtiltiy.Instance,
            trackingContext: this,
            trackingApplyChanges: null!
            );
    }
    public TrackingSet<EbbesKey, Ebbes> Ebbes { get; }
}

public record EbbesKey(
    Guid Id
);

public record Ebbes(
    Guid Id,
    string Title,
    ulong SerialVersion = 0
    );

public class EbbesUtiltiy
    : IEqualityComparer<EbbesKey> {
    private static EbbesUtiltiy? _Instance;
    public static EbbesUtiltiy Instance => (_Instance ??= new EbbesUtiltiy());
    private EbbesUtiltiy() { }

    public EbbesKey ExtractKey(Ebbes that)
        => new EbbesKey(that.Id);

    bool IEqualityComparer<EbbesKey>.Equals(EbbesKey? x, EbbesKey? y) {
        if (object.ReferenceEquals(x, y)) {
            return true;
        } else if ((x is null) || (y is null)) {
            return false;
        } else {
            return x.Equals(y);
        }
    }

    int IEqualityComparer<EbbesKey>.GetHashCode(EbbesKey obj) {
        return obj.GetHashCode();
    }

    //bool IEqualityComparer<Ebbes>.Equals(Ebbes? x, Ebbes? y) {
    //    if (object.ReferenceEquals(x, y)) {
    //        return true;
    //    } else if ((x is null) || (y is null)) {
    //        return false;
    //    } else {
    //        return x.Equals(y);
    //    }
    //}

    //int IEqualityComparer<Ebbes>.GetHashCode(Ebbes obj) {
    //    return obj.GetHashCode();
    //}
}