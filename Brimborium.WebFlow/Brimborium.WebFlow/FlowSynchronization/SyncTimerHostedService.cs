using Brimborium.CodeFlow.FlowSynchronization;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.WebFlow.FlowSynchronization {
#warning really needed?
    public class SyncTimerHostedService : ISyncTimerHostedService, IHostedService, IDisposable {
        private readonly ILogger<SyncTimerHostedService> _Logger;

        public SyncTimerHostedService(ILogger<SyncTimerHostedService> logger) {
            this._Logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            //_timer?.Change(Timeout.Infinite, 0);
            //IHostApplicationLifetime
            return Task.CompletedTask;
        }


        public void Dispose() {
        }

        public void Register(SyncTimer that) {
        }
    }
}
