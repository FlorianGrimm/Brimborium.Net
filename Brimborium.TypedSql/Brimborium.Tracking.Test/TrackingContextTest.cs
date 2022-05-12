#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.

using System;
using System.Collections.Generic;

using Xunit;

namespace Brimborium.Tracking;

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

    private Test1TrackingContext CreateTrackingContext(
        ITrackingSet<EbbesPK, EbbesEntity>? ebbes = default
    ) {
        var sut = new Test1TrackingContext(ebbes);
        Assert.NotNull(sut);
        sut.Ebbes.Attach(new EbbesEntity(id1, Title1));
        sut.Ebbes.Attach(new EbbesEntity(id2, Title2));
        sut.Ebbes.Attach(new EbbesEntity(id3, Title3));
        sut.Ebbes.Attach(new EbbesEntity(id4, Title4));
        sut.Ebbes.Attach(new EbbesEntity(id5, Title5));
        Assert.Equal(5, sut.Ebbes.Count);
        Assert.Equal(0, sut.TrackingChanges.Changes.Count);
        return sut;
    }

    [Fact]
    public void TrackingContext_001_Attach_WrongType() {
        Assert.Throws<InvalidOperationException>(
            () => {
                var sut = new Test1TrackingContext();
                sut.Attach(new Ebbes(id1, Title1));
            });
    }

    [Fact]
    public void TrackingContext_001_Attach() {
        var sut = CreateTrackingContext();
        Assert.Equal(Title1, sut.Ebbes[new EbbesPK(Id: id1)].Title);
        Assert.Empty(sut.TrackingChanges.Changes);
    }

    [Fact]
    public void TrackingContext_002_Detach() {
        var sut = CreateTrackingContext();
        Assert.Equal(EbbesCnt, sut.Ebbes.Count);
        sut.Ebbes.Detach(sut.Ebbes.GetTrackingObject(new EbbesPK(Id: id1)));
        Assert.Equal(EbbesCnt - 1, sut.Ebbes.Count);
        Assert.Equal(Title2, sut.Ebbes[new EbbesPK(Id: id2)].Title);
        Assert.Empty(sut.TrackingChanges.Changes);
    }

    [Fact]
    public void TrackingContext_003_Add() {
        var sut = CreateTrackingContext();
        Assert.Equal(5, sut.Ebbes.Count);

        sut.Ebbes.Add(new EbbesEntity(Id: id6, Title: Title6));
        Assert.Equal(EbbesCnt + 1, sut.Ebbes.Count);
        Assert.Equal(Title6, sut.Ebbes[new EbbesPK(Id: id6)].Title);
        Assert.Equal(1, sut.TrackingChanges.Changes.Count);

        var chg = sut.TrackingChanges.Changes[0];
        Assert.Equal(TrackingStatus.Added, chg.Status);
        Assert.Equal(Title6, ((EbbesEntity)chg.GetValue()).Title);
        Assert.Equal(6, sut.Ebbes.Count);
    }

    [Fact]
    public async Task TrackingContext_013_AddUndo() {
        var sut = CreateTrackingContext();
        Assert.Equal(5, sut.Ebbes.Count);

        sut.Ebbes.Add(new EbbesEntity(Id: id6, Title: Title6));
        Assert.Equal(EbbesCnt + 1, sut.Ebbes.Count);
        Assert.Equal(Title6, sut.Ebbes[new EbbesPK(Id: id6)].Title);
        Assert.Equal(1, sut.TrackingChanges.Changes.Count);

        var chg = sut.TrackingChanges.Changes[0];
        Assert.Equal(TrackingStatus.Added, chg.Status);
        Assert.Equal(Title6, ((EbbesEntity)chg.GetValue()).Title);
        Assert.Equal(6, sut.Ebbes.Count);

        sut.TrackingChanges.Undo(chg);
        Assert.Equal(5, sut.Ebbes.Count);

        foreach (var idx in System.Linq.Enumerable.Range(1, 50)) {
            sut.Ebbes.Add(new EbbesEntity(Id: Guid.NewGuid(), Title: idx.ToString()));
        }
        var to = sut.Ebbes.Add(new EbbesEntity(Id: id6, Title: Title6));
        foreach (var idx in System.Linq.Enumerable.Range(100, 50)) {
            sut.Ebbes.Add(new EbbesEntity(Id: Guid.NewGuid(), Title: idx.ToString()));
        }
        Assert.Equal(5 + 50 + 1 + 50, sut.Ebbes.Count);
        sut.TrackingChanges.Undo(to);
        Assert.Equal(5 + 50 + 0 + 50, sut.Ebbes.Count);
        var mockITrackingTransConnection = new Mock<ITrackingTransConnection>();
        await sut.ApplyChangesAsync(
            mockITrackingTransConnection.Object
        );
    }

    [Fact]
    public void TrackingContext_004_Update() {
        var sut = CreateTrackingContext();
        sut.Ebbes.Update(new EbbesEntity(Id: id1, Title: Title1V2));
        Assert.Equal(EbbesCnt, sut.Ebbes.Count);
        Assert.Equal(Title1V2, sut.Ebbes[new EbbesPK(Id: id1)].Title);
        Assert.Equal(1, sut.TrackingChanges.Changes.Count);

        var chg = sut.TrackingChanges.Changes[0];
        Assert.Equal(TrackingStatus.Modified, chg.Status);
        Assert.Equal(TrackingStatus.Modified, chg.Status);
        Assert.Equal(Title1V2, ((EbbesEntity)chg.GetValue()).Title);
    }

    [Fact]
    public void TrackingContext_004_Update_UnknownKey() {
        var sut = CreateTrackingContext();
        Assert.Throws<InvalidOperationException>(() => {
            sut.Ebbes.Update(new EbbesEntity(Id: id6, Title: Title6));
        });
    }

    [Fact]
    public void TrackingContext_005_Upsert() {
        var sut = CreateTrackingContext();
        sut.Ebbes.Upsert(new EbbesEntity(Id: id1, Title: Title1V2));
        sut.Ebbes.Upsert(new EbbesEntity(Id: id6, Title: Title6));
        Assert.Equal(EbbesCnt + 1, sut.Ebbes.Count);
        Assert.Equal(Title1V2, sut.Ebbes[new EbbesPK(Id: id1)].Title);
        Assert.Equal(Title6, sut.Ebbes[new EbbesPK(Id: id6)].Title);
        Assert.Equal(2, sut.TrackingChanges.Changes.Count);
        {
            var chg = sut.TrackingChanges.Changes[0];
            Assert.Equal(TrackingStatus.Modified, chg.Status);
            Assert.Equal(TrackingStatus.Modified, chg.Status);
            Assert.Equal(Title1V2, ((EbbesEntity)chg.GetValue()).Title);
        }
        {
            var chg = sut.TrackingChanges.Changes[1];
            Assert.Equal(TrackingStatus.Added, chg.Status);
            Assert.Equal(TrackingStatus.Added, chg.Status);
            Assert.Equal(Title6, ((EbbesEntity)chg.GetValue()).Title);
        }
    }
    [Fact]
    public void TrackingContext_006_Delete() {
        var sut = CreateTrackingContext();
        sut.Ebbes.Delete(sut.Ebbes[new EbbesPK(Id: id1)]);
        Assert.Equal(EbbesCnt - 1, sut.Ebbes.Count);
        Assert.Equal(Title2, sut.Ebbes[new EbbesPK(Id: id2)].Title);
        Assert.Equal(1, sut.TrackingChanges.Changes.Count);

        var chg = sut.TrackingChanges.Changes[0];
        Assert.Equal(TrackingStatus.Deleted, chg.Status);
        Assert.Equal(TrackingStatus.Deleted, chg.Status);
        Assert.Equal(Title1, ((EbbesEntity)chg.GetValue()).Title);
    }
}
