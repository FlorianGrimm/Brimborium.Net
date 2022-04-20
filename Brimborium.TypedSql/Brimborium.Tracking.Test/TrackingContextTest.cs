#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.

using System;
using System.Collections.Generic;

using Xunit;

namespace Brimborium.Tracking.Test;

public class TrackingContextTest {
    private Guid id1;
    private Guid id2;
    private Guid id3;
    private Guid id4;
    private Guid id5;
    private Guid id6;

    private const int EbbesCnt = 5;

    private const string Title1 = "Title 1";
    private const string Title1V2 = "Title 1V2";
    private const string Title2 = "Title 2";
    private const string Title3 = "Title 3";
    private const string Title4 = "Title 4";
    private const string Title5 = "Title 5";
    private const string Title6 = "Title 6";

    public TrackingContextTest() {
        id1 = Guid.NewGuid();
        id2 = Guid.NewGuid();
        id3 = Guid.NewGuid();
        id4 = Guid.NewGuid();
        id5 = Guid.NewGuid();
        id6 = Guid.NewGuid();
    }

    private Test1TrackingContext CreateTrackingContext() {
        var sut = new Test1TrackingContext();
        Assert.NotNull(sut);
        sut.Attach(new Ebbes(id1, Title1));
        sut.Attach(new Ebbes(id2, Title2));
        sut.Attach(new Ebbes(id3, Title3));
        sut.Attach(new Ebbes(id4, Title4));
        sut.Attach(new Ebbes(id5, Title5));
        Assert.Equal(5, sut.Ebbes.Count);
        Assert.Equal(0, sut.TrackingChanges.Changes.Count);
        return sut;
    }

    [Fact]
    public void TrackingContext_001_Attach() {
        var sut = CreateTrackingContext();
        Assert.Equal(Title1, sut.Ebbes[new EbbesKey(Id: id1)].Title);
        Assert.Empty(sut.TrackingChanges.Changes);
    }

    [Fact]
    public void TrackingContext_002_Detach() {
        var sut = CreateTrackingContext();
        Assert.Equal(EbbesCnt, sut.Ebbes.Count);
        sut.Ebbes.Detach(sut.Ebbes.GetTrackingObject(new EbbesKey(Id: id1)));
        Assert.Equal(EbbesCnt - 1, sut.Ebbes.Count);
        Assert.Equal(Title2, sut.Ebbes[new EbbesKey(Id: id2)].Title);
        Assert.Empty(sut.TrackingChanges.Changes);
    }

    [Fact]
    public void TrackingContext_003_Add() {
        var sut = CreateTrackingContext();
        sut.Ebbes.Add(new Ebbes(Id: id6, Title: Title6));
        Assert.Equal(EbbesCnt + 1, sut.Ebbes.Count);
        Assert.Equal(Title6, sut.Ebbes[new EbbesKey(Id: id6)].Title);
        Assert.Equal(1, sut.TrackingChanges.Changes.Count);

        var chg = sut.TrackingChanges.Changes[0];
        Assert.Equal(TrackingStatus.Added, chg.Status);
        Assert.Equal(TrackingStatus.Added, chg.TrackingObject.Status);
        Assert.Equal(Title6, ((Ebbes)chg.TrackingObject.GetValue()).Title);
    }

    [Fact]
    public void TrackingContext_004_Update() {
        var sut = CreateTrackingContext();
        sut.Ebbes.Update(new Ebbes(Id: id1, Title: Title1V2));
        Assert.Equal(EbbesCnt, sut.Ebbes.Count);
        Assert.Equal(Title1V2, sut.Ebbes[new EbbesKey(Id: id1)].Title);
        Assert.Equal(1, sut.TrackingChanges.Changes.Count);

        var chg = sut.TrackingChanges.Changes[0];
        Assert.Equal(TrackingStatus.Modified, chg.Status);
        Assert.Equal(TrackingStatus.Modified, chg.TrackingObject.Status);
        Assert.Equal(Title1V2, ((Ebbes)chg.TrackingObject.GetValue()).Title);
    }

    [Fact]
    public void TrackingContext_004_Update_UnknownKey() {
        var sut = CreateTrackingContext();
        Assert.Throws<System.InvalidOperationException>(() => {
            sut.Ebbes.Update(new Ebbes(Id: id6, Title: Title6));
        });
    }

    [Fact]
    public void TrackingContext_005_Upsert() {
        var sut = CreateTrackingContext();
        sut.Ebbes.Upsert(new Ebbes(Id: id1, Title: Title1V2));
        sut.Ebbes.Upsert(new Ebbes(Id: id6, Title: Title6));
        Assert.Equal(EbbesCnt + 1, sut.Ebbes.Count);
        Assert.Equal(Title1V2, sut.Ebbes[new EbbesKey(Id: id1)].Title);
        Assert.Equal(Title6, sut.Ebbes[new EbbesKey(Id: id6)].Title);
        Assert.Equal(2, sut.TrackingChanges.Changes.Count);
        {
            var chg = sut.TrackingChanges.Changes[0];
            Assert.Equal(TrackingStatus.Modified, chg.Status);
            Assert.Equal(TrackingStatus.Modified, chg.TrackingObject.Status);
            Assert.Equal(Title1V2, ((Ebbes)chg.TrackingObject.GetValue()).Title);
        }
        {
            var chg = sut.TrackingChanges.Changes[1];
            Assert.Equal(TrackingStatus.Added, chg.Status);
            Assert.Equal(TrackingStatus.Added, chg.TrackingObject.Status);
            Assert.Equal(Title6, ((Ebbes)chg.TrackingObject.GetValue()).Title);
        }
    }
    [Fact]
    public void TrackingContext_006_Delete() {
        var sut = CreateTrackingContext();
        sut.Ebbes.Delete(sut.Ebbes[new EbbesKey(Id: id1)]);
        Assert.Equal(EbbesCnt - 1, sut.Ebbes.Count);
        Assert.Equal(Title2, sut.Ebbes[new EbbesKey(Id: id2)].Title);
        Assert.Equal(1, sut.TrackingChanges.Changes.Count);

        var chg = sut.TrackingChanges.Changes[0];
        Assert.Equal(TrackingStatus.Deleted, chg.Status);
        Assert.Equal(TrackingStatus.Deleted, chg.TrackingObject.Status);
        Assert.Equal(Title1, ((Ebbes)chg.TrackingObject.GetValue()).Title);
    }
}
