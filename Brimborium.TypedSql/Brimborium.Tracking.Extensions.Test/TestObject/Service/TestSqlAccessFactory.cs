namespace Brimborium.Tracking.Extensions.TestObject;

public sealed class TestSqlAccessFactory : ITrackingAccessConnectionFactory {
    private readonly ITrackingTransConnection _TrackingTransConnection;

    public TestSqlAccessFactory(ITrackingTransConnection trackingTransConnection) {
        this.Options = new TrackingSqlConnectionOption();
        this._TrackingTransConnection = trackingTransConnection;
    }

    public TrackingSqlConnectionOption Options { get; set; }

    public TrackingSqlConnectionOption GetOptions() => this.Options;

    public void SetOptions(TrackingSqlConnectionOption value) => this.Options = value;

    public Task<ITrackingTransConnection> BeginTrackingTransConnection(CancellationToken cancellationToken = default) {
        return Task.FromResult(this._TrackingTransConnection);
    }
}

