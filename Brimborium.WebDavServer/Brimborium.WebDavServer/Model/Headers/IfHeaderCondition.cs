using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Brimborium.WebDavServer.Utils;

namespace Brimborium.WebDavServer.Model.Headers
{
    /// <summary>
    /// Represents a single condition for an HTTP <c>If</c> header
    /// </summary>
    public class IfHeaderCondition
    {
        private readonly EntityTagComparer _etagComparer;

        private IfHeaderCondition(bool not, Uri? stateToken, EntityTag? etag, EntityTagComparer etagComparer)
        {
            this._etagComparer = etagComparer;
            this.Not = not;
            this.StateToken = stateToken;
            this.ETag = etag;
        }

        /// <summary>
        /// Gets a value indicating whether the result should be negated
        /// </summary>
        public bool Not { get; }

        /// <summary>
        /// Gets the state token to validate with
        /// </summary>
        public Uri? StateToken { get; }

        /// <summary>
        /// Gets the entity tag to validate with
        /// </summary>
        public EntityTag? ETag { get; }

        /// <summary>
        /// Validates if this condition matches the passed entity tag and/or state tokens
        /// </summary>
        /// <param name="etag">The entity tag</param>
        /// <param name="stateTokens">The state tokens</param>
        /// <returns><see langword="true"/> when this condition matches</returns>
        public bool IsMatch(EntityTag? etag, IReadOnlyCollection<Uri> stateTokens)
        {
            bool result;

            if (this.ETag.HasValue)
            {
                if (etag == null) {
                    return false;
                }

                result = this._etagComparer.Equals(etag.Value, this.ETag.Value);
            }
            else
            {
                Debug.Assert(this.StateToken != null, "StateToken != null");
                result = stateTokens.Any(x => x.Equals(this.StateToken));
            }

            return this.Not ? !result : result;
        }

        internal static IEnumerable<IfHeaderCondition> Parse(StringSource source, EntityTagComparer entityTagComparer)
        {
            while (!source.SkipWhiteSpace())
            {
                var isNot = false;
                EntityTag? etag = null;
                if (source.AdvanceIf("Not", StringComparison.OrdinalIgnoreCase))
                {
                    isNot = true;
                    source.SkipWhiteSpace();
                }

                if (CodedUrlParser.TryParse(source, out var stateToken))
                {
                    // Coded-URL found
                }
                else if (source.Get() == '[')
                {
                    // Entity-tag found
                    etag = EntityTag.Parse(source).Single();
                    if (!source.AdvanceIf("]")) {
                        throw new ArgumentException($"{source.Remaining} is not a valid condition (ETag not ending with ']')", nameof(source));
                    }
                }
                else
                {
                    source.Back();
                    break;
                }

                yield return new IfHeaderCondition(isNot, stateToken, etag, entityTagComparer);
            }
        }
    }
}
