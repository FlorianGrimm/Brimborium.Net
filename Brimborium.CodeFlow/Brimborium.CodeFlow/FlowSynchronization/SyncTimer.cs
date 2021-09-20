using Microsoft.Extensions.Logging;

using System;
using System.Threading;

namespace Brimborium.CodeFlow.FlowSynchronization {
    public class SyncTimer : IDisposable {
        private bool _DisposedValue;
        private Timer? _timer;
        private readonly ILogger _Logger;

        public SyncTimer(
            ILogger<SyncTimer> logger
            // IHostApplicationLifetime appLifetime
            ) {
            this._Logger = logger;
        }

        public void Register() { 
        }

        public void RegisterCallback() {
            if (this._timer is null) {
                lock (this) {
                    if (this._timer is null) {
                        this._timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
                    }
                }
            }
        }

        //public Task StopAsync(CancellationToken cancellationToken) {
        //    _timer?.Change(Timeout.Infinite, 0);

        //    return Task.CompletedTask;
        //}


        private void DoWork(object? state) {

        }

        protected virtual void Dispose(bool disposing) {
            if (!_DisposedValue) {
                using (var t = this._timer) {
                    if (disposing) {
                        // TODO: dispose managed state (managed objects)
                    }
                    _DisposedValue = true;
                    this._timer = null;
                }
            }
        }

        ~SyncTimer() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
    public interface ISyncTimerHostedService {
        void Register(SyncTimer that);
    }
}