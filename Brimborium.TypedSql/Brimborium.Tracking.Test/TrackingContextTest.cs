using System;
using System.Collections.Generic;

using Xunit;

namespace Brimborium.Tracking.Test;

public class TrackingContextTest {
    [Fact]
    public void TrackingContext_001_Attach() {
        var sut = new Test1TrackingContext();
        Assert.NotNull(sut);
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        sut.Attach(new Ebbes(id1, "1"));
        sut.Attach(new Ebbes(id2, "2"));
        Assert.Equal(2, sut.Ebbes.Count);
        Assert.Equal("1", sut.Ebbes[new EbbesKey(Id:id1)].Value.Title);
    }
}

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
    : System.Collections.Generic.IEqualityComparer<EbbesKey> {
    //System.Collections.Generic.IEqualityComparer<Ebbes>
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