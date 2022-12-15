namespace Brimborium.LocalObservability;

public class IncidentProcessingEngine 
    : IIncidentProcessingEngine2 {
    private readonly Channel<ActualPolymorphCodePoint> _Channel;
    private readonly List<IStateTransition> _ListStateTransition;
    //private ChannelReader<ActualPolymorphCodePoint>? _ChannelReader;
    private readonly CodePointState _CodePointState;

    public IncidentProcessingEngine(
        MatchingEngineOptions options
        ) {
        this._Channel = Channel.CreateBounded<ActualPolymorphCodePoint>(
            new BoundedChannelOptions(16 * 1024) { 
            FullMode=BoundedChannelFullMode.DropWrite
            });
        this._ListStateTransition = new List<IStateTransition>(options.ListStateTransition);       //this._ChannelReader = this._Channel.Reader;
        this._CodePointState = new CodePointState();
    }

    public ChannelWriter<ActualPolymorphCodePoint> GetChannelWriter()
        => this._Channel.Writer;

    public async Task Execute(CancellationToken stoppingToken) {
        var reader = this._Channel.Reader;
        try {
            while (!stoppingToken.IsCancellationRequested) {
                ActualPolymorphCodePoint actualPolymorphCodePoint;
                try {
                    actualPolymorphCodePoint = await reader.ReadAsync(stoppingToken);
                } catch (Exception ex) {
                    return;
                }
                try {
                    // apply the IStateTransition
                    foreach (var stateTransition in this._ListStateTransition) {
                        stateTransition.Apply(
                            actualPolymorphCodePoint,
                            this._CodePointState,
                            this);
                    }
                    System.Diagnostics.Debug.Print($"acp:{actualPolymorphCodePoint.MatchedActualCodePoint.Count} {actualPolymorphCodePoint.MatchedActualCodePoint.FirstOrDefault()?.CodePoint?.Name}");
                } catch (Exception ex) {
                }
            }
        } finally {
        }
    }

    public void SetReport(IActualCodePoint acp, string message) {
#warning TODO
    }

    public void SetReport(ActualPolymorphCodePoint actualPolymorphCodePoint, string message) {
#warning TODO
    }

    public void SetTerminating(IActualCodePoint acp, ICodePointState codePointState) {
#warning TODO
    }
}

