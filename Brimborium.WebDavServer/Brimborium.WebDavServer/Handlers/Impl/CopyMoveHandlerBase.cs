﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.Engines;
using Brimborium.WebDavServer.Engines.Local;
using Brimborium.WebDavServer.Engines.Remote;
using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Locking;
using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Model.Headers;
using Brimborium.WebDavServer.Utils;

using Microsoft.Extensions.Logging;

namespace Brimborium.WebDavServer.Handlers.Impl
{
    /// <summary>
    /// The shared implementation of the COPY and MOVE handlers.
    /// </summary>
    public abstract class CopyMoveHandlerBase
    {
        private readonly IFileSystem _rootFileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyMoveHandlerBase"/> class.
        /// </summary>
        /// <param name="rootFileSystem">The root file system</param>
        /// <param name="context">The WebDAV context</param>
        /// <param name="logger">The logger to use (either for COPY or MOVE)</param>
        protected CopyMoveHandlerBase(IFileSystem rootFileSystem, IWebDavContext context, ILogger logger)
        {
            this._rootFileSystem = rootFileSystem;
            this.WebDavContext = context;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the WebDAV context
        /// </summary>
        protected IWebDavContext WebDavContext { get; }

        /// <summary>
        /// Gets the logger
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Executes the COPY or MOVE recursively
        /// </summary>
        /// <param name="sourcePath">The source path</param>
        /// <param name="destination">The destination URI</param>
        /// <param name="depth">The depth</param>
        /// <param name="overwrite">Can the destination be overwritten?</param>
        /// <param name="mode">The COPY mode to use</param>
        /// <param name="isMove">Is this a move operation?</param>
        /// <param name="cancellationToken">The cancellcation token</param>
        /// <returns>The result of the operation</returns>
        protected async Task<IWebDavResult> ExecuteAsync(
            string sourcePath,
            Uri destination,
            DepthHeader depth,
            bool overwrite,
            RecursiveProcessingMode mode,
            bool isMove,
            CancellationToken cancellationToken)
        {
            var sourceSelectionResult = await this._rootFileSystem.SelectAsync(sourcePath, cancellationToken).ConfigureAwait(false);
            if (sourceSelectionResult.IsMissing)
            {
                if (this.WebDavContext.RequestHeaders.IfNoneMatch != null) {
                    throw new WebDavException(WebDavStatusCode.PreconditionFailed);
                }

                throw new WebDavException(WebDavStatusCode.NotFound);
            }

            await this.WebDavContext.RequestHeaders
                .ValidateAsync(sourceSelectionResult.TargetEntry, cancellationToken).ConfigureAwait(false);

            IWebDavResult result;
            IImplicitLock sourceTempLock;
            var lockManager = this._rootFileSystem.LockManager;

            if (isMove)
            {
                var sourceLockRequirements = new Lock(
                    sourceSelectionResult.TargetEntry.Path,
                    this.WebDavContext.PublicRelativeRequestUrl,
                    depth != DepthHeader.Zero,
                    new XElement(WebDavXml.Dav + "owner", this.WebDavContext.User.Identity?.Name ?? string.Empty),
                    LockAccessType.Write,
                    LockShareMode.Shared,
                    TimeoutHeader.Infinite);
                sourceTempLock = lockManager == null
                    ? new ImplicitLock(true)
                    : await lockManager.LockImplicitAsync(
                            this._rootFileSystem,
                            this.WebDavContext.RequestHeaders.If?.Lists,
                            sourceLockRequirements,
                            cancellationToken)
                        .ConfigureAwait(false);
                if (!sourceTempLock.IsSuccessful) {
                    return sourceTempLock.CreateErrorResponse();
                }
            }
            else
            {
                sourceTempLock = new ImplicitLock(true);
            }

            try
            {
                var sourceUrl = this.WebDavContext.PublicAbsoluteRequestUrl;
                var destinationUrl = new Uri(sourceUrl, destination);

                // Ignore different schemes
                if (!this.WebDavContext.PublicControllerUrl.IsBaseOf(destinationUrl) || mode == RecursiveProcessingMode.PreferCrossServer)
                {
                    if (this.Logger.IsEnabled(LogLevel.Trace)) {
                        this.Logger.LogTrace("Using cross-server mode");
                    }

                    if (this.Logger.IsEnabled(LogLevel.Debug)) {
                        this.Logger.LogDebug($"{this.WebDavContext.PublicControllerUrl} is not a base of {destinationUrl}");
                    }

                    using (var remoteHandler = await this.CreateRemoteTargetActionsAsync(
                            destinationUrl,
                            cancellationToken)
                        .ConfigureAwait(false))
                    {
                        if (remoteHandler == null)
                        {
                            throw new WebDavException(
                                WebDavStatusCode.BadGateway,
                                "No remote handler for given client");
                        }

                        // For error reporting
                        sourceUrl = this.WebDavContext.PublicRootUrl.MakeRelativeUri(sourceUrl);

                        var remoteTargetResult = await this.RemoteExecuteAsync(
                            remoteHandler,
                            sourceUrl,
                            sourceSelectionResult,
                            destinationUrl,
                            depth,
                            overwrite,
                            cancellationToken).ConfigureAwait(false);
                        result = remoteTargetResult.Evaluate(this.WebDavContext);
                    }
                }
                else
                {
                    // Copy or move from one known file system to another
                    var destinationPath = this.WebDavContext.PublicControllerUrl.MakeRelativeUri(destinationUrl).ToString();

                    // For error reporting
                    sourceUrl = this.WebDavContext.PublicRootUrl.MakeRelativeUri(sourceUrl);
                    destinationUrl = this.WebDavContext.PublicRootUrl.MakeRelativeUri(destinationUrl);

                    var destinationSelectionResult =
                        await this._rootFileSystem.SelectAsync(destinationPath, cancellationToken).ConfigureAwait(false);
                    if (destinationSelectionResult.IsMissing && destinationSelectionResult.MissingNames.Count != 1)
                    {
                        this.Logger.LogDebug(
                            $"{destinationUrl}: The target is missing with the following path parts: {string.Join(", ", destinationSelectionResult.MissingNames)}");
                        throw new WebDavException(WebDavStatusCode.Conflict);
                    }

                    var destLockRequirements = new Lock(
                        new Uri(destinationPath, UriKind.Relative),
                        destinationUrl,
                        isMove || depth != DepthHeader.Zero,
                        new XElement(WebDavXml.Dav + "owner", this.WebDavContext.User.Identity?.Name ?? string.Empty),
                        LockAccessType.Write,
                        LockShareMode.Shared,
                        TimeoutHeader.Infinite);
                    var destTempLock = lockManager == null
                        ? new ImplicitLock(true)
                        : await lockManager.LockImplicitAsync(
                                this._rootFileSystem,
                                this.WebDavContext.RequestHeaders.If?.Lists,
                                destLockRequirements,
                                cancellationToken)
                            .ConfigureAwait(false);
                    if (!destTempLock.IsSuccessful) {
                        return destTempLock.CreateErrorResponse();
                    }

                    try
                    {
                        var isSameFileSystem = ReferenceEquals(
                            sourceSelectionResult.TargetFileSystem,
                            destinationSelectionResult.TargetFileSystem);
                        var localMode = isSameFileSystem && mode == RecursiveProcessingMode.PreferFastest
                            ? RecursiveProcessingMode.PreferFastest
                            : RecursiveProcessingMode.PreferCrossFileSystem;
                        var handler = this.CreateLocalTargetActions(localMode);

                        var targetInfo = FileSystemTarget.FromSelectionResult(
                            destinationSelectionResult,
                            destinationUrl,
                            handler);
                        var targetResult = await this.LocalExecuteAsync(
                            handler,
                            sourceUrl,
                            sourceSelectionResult,
                            targetInfo,
                            depth,
                            overwrite,
                            cancellationToken).ConfigureAwait(false);
                        result = targetResult.Evaluate(this.WebDavContext);
                    }
                    finally
                    {
                        await destTempLock.DisposeAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                await sourceTempLock.DisposeAsync(cancellationToken).ConfigureAwait(false);
            }

            if (isMove && lockManager != null)
            {
                var locksToRemove = await lockManager
                    .GetAffectedLocksAsync(sourcePath, true, false, cancellationToken)
                    .ConfigureAwait(false);
                foreach (var activeLock in locksToRemove)
                {
                    await lockManager.ReleaseAsync(
                            activeLock.Path,
                            new Uri(activeLock.StateToken),
                            cancellationToken)
                        .ConfigureAwait(false);
                }
            }

            return result;
        }

        /// <summary>
        /// Create the target action implementation for remote COPY or MOVE
        /// </summary>
        /// <param name="destinationUrl">The destination URL</param>
        /// <param name="cancellationToken">The cancellcation token</param>
        /// <returns>The implementation for remote actions</returns>
        protected abstract Task<IRemoteTargetActions?> CreateRemoteTargetActionsAsync(Uri destinationUrl, CancellationToken cancellationToken);

        /// <summary>
        /// Create the target action implementation for local COPY or MOVE
        /// </summary>
        /// <param name="mode">The requested processing mode (in-filesystem or cross-filesystem)</param>
        /// <returns>The implementation for local actions</returns>
        protected abstract ITargetActions<CollectionTarget, DocumentTarget, MissingTarget> CreateLocalTargetActions(RecursiveProcessingMode mode);

        /// <summary>
        /// Executes the COPY or MOVE recursively
        /// </summary>
        /// <typeparam name="TCollection">The collection type</typeparam>
        /// <typeparam name="TDocument">The document type</typeparam>
        /// <typeparam name="TMissing">The type for a missing entry</typeparam>
        /// <param name="engine">The engine to use to perform the operation</param>
        /// <param name="sourceUrl">The source URL</param>
        /// <param name="sourceSelectionResult">The source element</param>
        /// <param name="parentCollection">The parent collection of the source element</param>
        /// <param name="targetItem">The target of the operation</param>
        /// <param name="depth">The depth</param>
        /// <param name="cancellationToken">The cancellcation token</param>
        /// <returns>The result of the operation</returns>
        private async Task<Engines.CollectionActionResult> ExecuteAsync<TCollection, TDocument, TMissing>(
            RecursiveExecutionEngine<TCollection, TDocument, TMissing> engine,
            Uri sourceUrl,
            SelectionResult sourceSelectionResult,
            TCollection parentCollection,
            ITarget targetItem,
            DepthHeader depth,
            CancellationToken cancellationToken)
            where TCollection : class, ICollectionTarget<TCollection, TDocument, TMissing>
            where TDocument : class, IDocumentTarget<TCollection, TDocument, TMissing>
            where TMissing : class, IMissingTarget<TCollection, TDocument, TMissing>
        {
            Debug.Assert(sourceSelectionResult.Collection != null, "sourceSelectionResult.Collection != null");

            if (this.Logger.IsEnabled(LogLevel.Trace)) {
                this.Logger.LogTrace($"Copy or move from {sourceUrl} to {targetItem.DestinationUrl}");
            }

            if (sourceSelectionResult.ResultType == SelectionResultType.FoundDocument)
            {
                ActionResult docResult;
                if (targetItem is TCollection)
                {
                    // Cannot overwrite collection with document
                    docResult = new ActionResult(ActionStatus.OverwriteFailed, targetItem);
                }
                else if (targetItem is TMissing)
                {
                    var target = (TMissing)targetItem;
                    docResult = await engine.ExecuteAsync(
                        sourceUrl,
                        sourceSelectionResult.Document!,
                        target,
                        cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    var target = (TDocument)targetItem;
                    docResult = await engine.ExecuteAsync(
                        sourceUrl,
                        sourceSelectionResult.Document!,
                        target,
                        cancellationToken).ConfigureAwait(false);
                }

                var engineResult = new Engines.CollectionActionResult(ActionStatus.Ignored, parentCollection)
                {
                    DocumentActionResults = new[] { docResult },
                };

                return engineResult;
            }

            Engines.CollectionActionResult collResult;
            if (targetItem is TDocument)
            {
                // Cannot overwrite document with collection
                collResult = new Engines.CollectionActionResult(ActionStatus.OverwriteFailed, targetItem);
            }
            else if (targetItem is TMissing)
            {
                var target = (TMissing)targetItem;
                collResult = await engine.ExecuteAsync(
                    sourceUrl,
                    sourceSelectionResult.Collection,
                    depth,
                    target,
                    cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var target = (TCollection)targetItem;
                collResult = await engine.ExecuteAsync(
                    sourceUrl,
                    sourceSelectionResult.Collection,
                    depth,
                    target,
                    cancellationToken).ConfigureAwait(false);
            }

            return collResult;
        }

        private async Task<Engines.CollectionActionResult> RemoteExecuteAsync(
            IRemoteTargetActions handler,
            Uri sourceUrl,
            SelectionResult sourceSelectionResult,
            Uri targetUrl,
            DepthHeader depth,
            bool overwrite,
            CancellationToken cancellationToken)
        {
            Debug.Assert(sourceSelectionResult.Collection != null, "sourceSelectionResult.Collection != null");

            var parentCollectionUrl = targetUrl.GetParent();

            var engine = new RecursiveExecutionEngine<RemoteCollectionTarget, RemoteDocumentTarget, RemoteMissingTarget>(
                handler,
                overwrite,
                this.Logger);

            var targetName = targetUrl.GetName();
            var parentName = parentCollectionUrl.GetName();
            var parentCollection = new RemoteCollectionTarget(null, parentName, parentCollectionUrl, false, handler);
            var targetItem = await handler.GetAsync(parentCollection, targetName, cancellationToken).ConfigureAwait(false);

            return await this.ExecuteAsync(
                    engine,
                    sourceUrl,
                    sourceSelectionResult,
                    parentCollection,
                    targetItem,
                    depth,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task<Engines.CollectionActionResult> LocalExecuteAsync(
            ITargetActions<CollectionTarget, DocumentTarget, MissingTarget> handler,
            Uri sourceUrl,
            SelectionResult sourceSelectionResult,
            FileSystemTarget targetInfo,
            DepthHeader depth,
            bool overwrite,
            CancellationToken cancellationToken)
        {
            Debug.Assert(sourceSelectionResult.Collection != null, "sourceSelectionResult.Collection != null");

            var engine = new RecursiveExecutionEngine<CollectionTarget, DocumentTarget, MissingTarget>(
                handler,
                overwrite,
                this.Logger);

            CollectionTarget? parentCollection;
            ITarget targetItem;
            if (targetInfo.Collection != null)
            {
                var collTarget = targetInfo.NewCollectionTarget();
                parentCollection = collTarget.Parent;
                targetItem = collTarget;
            }
            else if (targetInfo.Document != null)
            {
                var docTarget = targetInfo.NewDocumentTarget();
                parentCollection = docTarget.Parent;
                targetItem = docTarget;
            }
            else
            {
                var missingTarget = targetInfo.NewMissingTarget();
                parentCollection = missingTarget.Parent;
                targetItem = missingTarget;
            }

            Debug.Assert(parentCollection != null, "Cannt copy or move the root collection.");
            if (parentCollection == null) {
                throw new InvalidOperationException("Cannt copy or move the root collection.");
            }

            return await this.ExecuteAsync(
                    engine,
                    sourceUrl,
                    sourceSelectionResult,
                    parentCollection,
                    targetItem,
                    depth,
                    cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
