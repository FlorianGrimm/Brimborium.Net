using System;

namespace Brimborium.WebDavServer
{
    /// <summary>
    /// Interface for querying the system clock
    /// </summary>
    public interface ISystemClock
    {
        /// <summary>
        /// Gets the <see cref="DateTime.UtcNow"/>
        /// </summary>
        DateTime UtcNow { get; }
    }
}
