#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.


using Brimborium.Tracking.API;
using Brimborium.Tracking.Entity;

namespace Brimborium.Tracking.Service;

public class Test1TrackingContext : TrackingContext {
    public Test1TrackingContext(
        Func<Test1TrackingContext, ITrackingSetEbbes>? fnEbbes = default
    ) {
        this.Ebbes = (fnEbbes is not null)
            ? fnEbbes(this)
            : new TrackingSetEbbes0(trackingContext: this);
    }
    public ITrackingSetEbbes Ebbes { get; }
}


public interface ITrackingSetEbbes : ITrackingSet<EbbesPK, EbbesEntity> {
}

public class Test1TrackingTransConnection : ITrackingTransConnection {
    public Task CommitAsync() {
        return Task.CompletedTask;
    }

    public void Dispose() {
    }
}