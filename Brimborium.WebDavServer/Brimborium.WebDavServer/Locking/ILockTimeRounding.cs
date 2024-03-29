﻿using System;

namespace Brimborium.WebDavServer.Locking
{
    /// <summary>
    /// Interface for classes that want to implement custom rounding
    /// </summary>
    public interface ILockTimeRounding
    {
        /// <summary>
        /// The rounding implementation
        /// </summary>
        /// <param name="dt">The date and time to round</param>
        /// <returns>The new <see cref="DateTime"/></returns>
        DateTime Round(DateTime dt);

        /// <summary>
        /// The rounding implementation
        /// </summary>
        /// <param name="ts">The time span to round</param>
        /// <returns>The new timestamp</returns>
        TimeSpan Round(TimeSpan ts);
    }
}
