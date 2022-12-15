
using Microsoft.Extensions.Hosting;

using System.Threading;

namespace Brimborium.LocalObservability;
    public class IncidentProcessingEngineHostedService : BackgroundService {
    private readonly IIncidentProcessingEngine2 _IncidentProcessingEngine;

    public IncidentProcessingEngineHostedService(
        IIncidentProcessingEngine2 incidentProcessingEngine
        ) {
        this._IncidentProcessingEngine = incidentProcessingEngine;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
        return this._IncidentProcessingEngine.Execute(stoppingToken);
    }
}
