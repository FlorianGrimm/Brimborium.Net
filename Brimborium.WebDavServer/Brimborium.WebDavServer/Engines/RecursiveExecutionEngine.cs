﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Model.Headers;
using Brimborium.WebDavServer.Props;

using Microsoft.Extensions.Logging;

namespace Brimborium.WebDavServer.Engines
{
    /// <summary>
    /// The engine that operates recursively on its targets
    /// </summary>
    /// <typeparam name="TCollection">The interface type for a collection target</typeparam>
    /// <typeparam name="TDocument">The interface type for a document target</typeparam>
    /// <typeparam name="TMissing">The interface type for a missing target</typeparam>
    public class RecursiveExecutionEngine<TCollection, TDocument, TMissing>
        where TCollection : class, ICollectionTarget<TCollection, TDocument, TMissing>
        where TDocument : class, IDocumentTarget<TCollection, TDocument, TMissing>
        where TMissing : class, IMissingTarget<TCollection, TDocument, TMissing>
    {
        private readonly ITargetActions<TCollection, TDocument, TMissing> _handler;

        private readonly bool _allowOverwrite;

        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecursiveExecutionEngine{TCollection,TDocument,TMissing}"/> class.
        /// </summary>
        /// <param name="handler">The handler that performs the operation on the targets</param>
        /// <param name="allowOverwrite">Is overwriting the destination allowed?</param>
        /// <param name="logger">The logger</param>
        public RecursiveExecutionEngine(ITargetActions<TCollection, TDocument, TMissing> handler, bool allowOverwrite, ILogger logger)
        {
            this._handler = handler;
            this._allowOverwrite = allowOverwrite;
            this._logger = logger;
        }

        /// <summary>
        /// Operates on a collection and the given missing target
        /// </summary>
        /// <param name="sourceUrl">The root-relative source URL</param>
        /// <param name="source">The source collection</param>
        /// <param name="depth">The recursion depth</param>
        /// <param name="target">The target of the operation</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The result information of the current operation</returns>
        public async Task<CollectionActionResult> ExecuteAsync(Uri sourceUrl, ICollection source, DepthHeader depth, TMissing target, CancellationToken cancellationToken)
        {
            if (this._logger.IsEnabled(LogLevel.Trace)) {
                this._logger.LogTrace($"Collecting nodes for operation on collection {sourceUrl} with missing target {target.DestinationUrl}.");
            }

            var nodes = await source.GetNodeAsync(depth.OrderValue, cancellationToken).ConfigureAwait(false);
            return await this.ExecuteAsync(sourceUrl, nodes, target, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Operates on a collection and the given target collection
        /// </summary>
        /// <param name="sourceUrl">The root-relative source URL</param>
        /// <param name="source">The source collection</param>
        /// <param name="depth">The recursion depth</param>
        /// <param name="target">The target collection</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The result information of the current operation</returns>
        public async Task<CollectionActionResult> ExecuteAsync(Uri sourceUrl, ICollection source, DepthHeader depth, TCollection target, CancellationToken cancellationToken)
        {
            if (this._logger.IsEnabled(LogLevel.Trace)) {
                this._logger.LogTrace($"Collecting nodes for operation on collection {sourceUrl} with existing target {target.DestinationUrl}.");
            }

            var nodes = await source.GetNodeAsync(depth.OrderValue, cancellationToken).ConfigureAwait(false);
            return await this.ExecuteAsync(sourceUrl, nodes, target, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Operates on a documentation and the given missing target
        /// </summary>
        /// <param name="sourceUrl">The root-relative source URL</param>
        /// <param name="source">The source documentation</param>
        /// <param name="target">The target of the operation</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The result information of the current operation</returns>
        public async Task<ActionResult> ExecuteAsync(Uri sourceUrl, IDocument source, TMissing target, CancellationToken cancellationToken)
        {
            if (this._logger.IsEnabled(LogLevel.Trace)) {
                this._logger.LogTrace($"Perform operation on document {sourceUrl} with missing target {target.DestinationUrl}.");
            }

            try
            {
                var properties = await this.GetWriteableProperties(source, cancellationToken).ConfigureAwait(false);

                var newDoc = await this._handler.ExecuteAsync(source, target, cancellationToken).ConfigureAwait(false);

                var failedPropertyNames = await newDoc.SetPropertiesAsync(properties, cancellationToken).ConfigureAwait(false);
                if (failedPropertyNames.Count != 0)
                {
                    this._logger.LogDebug($"{target.DestinationUrl}: Failed setting properties {string.Join(", ", failedPropertyNames.Select(x => x.ToString()))}");
                    return new ActionResult(ActionStatus.PropSetFailed, target)
                    {
                        FailedProperties = failedPropertyNames,
                    };
                }

                return new ActionResult(ActionStatus.Created, newDoc);
            }
            catch (Exception ex)
            {
                this._logger.LogDebug($"{target.DestinationUrl}: Failed with exception {ex.Message}");
                return new ActionResult(ActionStatus.CreateFailed, target)
                {
                    Exception = ex,
                };
            }
        }

        /// <summary>
        /// Operates on a documentation and the given document target
        /// </summary>
        /// <param name="sourceUrl">The root-relative source URL</param>
        /// <param name="source">The source documentation</param>
        /// <param name="target">The target document of the operation</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The result information of the current operation</returns>
        public async Task<ActionResult> ExecuteAsync(Uri sourceUrl, IDocument source, TDocument target, CancellationToken cancellationToken)
        {
            if (this._logger.IsEnabled(LogLevel.Trace)) {
                this._logger.LogTrace($"Try to perform operation on document {sourceUrl} with existing target {target.DestinationUrl}.");
            }

            if (!this._allowOverwrite)
            {
                this._logger.LogDebug($"{target.DestinationUrl}: Cannot overwrite because destination exists");
                return new ActionResult(ActionStatus.CannotOverwrite, target);
            }

            if (this._handler.ExistingTargetBehaviour == RecursiveTargetBehaviour.DeleteTarget)
            {
                if (this._logger.IsEnabled(LogLevel.Trace)) {
                    this._logger.LogTrace($"Delete {target.DestinationUrl} before performing operation on document {sourceUrl}.");
                }

                TMissing missingTarget;
                try
                {
                    missingTarget = await target.DeleteAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this._logger.LogDebug($"{target.DestinationUrl}: Delete failed with exception {ex.Message}");
                    return new CollectionActionResult(ActionStatus.TargetDeleteFailed, target)
                    {
                        Exception = ex,
                    };
                }

                return await this.ExecuteAsync(sourceUrl, source, missingTarget, cancellationToken).ConfigureAwait(false);
            }

            if (this._logger.IsEnabled(LogLevel.Trace)) {
                this._logger.LogTrace($"Perform operation on document {sourceUrl} with existing target {target.DestinationUrl}.");
            }

            var properties = await this.GetWriteableProperties(source, cancellationToken).ConfigureAwait(false);

            var docActionResult = await this._handler.ExecuteAsync(source, target, cancellationToken).ConfigureAwait(false);
            if (docActionResult.IsFailure) {
                return docActionResult;
            }

            var failedPropertyNames = await target.SetPropertiesAsync(properties, cancellationToken).ConfigureAwait(false);
            if (failedPropertyNames.Count != 0)
            {
                this._logger.LogDebug($"{target.DestinationUrl}: Failed setting properties {string.Join(", ", failedPropertyNames.Select(x => x.ToString()))}");
                return new ActionResult(ActionStatus.PropSetFailed, target)
                {
                    FailedProperties = failedPropertyNames,
                };
            }

            return new ActionResult(ActionStatus.Overwritten, target);
        }

        private async Task<List<IUntypedWriteableProperty>> GetWriteableProperties(IEntry entry, CancellationToken cancellationToken)
        {
            var properties = new List<IUntypedWriteableProperty>();
            await using (var propsEnum = entry.GetProperties(this._handler.Dispatcher).GetAsyncEnumerator())
            {
                while (await propsEnum.MoveNextAsync(cancellationToken).ConfigureAwait(false))
                {
                    var prop = propsEnum.Current as IUntypedWriteableProperty;
                    if (prop != null) {
                        properties.Add(prop);
                    }
                }
            }

            return properties;
        }

        private async Task<CollectionActionResult> ExecuteAsync(
            Uri sourceUrl,
            ICollectionNode sourceNode,
            TMissing target,
            CancellationToken cancellationToken)
        {
            if (this._logger.IsEnabled(LogLevel.Trace)) {
                this._logger.LogTrace($"Collect properties for operation on collection {sourceUrl} and create target {target.DestinationUrl}.");
            }

            var properties = await this.GetWriteableProperties(sourceNode.Collection, cancellationToken).ConfigureAwait(false);

            TCollection newColl;
            try
            {
                newColl = await target.CreateCollectionAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogDebug($"{target.DestinationUrl}: Create failed with exception {ex.Message}");
                return new CollectionActionResult(ActionStatus.CreateFailed, target)
                {
                    Exception = ex,
                };
            }

            return await this.ExecuteAsync(sourceUrl, sourceNode, newColl, properties, cancellationToken).ConfigureAwait(false);
        }

        private async Task<CollectionActionResult> ExecuteAsync(
            Uri sourceUrl,
            ICollectionNode sourceNode,
            TCollection target,
            CancellationToken cancellationToken)
        {
            if (this._allowOverwrite && this._handler.ExistingTargetBehaviour == RecursiveTargetBehaviour.DeleteTarget)
            {
                if (this._logger.IsEnabled(LogLevel.Trace)) {
                    this._logger.LogTrace($"Delete existing target {target.DestinationUrl} for operation on collection {sourceUrl}.");
                }

                // Only delete an existing collection when the client allows an overwrite
                TMissing missing;
                try
                {
                    missing = await target.DeleteAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this._logger.LogDebug($"{target.DestinationUrl}: Delete failed with exception {ex.Message}");
                    return new CollectionActionResult(ActionStatus.TargetDeleteFailed, target)
                    {
                        Exception = ex,
                    };
                }

                return await this.ExecuteAsync(sourceUrl, sourceNode, missing, cancellationToken).ConfigureAwait(false);
            }

            if (this._logger.IsEnabled(LogLevel.Trace)) {
                this._logger.LogTrace($"Collect properties for operation on collection {sourceUrl} with existing target {target.DestinationUrl}.");
            }

            var properties = await this.GetWriteableProperties(sourceNode.Collection, cancellationToken).ConfigureAwait(false);
            return await this.ExecuteAsync(sourceUrl, sourceNode, target, properties, cancellationToken).ConfigureAwait(false);
        }

        private async Task<CollectionActionResult> ExecuteAsync(
            Uri sourceUrl,
            ICollectionNode sourceNode,
            TCollection target,
            IReadOnlyCollection<IUntypedWriteableProperty> properties,
            CancellationToken cancellationToken)
        {
            if (this._logger.IsEnabled(LogLevel.Trace)) {
                this._logger.LogTrace($"Perform operation on collection {sourceUrl} with existing target {target.DestinationUrl}.");
            }

            var documentActionResults = ImmutableList<ActionResult>.Empty;
            var collectionActionResults = ImmutableList<CollectionActionResult>.Empty;

            var subNodeProperties = new Dictionary<string, IReadOnlyCollection<IUntypedWriteableProperty>>();
            foreach (var childNode in sourceNode.Nodes)
            {
                var subProperties = await this.GetWriteableProperties(childNode.Collection, cancellationToken).ConfigureAwait(false);
                subNodeProperties.Add(childNode.Name, subProperties);
            }

            foreach (var document in sourceNode.Documents)
            {
                var docUrl = sourceUrl.Append(document);
                if (target.Created)
                {
                    // Collection was created by us - we just assume that the document doesn't exist
                    var missingTarget = target.NewMissing(document.Name);
                    var docResult = await this.ExecuteAsync(docUrl, document, missingTarget, cancellationToken).ConfigureAwait(false);
                    documentActionResults = documentActionResults.Add(docResult);
                }
                else
                {
                    var foundTarget = await target.GetAsync(document.Name, cancellationToken).ConfigureAwait(false);
                    var docTarget = foundTarget as TDocument;
                    if (docTarget != null)
                    {
                        // We found a document: Business as usual when we're allowed to overwrite it
                        var docResult = await this.ExecuteAsync(docUrl, document, docTarget, cancellationToken).ConfigureAwait(false);
                        documentActionResults = documentActionResults.Add(docResult);
                    }
                    else
                    {
                        var collTarget = foundTarget as TCollection;
                        if (collTarget != null)
                        {
                            // We found a collection instead of a document
                            this._logger.LogDebug($"{target.DestinationUrl}: Found a collection instead of a document");
                            var docResult = new ActionResult(ActionStatus.OverwriteFailed, foundTarget);
                            documentActionResults = documentActionResults.Add(docResult);
                        }
                        else
                        {
                            // We didn't find anything: Business as usual
                            var missingTarget = (TMissing)foundTarget;
                            var docResult = await this.ExecuteAsync(docUrl, document, missingTarget, cancellationToken).ConfigureAwait(false);
                            documentActionResults = documentActionResults.Add(docResult);
                        }
                    }
                }
            }

            foreach (var childNode in sourceNode.Nodes)
            {
                var childProperties = subNodeProperties[childNode.Name];
                var collection = childNode.Collection;
                var docUrl = sourceUrl.Append(childNode.Collection);
                if (target.Created)
                {
                    // Collection was created by us - we just assume that the sub collection doesn't exist
                    var missingTarget = target.NewMissing(childNode.Name);
                    var newColl = await missingTarget.CreateCollectionAsync(cancellationToken).ConfigureAwait(false);
                    var collResult = await this.ExecuteAsync(docUrl, childNode, newColl, childProperties, cancellationToken).ConfigureAwait(false);
                    collectionActionResults = collectionActionResults.Add(collResult);
                }
                else
                {
                    // Test if the target node exists
                    var foundTarget = await target.GetAsync(collection.Name, cancellationToken).ConfigureAwait(false);
                    var docTarget = foundTarget as TDocument;
                    if (docTarget != null)
                    {
                        // We found a document instead of a collection
                        this._logger.LogDebug($"{target.DestinationUrl}: Found a document instead of a collection");
                        var collResult = new CollectionActionResult(ActionStatus.OverwriteFailed, foundTarget);
                        collectionActionResults = collectionActionResults.Add(collResult);
                    }
                    else
                    {
                        var collTarget = foundTarget as TCollection;
                        if (collTarget != null)
                        {
                            // We found a collection: Business as usual
                            var collResult = await this.ExecuteAsync(docUrl, childNode, collTarget, childProperties, cancellationToken).ConfigureAwait(false);
                            collectionActionResults = collectionActionResults.Add(collResult);
                        }
                        else
                        {
                            // We didn't find anything: Business as usual
                            var missingTarget = (TMissing)foundTarget;
                            var newColl = await missingTarget.CreateCollectionAsync(cancellationToken).ConfigureAwait(false);
                            var collResult = await this.ExecuteAsync(docUrl, childNode, newColl, childProperties, cancellationToken).ConfigureAwait(false);
                            collectionActionResults = collectionActionResults.Add(collResult);
                        }
                    }
                }
            }

            try
            {
                if (this._logger.IsEnabled(LogLevel.Trace)) {
                    this._logger.LogTrace($"Set properties on collection {target.DestinationUrl}.");
                }

                var failedPropertyNames = await target.SetPropertiesAsync(properties, cancellationToken).ConfigureAwait(false);
                if (failedPropertyNames.Count != 0)
                {
                    this._logger.LogDebug($"{target.DestinationUrl}: Failed setting properties {string.Join(", ", failedPropertyNames.Select(x => x.ToString()))}");
                    return new CollectionActionResult(ActionStatus.PropSetFailed, target)
                    {
                        FailedProperties = failedPropertyNames,
                        CollectionActionResults = collectionActionResults,
                        DocumentActionResults = documentActionResults,
                    };
                }

                await this._handler.ExecuteAsync(sourceNode.Collection, target, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this._logger.LogDebug($"{sourceNode.Collection.Path}: Cleanup failed with exception {ex.Message}");
                return new CollectionActionResult(ActionStatus.CleanupFailed, target)
                {
                    Exception = ex,
                    CollectionActionResults = collectionActionResults,
                    DocumentActionResults = documentActionResults,
                };
            }

            return new CollectionActionResult(ActionStatus.Created, target)
            {
                CollectionActionResults = collectionActionResults,
                DocumentActionResults = documentActionResults,
            };
        }
    }
}
