namespace Brimborium.Tracking.Extensions.TestObject;

public class DBContext : TrackingContext {
    public DBContext() {
        this.Ebbes = new TrackingSetEbbes(
            trackingContext:this
            );
    }

    public TrackingSetEbbes Ebbes { get; }
}