using System;

namespace Brimborium.WebDavServer.Model.Headers {
    /// <summary>
    /// The <c>Lock-Token</c> header
    /// </summary>
    public class LockTokenHeader {
        /// <summary>
        /// Initializes a new instance of the <see cref="LockTokenHeader"/> class.
        /// </summary>
        /// <param name="stateToken">The lock token</param>
        public LockTokenHeader(Uri stateToken) {
            this.StateToken = stateToken;
        }

        /// <summary>
        /// Gets the lock token
        /// </summary>
        public Uri StateToken { get; }

        /// <summary>
        /// Parses the header string to get a new instance of the <see cref="LockTokenHeader"/> class
        /// </summary>
        /// <param name="s">The header string to parse</param>
        /// <returns>The new instance of the <see cref="LockTokenHeader"/> class</returns>
        public static LockTokenHeader Parse(string s) {
            if (!CodedUrlParser.TryParse(s, out var stateToken)) {
                throw new ArgumentException($"{s} is not a valid lock token", nameof(s));
            }

            return new LockTokenHeader(stateToken);
        }

        /// <inheritdoc />
        public override string ToString() {
            return $"<{this.StateToken}>";
        }
    }
}
