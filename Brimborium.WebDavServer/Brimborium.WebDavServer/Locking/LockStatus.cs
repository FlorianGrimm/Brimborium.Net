using System.Collections.Generic;
using System.Linq;

namespace Brimborium.WebDavServer.Locking
{
    /// <summary>
    /// A list of locks affecting a single lock (request)
    /// </summary>
    public class LockStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LockStatus"/> class.
        /// </summary>
        /// <param name="referenceLocks">The locks found at the reference position</param>
        /// <param name="parentLocks">The locks found at positions higher in the hierarchy</param>
        /// <param name="childLocks">The locks found at positions lower in the hierarchy</param>
        public LockStatus(
            IReadOnlyCollection<IActiveLock> referenceLocks,
            IReadOnlyCollection<IActiveLock> parentLocks,
            IReadOnlyCollection<IActiveLock> childLocks)
        {
            this.ReferenceLocks = referenceLocks;
            this.ParentLocks = parentLocks;
            this.ChildLocks = childLocks;
        }

        /// <summary>
        /// Gets the empty lock status
        /// </summary>
        public static LockStatus Empty { get; } = new LockStatus(new IActiveLock[0], new IActiveLock[0], new IActiveLock[0]);

        /// <summary>
        /// Gets the locks found at the reference position
        /// </summary>
        public IReadOnlyCollection<IActiveLock> ReferenceLocks { get; }

        /// <summary>
        /// Gets the locks found at positions higher in the hierarchy
        /// </summary>
        public IReadOnlyCollection<IActiveLock> ParentLocks { get; }

        /// <summary>
        /// Gets the locks found at positions lower in the hierarchy
        /// </summary>
        public IReadOnlyCollection<IActiveLock> ChildLocks { get; }

        /// <summary>
        /// Gets a value indicating whether there where no locks found at all
        /// </summary>
        public bool IsEmpty => this.Count == 0;

        /// <summary>
        /// Gets the number of found locks
        /// </summary>
        public int Count => this.ReferenceLocks.Count + this.ParentLocks.Count + this.ChildLocks.Count;

        /// <summary>
        /// Gets all found locks
        /// </summary>
        /// <returns>all found locks</returns>
        public IEnumerable<IActiveLock> GetLocks()
        {
            return this.ParentLocks.Concat(this.ReferenceLocks).Concat(this.ChildLocks);
        }
    }
}
