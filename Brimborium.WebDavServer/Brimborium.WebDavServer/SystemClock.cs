using System;

namespace Brimborium.WebDavServer
{
    /// <summary>
    /// The default implementation of <see cref="ISystemClock"/>
    /// </summary>
    public class SystemClock : ISystemClock
    {
        /// <inheritdoc />
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
