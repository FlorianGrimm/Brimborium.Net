using System;
using System.Text.RegularExpressions;

namespace Brimborium.WebDavServer.Model.Headers
{
    /// <summary>
    /// Range for a HTTP request or response
    /// </summary>
    public struct RangeHeaderItem
    {
        private static readonly Regex _rangePattern = new Regex(@"^((\d+)-(\d+))|((\d+)-)|(-(\d+))|(\d+)$", RegexOptions.CultureInvariant);

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeHeaderItem"/> struct.
        /// </summary>
        /// <param name="from">From byte</param>
        /// <param name="to">To byte</param>
        public RangeHeaderItem(long? from, long? to)
        {
            this.From = from;
            this.To = to;
        }

        /// <summary>
        /// Gets the start position
        /// </summary>
        public long? From { get; }

        /// <summary>
        /// Gets the end position
        /// </summary>
        public long? To { get; }

        /// <summary>
        /// Parses a <see cref="RangeHeaderItem"/> from a string
        /// </summary>
        /// <remarks>
        /// Allowed are: "*", "from-", "-to", "from-to"
        /// </remarks>
        /// <param name="rangeItem">The string to parse</param>
        /// <returns>The new <see cref="RangeHeaderItem"/></returns>
        public static RangeHeaderItem Parse(string rangeItem)
        {
            if (rangeItem == "*") {
                return default(RangeHeaderItem);
            }

            var match = _rangePattern.Match(rangeItem);
            if (!match.Success) {
                throw new ArgumentOutOfRangeException(nameof(rangeItem));
            }

            var s = match.Groups[8].Value;
            if (!string.IsNullOrEmpty(s))
            {
                var v = Convert.ToInt64(s, 10);
                return new RangeHeaderItem(v, v);
            }

            s = match.Groups[7].Value;
            if (!string.IsNullOrEmpty(s))
            {
                var v = Convert.ToInt64(s, 10);
                return new RangeHeaderItem(null, v);
            }

            s = match.Groups[5].Value;
            if (!string.IsNullOrEmpty(s))
            {
                var v = Convert.ToInt64(s, 10);
                return new RangeHeaderItem(v, null);
            }

            var from = Convert.ToInt64(match.Groups[2].Value, 10);
            var to = Convert.ToInt64(match.Groups[3].Value, 10);
            return new RangeHeaderItem(from, to);
        }

        /// <summary>
        /// Returns the textual representation of the HTTP range item
        /// </summary>
        /// <returns>the textual representation of the HTTP range item</returns>
        public override string ToString()
        {
            if (this.From.HasValue || this.To.HasValue) {
                return $"{this.From}-{this.To}";
            }

            return "*";
        }

        /// <summary>
        /// Normalize this range header item
        /// </summary>
        /// <param name="totalLength">The total length to normalize this item with</param>
        /// <returns>The normalized range item</returns>
        public NormalizedRangeItem Normalize(long totalLength)
        {
            if (!this.From.HasValue && !this.To.HasValue) {
                return new NormalizedRangeItem(0, totalLength - 1);
            }

            if (this.From.HasValue && this.To.HasValue) {
                return new NormalizedRangeItem(this.From.Value, this.To.Value);
            }

            if (this.From.HasValue) {
                return new NormalizedRangeItem(this.From.Value, totalLength - 1);
            }

            return new NormalizedRangeItem(totalLength - this.To.GetValueOrDefault(0), totalLength - 1);
        }
    }
}
