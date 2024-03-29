﻿namespace Brimborium.WebDavServer.Model.Headers
{
    /// <summary>
    /// The normalized range item
    /// </summary>
    public struct NormalizedRangeItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NormalizedRangeItem"/> struct.
        /// </summary>
        /// <param name="from">The start position from where the data will be read from/written to</param>
        /// <param name="to">The end position until which the data will be read from/written to</param>
        public NormalizedRangeItem(long from, long to)
        {
            this.From = from;
            this.To = to;
        }

        /// <summary>
        /// Gets the start position
        /// </summary>
        public long From { get; }

        /// <summary>
        /// Gets the end position
        /// </summary>
        public long To { get; }

        /// <summary>
        /// Gets the length of this range item
        /// </summary>
        public long Length => this.To - this.From + 1;
    }
}
