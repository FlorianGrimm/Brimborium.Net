﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Locking;
using Brimborium.WebDavServer.Model;

namespace Brimborium.WebDavServer.Props.Live
{
    /// <summary>
    /// The <c>lockdiscovery</c> property
    /// </summary>
    public class LockDiscoveryProperty : ILiveProperty
    {
        /// <summary>
        /// The XML property name
        /// </summary>
        public static readonly XName PropertyName = WebDavXml.Dav + "lockdiscovery";

        private readonly ILockManager? _lockManager;

        private readonly string _entryPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="LockDiscoveryProperty"/> class.
        /// </summary>
        /// <param name="entry">The file system entry</param>
        public LockDiscoveryProperty(IEntry entry)
        {
            this._lockManager = entry.FileSystem.LockManager;
            this._entryPath = entry.Path.OriginalString;
        }

        /// <inheritdoc />
        public XName Name { get; } = PropertyName;

        /// <inheritdoc />
        public string? Language { get; } = null;

        /// <inheritdoc />
        public IReadOnlyCollection<XName> AlternativeNames { get; } = new XName[0];

        /// <inheritdoc />
        public int Cost => this._lockManager?.Cost ?? 0;

        /// <inheritdoc />
        public Task<bool> IsValidAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public Task<XElement> GetXmlValueAsync(CancellationToken ct)
        {
            return this.GetXmlValueAsync(false, true, ct);
        }

        /// <summary>
        /// Get the XML value asynchronously
        /// </summary>
        /// <param name="omitOwner">Do we want to omit the owner?</param>
        /// <param name="omitToken">Do we want to omit the lock token?</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns>The XML value</returns>
        public async Task<XElement> GetXmlValueAsync(bool omitOwner, bool omitToken, CancellationToken ct)
        {
            if (this._lockManager == null) {
                return new XElement(this.Name);
            }

            var affectedLocks = await this._lockManager.GetAffectedLocksAsync(this._entryPath, false, true, ct).ConfigureAwait(false);
            var lockElements = affectedLocks.Select(x => x.ToXElement(omitOwner: omitOwner, omitToken: omitToken)).Cast<object>().ToArray();
            return new XElement(this.Name, lockElements);
        }
    }
}
