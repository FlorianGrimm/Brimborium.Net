using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.Services {
    public interface ISystemClock {
        DateTime GetUtcNow();
    }
    public sealed class SystemClock : ISystemClock {
        public DateTime GetUtcNow() {
            return DateTime.UtcNow;
        }
    }
}
