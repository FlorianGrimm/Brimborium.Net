using System;
using System.Xml.Linq;

using Brimborium.WebDavServer.Model.Headers;

namespace Brimborium.WebDavServer.Locking
{
    /// <summary>
    /// A generic implementation of an active lock
    /// </summary>
    /// <remarks>
    /// The <see cref="ILockManager"/> implementation might use a different implementation
    /// of an <see cref="IActiveLock"/>.
    /// </remarks>
    internal class ActiveLock : Lock, IActiveLock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveLock"/> class.
        /// </summary>
        /// <param name="l">The lock to create this active lock from</param>
        /// <param name="issued">The date/time when this lock was issued</param>
        internal ActiveLock(ILock l, DateTime issued)
            : this(
                l.Path,
                l.Href,
                l.Recursive,
                l.GetOwner(),
                LockAccessType.Parse(l.AccessType),
                LockShareMode.Parse(l.ShareMode),
                l.Timeout,
                issued,
                null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveLock"/> class.
        /// </summary>
        /// <param name="l">The lock to create this active lock from</param>
        /// <param name="issued">The date/time when this lock was issued</param>
        /// <param name="timeout">Override the timeout from the original lock (to enforce rounding)</param>
        internal ActiveLock(ILock l, DateTime issued, TimeSpan timeout)
            : this(
                l.Path,
                l.Href,
                l.Recursive,
                l.GetOwner(),
                LockAccessType.Parse(l.AccessType),
                LockShareMode.Parse(l.ShareMode),
                timeout,
                issued,
                null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveLock"/> class.
        /// </summary>
        /// <param name="path">The file system path (root-relative) this lock should be applied to</param>
        /// <param name="href">The href this lock should be applied to (might be relative or absolute)</param>
        /// <param name="recursive">Must the lock be applied recursively to all children?</param>
        /// <param name="owner">The owner of the lock</param>
        /// <param name="accessType">The <see cref="LockAccessType"/> of the lock</param>
        /// <param name="shareMode">The <see cref="LockShareMode"/> of the lock</param>
        /// <param name="timeout">The lock timeout</param>
        /// <param name="issued">The date/time when this lock was issued</param>
        /// <param name="lastRefresh">The date/time of the last refresh</param>
        internal ActiveLock(
            string path,
            string href,
            bool recursive,
            XElement? owner,
            LockAccessType accessType,
            LockShareMode shareMode,
            TimeSpan timeout,
            DateTime issued,
            DateTime? lastRefresh)
            : base(
                path,
                href,
                recursive,
                owner,
                accessType.Name.LocalName,
                shareMode.Name.LocalName,
                timeout)
        {
            this.Issued = issued;
            this.LastRefresh = lastRefresh;
            this.Expiration = timeout == TimeoutHeader.Infinite ? DateTime.MaxValue : (lastRefresh ?? issued) + timeout;
            this.StateToken = $"urn:uuid:{Guid.NewGuid():D}";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveLock"/> class.
        /// </summary>
        /// <param name="path">The file system path (root-relative) this lock should be applied to</param>
        /// <param name="href">The href this lock should be applied to (might be relative or absolute)</param>
        /// <param name="recursive">Must the lock be applied recursively to all children?</param>
        /// <param name="owner">The owner of the lock</param>
        /// <param name="accessType">The <see cref="LockAccessType"/> of the lock</param>
        /// <param name="shareMode">The <see cref="LockShareMode"/> of the lock</param>
        /// <param name="timeout">The lock timeout</param>
        /// <param name="issued">The date/time when this lock was issued</param>
        /// <param name="lastRefresh">The date/time of the last refresh</param>
        /// <param name="stateToken">The stateTokenh</param>
        internal ActiveLock(
            string path,
            string href,
            bool recursive,
            XElement? owner,
            LockAccessType accessType,
            LockShareMode shareMode,
            TimeSpan timeout,
            DateTime issued,
            DateTime? lastRefresh,
            string stateToken)
            : base(
                path,
                href,
                recursive,
                owner,
                accessType.Name.LocalName,
                shareMode.Name.LocalName,
                timeout)
        {
            this.Issued = issued;
            this.LastRefresh = lastRefresh;
            this.Expiration = timeout == TimeoutHeader.Infinite ? DateTime.MaxValue : (lastRefresh ?? issued) + timeout;
            this.StateToken = stateToken;
        }

        /// <inheritdoc />
        public string StateToken { get; }

        /// <inheritdoc />
        public DateTime Issued { get; }

        /// <inheritdoc />
        public DateTime? LastRefresh { get; }

        /// <inheritdoc />
        public DateTime Expiration { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Path={this.Path} [Href={this.Href}, Recursive={this.Recursive}, AccessType={this.AccessType}, ShareMode={this.ShareMode}, Timeout={this.Timeout}, Owner={this.Owner}, StateToken={this.StateToken}, Issued={this.Issued:O}, LastRefresh={this.LastRefresh:O}, Expiration={this.Expiration:O}]";
        }
    }
}
