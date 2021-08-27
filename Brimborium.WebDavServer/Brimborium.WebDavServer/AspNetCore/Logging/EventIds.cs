using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;

namespace Brimborium.WebDavServer.AspNetCore.Logging
{
    /// <summary>
    /// The default logging event IDs
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "EventId is a struct.")]
    public static class EventIds
    {
        /// <summary>
        /// The unspecified event ID
        /// </summary>
        public static EventId Unspecified = new EventId(0);
    }
}
