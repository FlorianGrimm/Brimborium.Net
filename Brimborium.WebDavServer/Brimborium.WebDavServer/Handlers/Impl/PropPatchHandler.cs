﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Locking;
using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Model.Headers;
using Brimborium.WebDavServer.Props;
using Brimborium.WebDavServer.Props.Dead;
using Brimborium.WebDavServer.Props.Live;
using Brimborium.WebDavServer.Utils;

namespace Brimborium.WebDavServer.Handlers.Impl {
    /// <summary>
    /// Implementation of the <see cref="IPropPatchHandler"/> interface
    /// </summary>
    public class PropPatchHandler : IPropPatchHandler {
        private readonly IFileSystem _fileSystem;

        private readonly IWebDavContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropPatchHandler"/> class.
        /// </summary>
        /// <param name="fileSystem">The root file system</param>
        /// <param name="context">The WebDAV request context</param>
        public PropPatchHandler(IFileSystem fileSystem, IWebDavContext context) {
            this._fileSystem = fileSystem;
            this._context = context;
        }

        private enum ChangeStatus {
            Added,
            Modified,
            Removed,
            Failed,
            Conflict,
            FailedDependency,
            InsufficientStorage,
            ReadOnlyProperty,
        }

        /// <inheritdoc />
        public IEnumerable<string> HttpMethods { get; } = new[] { "PROPPATCH" };

        /// <inheritdoc />
        public async Task<IWebDavResult> PropPatchAsync(
            string path,
            propertyupdate request,
            CancellationToken cancellationToken) {
            var selectionResult = await this._fileSystem.SelectAsync(path, cancellationToken).ConfigureAwait(false);
            if (selectionResult.IsMissing) {
                if (this._context.RequestHeaders.IfNoneMatch != null) {
                    throw new WebDavException(WebDavStatusCode.PreconditionFailed);
                }

                throw new WebDavException(WebDavStatusCode.NotFound);
            }

            var targetEntry = selectionResult.TargetEntry;
            Debug.Assert(targetEntry != null, "targetEntry != null");

            await this._context.RequestHeaders
                .ValidateAsync(selectionResult.TargetEntry, cancellationToken).ConfigureAwait(false);

            var lockRequirements = new Lock(
                new Uri(path, UriKind.Relative),
                this._context.PublicRelativeRequestUrl,
                false,
                new XElement(WebDavXml.Dav + "owner", this._context.User.Identity?.Name ?? string.Empty),
                LockAccessType.Write,
                LockShareMode.Shared,
                TimeoutHeader.Infinite);
            var lockManager = this._fileSystem.LockManager;
            var tempLock = lockManager == null
                ? new ImplicitLock(true)
                : await lockManager.LockImplicitAsync(
                        this._fileSystem,
                        this._context.RequestHeaders.If?.Lists,
                        lockRequirements,
                        cancellationToken)
                    .ConfigureAwait(false);
            if (!tempLock.IsSuccessful) {
                return tempLock.CreateErrorResponse();
            }

            try {
                var propertiesList = new List<IUntypedReadableProperty>();
                await using (var propEnum = targetEntry.GetProperties(this._context.Dispatcher, returnInvalidProperties: true).GetAsyncEnumerator()) {
                    while (await propEnum.MoveNextAsync(cancellationToken).ConfigureAwait(false)) {
                        propertiesList.Add(propEnum.Current);
                    }
                }

                var properties = propertiesList.ToDictionary(x => x.Name);
                var changes =
                    await this.ApplyChangesAsync(targetEntry, properties, request, cancellationToken).ConfigureAwait(false);
                var hasError = changes.Any(x => !x.IsSuccess);
                if (hasError) {
                    changes = await this.RevertChangesAsync(
                            targetEntry,
                            changes,
                            properties,
                            cancellationToken)
                        .ConfigureAwait(false);
                } else {
                    var targetPropStore = targetEntry.FileSystem.PropertyStore;
                    if (targetPropStore != null) {
                        await targetPropStore.UpdateETagAsync(targetEntry, cancellationToken).ConfigureAwait(false);
                    }

                    var parent = targetEntry.Parent;
                    while (parent != null) {
                        var parentPropStore = parent.FileSystem.PropertyStore;
                        if (parentPropStore != null) {
                            await parentPropStore.UpdateETagAsync(parent, cancellationToken)
                                .ConfigureAwait(false);
                        }

                        parent = parent.Parent;
                    }
                }

                var statusCode = hasError ? WebDavStatusCode.Forbidden : WebDavStatusCode.MultiStatus;
                var propStats = new List<propstat>();

                var readOnlyProperties = changes.Where(x => x.Status == ChangeStatus.ReadOnlyProperty).ToList();
                if (readOnlyProperties.Count != 0) {
                    propStats.AddRange(
                        this.CreatePropStats(
                            readOnlyProperties,
                            new error() {
                                ItemsElementName = new[] { ItemsChoiceType.cannotmodifyprotectedproperty, },
                                Items = new[] { new object(), },
                            }));
                    changes = changes.Except(readOnlyProperties).ToList();
                }

                propStats.AddRange(this.CreatePropStats(changes, null));

                var status = new multistatus() {
                    response = new[]
                    {
                        new response()
                        {
                            href = this._context.PublicControllerUrl.Append(path, true).OriginalString,
                            ItemsElementName = propStats.Select(x => ItemsChoiceType2.propstat).ToArray(),
                            Items = propStats.Cast<object>().ToArray(),
                        },
                    },
                };

                return new WebDavResult<multistatus>(statusCode, status);
            } finally {
                await tempLock.DisposeAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private static IUntypedReadableProperty? FindProperty(IReadOnlyDictionary<XName, IUntypedReadableProperty> properties, XName propertyKey) {
            if (properties.TryGetValue(propertyKey, out var foundProperty)) {
                return foundProperty;
            }

            foreach (var item in properties.Values.Where(x => x.AlternativeNames.Count != 0)) {
                if (item.AlternativeNames.Any(x => x == propertyKey)) {
                    return item;
                }
            }

            return null;
        }

        private IEnumerable<propstat> CreatePropStats(IEnumerable<ChangeItem> changes, error? error) {
            var changesByStatusCodes = changes.GroupBy(x => x.StatusCode);
            foreach (var changesByStatusCode in changesByStatusCodes) {
                var elements = new List<XElement>();
                foreach (var changeItem in changesByStatusCode) {
                    elements.Add(new XElement(changeItem.Key));
                }

                var propStat = new propstat() {
                    prop = new prop() {
                        Any = elements.ToArray(),
                    },
                    status = new Status(this._context.RequestProtocol, changesByStatusCode.Key).ToString(),
                    error = error,
                };

                yield return propStat;
            }
        }

        private async Task<IReadOnlyCollection<ChangeItem>> RevertChangesAsync(IEntry entry, IReadOnlyCollection<ChangeItem> changes, IDictionary<XName, IUntypedReadableProperty> properties, CancellationToken cancellationToken) {
            if (entry.FileSystem.PropertyStore == null || this._fileSystem.PropertyStore == null) {
                throw new InvalidOperationException("The property store must be configured");
            }

            var newChangeItems = new List<ChangeItem>();

            foreach (var changeItem in changes.Reverse()) {
                ChangeItem newChangeItem;
                switch (changeItem.Status) {
                    case ChangeStatus.Added:
                        Debug.Assert(entry.FileSystem.PropertyStore != null, "entry.FileSystem.PropertyStore != null");
                        await entry.FileSystem.PropertyStore.RemoveAsync(entry, changeItem.Key, cancellationToken).ConfigureAwait(false);
                        newChangeItem = ChangeItem.FailedDependency(changeItem.Key);
                        properties.Remove(changeItem.Key);
                        break;
                    case ChangeStatus.Modified:
                        Debug.Assert(entry.FileSystem.PropertyStore != null, "entry.FileSystem.PropertyStore != null");
                        Debug.Assert(changeItem.OldValue != null, "changeItem.OldValue != null");
                        if (changeItem.OldValue == null) {
                            throw new InvalidOperationException("There must be a old value for the item to change");
                        }

                        await entry.FileSystem.PropertyStore.SetAsync(entry, changeItem.OldValue, cancellationToken).ConfigureAwait(false);
                        newChangeItem = ChangeItem.FailedDependency(changeItem.Key);
                        break;
                    case ChangeStatus.Removed:
                        if (changeItem.Property != null) {
                            properties.Add(changeItem.Key, changeItem.Property);
                            Debug.Assert(this._fileSystem.PropertyStore != null, "_fileSystem.PropertyStore != null");
                            Debug.Assert(changeItem.OldValue != null, "changeItem.OldValue != null");
                            if (changeItem.OldValue == null) {
                                throw new InvalidOperationException("There must be a old value for the item to change");
                            }

                            await this._fileSystem.PropertyStore.SetAsync(entry, changeItem.OldValue, cancellationToken).ConfigureAwait(false);
                        }

                        newChangeItem = ChangeItem.FailedDependency(changeItem.Key);
                        break;
                    case ChangeStatus.Conflict:
                    case ChangeStatus.Failed:
                    case ChangeStatus.InsufficientStorage:
                    case ChangeStatus.ReadOnlyProperty:
                    case ChangeStatus.FailedDependency:
                        newChangeItem = changeItem;
                        break;
                    default:
                        throw new NotSupportedException();
                }

                newChangeItems.Add(newChangeItem);
            }

            newChangeItems.Reverse();
            return newChangeItems;
        }

        private async Task<IReadOnlyCollection<ChangeItem>> ApplyChangesAsync(IEntry entry, Dictionary<XName, IUntypedReadableProperty> properties, propertyupdate request, CancellationToken cancellationToken) {
            var result = new List<ChangeItem>();
            if (request.Items == null) {
                return result;
            }

            var failed = false;
            foreach (var item in request.Items) {
                IReadOnlyCollection<ChangeItem> changeItems;
                var set = item as propset;
                if (set != null) {
                    changeItems = await this.ApplySetAsync(entry, properties, set, failed, cancellationToken).ConfigureAwait(false);
                } else if (item is propremove remove) {
                    changeItems = await this.ApplyRemoveAsync(entry, properties, remove, failed, cancellationToken).ConfigureAwait(false);
                } else {
                    changeItems = new ChangeItem[0];
                }

                result.AddRange(changeItems);

                failed = failed || changeItems.Any(x => !x.IsSuccess);
            }

            return result;
        }

        private async Task<IReadOnlyCollection<ChangeItem>> ApplyRemoveAsync(IEntry entry, IReadOnlyDictionary<XName, IUntypedReadableProperty> properties, propremove remove, bool previouslyFailed, CancellationToken cancellationToken) {
            var result = new List<ChangeItem>();

            if (remove.prop?.Any == null) {
                return result;
            }

            var language = remove.prop.Language;

            var failed = previouslyFailed;
            foreach (var element in remove.prop.Any) {
                // Add a parent elements xml:lang to the element
                var elementLanguage = element.Attribute(XNamespace.Xml + "lang")?.Value;
                if (string.IsNullOrEmpty(elementLanguage) && !string.IsNullOrEmpty(language)) {
                    element.SetAttributeValue(XNamespace.Xml + "lang", language);
                }

                var propertyKey = element.Name;

                if (failed) {
                    result.Add(ChangeItem.FailedDependency(propertyKey));
                    continue;
                }

                var property = FindProperty(properties, propertyKey);
                if (property != null) {
                    if (!(property is IUntypedWriteableProperty)) {
                        result.Add(ChangeItem.ReadOnly(property, element, "Cannot remove protected property"));
                    } else if (entry.FileSystem.PropertyStore == null) {
                        if (property is IDeadProperty) {
                            result.Add(ChangeItem.ReadOnly(property, element, "Cannot remove dead without property store"));
                        } else {
                            result.Add(ChangeItem.ReadOnly(property, element, "Cannot remove live property"));
                        }
                    } else if (property is ILiveProperty) {
                        result.Add(ChangeItem.Failed(property, "Cannot remove live property"));
                    } else {
                        try {
                            var oldValue = await property.GetXmlValueAsync(cancellationToken).ConfigureAwait(false);
                            var success = await entry.FileSystem.PropertyStore.RemoveAsync(entry, propertyKey, cancellationToken).ConfigureAwait(false);

                            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                            if (!success) {
                                result.Add(ChangeItem.Failed(property, "Couldn't remove property from property store (concurrent access?)"));
                            } else {
                                result.Add(ChangeItem.Removed(property, oldValue));
                            }
                        } catch (Exception ex) {
                            result.Add(ChangeItem.Failed(property, ex.Message));
                            failed = true;
                        }
                    }
                } else {
                    result.Add(ChangeItem.Removed(propertyKey));
                }
            }

            return result;
        }

        private async Task<IReadOnlyCollection<ChangeItem>> ApplySetAsync(IEntry entry, Dictionary<XName, IUntypedReadableProperty> properties, propset set, bool previouslyFailed, CancellationToken cancellationToken) {
            var result = new List<ChangeItem>();

            if (set.prop?.Any == null) {
                return result;
            }

            var language = set.prop.Language;

            var failed = previouslyFailed;
            foreach (var element in set.prop.Any) {
                // Add a parent elements xml:lang to the element
                var elementLanguage = element.Attribute(XNamespace.Xml + "lang")?.Value;
                if (string.IsNullOrEmpty(elementLanguage) && !string.IsNullOrEmpty(language)) {
                    element.SetAttributeValue(XNamespace.Xml + "lang", language);
                }

                if (failed) {
                    result.Add(ChangeItem.FailedDependency(element.Name));
                    continue;
                }

                var property = FindProperty(properties, element.Name);
                if (property != null) {
                    ChangeItem changeItem;
                    try {
                        var writeableProperty = property as IUntypedWriteableProperty;
                        if (writeableProperty != null) {
                            if (entry.FileSystem.PropertyStore == null && writeableProperty is IDeadProperty) {
                                changeItem = ChangeItem.ReadOnly(property, element, "Cannot modify dead without property store");
                            } else {
                                var oldValue = await writeableProperty
                                    .GetXmlValueAsync(cancellationToken)
                                    .ConfigureAwait(false);
                                await writeableProperty
                                    .SetXmlValueAsync(element, cancellationToken)
                                    .ConfigureAwait(false);
                                changeItem = ChangeItem.Modified(property, element, oldValue);
                            }
                        } else {
                            changeItem = ChangeItem.ReadOnly(property, element, "Cannot modify protected property");
                        }
                    } catch (Exception ex) {
                        changeItem = ChangeItem.Failed(property, ex.Message);
                    }

                    failed = !changeItem.IsSuccess;
                    result.Add(changeItem);
                } else {
                    if (entry.FileSystem.PropertyStore == null) {
                        result.Add(ChangeItem.InsufficientStorage(element, "Cannot add dead property without property store"));
                        failed = true;
                    } else {
                        var newProperty = new DeadProperty(entry.FileSystem.PropertyStore, entry, element);
                        properties.Add(newProperty.Name, newProperty);
                        await newProperty.SetXmlValueAsync(element, cancellationToken)
                            .ConfigureAwait(false);
                        result.Add(ChangeItem.Added(newProperty, element));
                    }
                }
            }

            return result;
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local", Justification = "Reviewed. Might be used when locking is implemented.")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Reviewed. Might be used when locking is implemented.")]
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Reviewed. Might be used when locking is implemented.")]
        private class ChangeItem {
            private ChangeItem(
                ChangeStatus status,
                IUntypedReadableProperty? property,
                XElement? newValue,
                XElement? oldValue,
                XName key,
                string? description) {
                this.Status = status;
                this.Property = property;
                this.NewValue = newValue;
                this.OldValue = oldValue;
                this.Key = key;
                this.Description = description;
            }

            public ChangeStatus Status { get; }

            public IUntypedReadableProperty? Property { get; }

            public XElement? NewValue { get; }

            public XElement? OldValue { get; }

            public XName Key { get; }

            public string? Description { get; }

            public bool IsSuccess => this.Status == ChangeStatus.Added || this.Status == ChangeStatus.Modified || this.Status == ChangeStatus.Removed;

            public bool IsFailure => this.Status == ChangeStatus.Conflict || this.Status == ChangeStatus.Failed || this.Status == ChangeStatus.InsufficientStorage || this.Status == ChangeStatus.ReadOnlyProperty;

            public WebDavStatusCode StatusCode {
                get {
                    switch (this.Status) {
                        case ChangeStatus.Added:
                        case ChangeStatus.Modified:
                        case ChangeStatus.Removed:
                            return WebDavStatusCode.OK;
                        case ChangeStatus.Conflict:
                            return WebDavStatusCode.Conflict;
                        case ChangeStatus.FailedDependency:
                            return WebDavStatusCode.FailedDependency;
                        case ChangeStatus.InsufficientStorage:
                            return WebDavStatusCode.InsufficientStorage;
                        case ChangeStatus.Failed:
                        case ChangeStatus.ReadOnlyProperty:
                            return WebDavStatusCode.Forbidden;
                    }

                    throw new NotSupportedException();
                }
            }

            public static ChangeItem Added(IUntypedReadableProperty property, XElement newValue) {
                return new ChangeItem(ChangeStatus.Added, property, newValue, null, property.Name, null);
            }

            public static ChangeItem Modified(IUntypedReadableProperty property, XElement newValue, XElement oldValue) {
                return new ChangeItem(ChangeStatus.Modified, property, newValue, oldValue, property.Name, null);
            }

            public static ChangeItem Removed(IUntypedReadableProperty property, XElement oldValue) {
                return new ChangeItem(ChangeStatus.Removed, property, null, oldValue, property.Name, null);
            }

            public static ChangeItem Removed(XName key) {
                return new ChangeItem(ChangeStatus.Removed, null, null, null, key, null);
            }

            public static ChangeItem Failed(IUntypedReadableProperty property, string description) {
                return new ChangeItem(ChangeStatus.Failed, property, null, null, property.Name, description);
            }

            public static ChangeItem Conflict(IUntypedReadableProperty property, XElement oldValue, string description) {
                return new ChangeItem(ChangeStatus.Conflict, property, null, oldValue, property.Name, description);
            }

            public static ChangeItem FailedDependency(XName key, string description = "Failed dependency") {
                return new ChangeItem(ChangeStatus.FailedDependency, null, null, null, key, description);
            }

            public static ChangeItem InsufficientStorage(XElement newValue, string description) {
                return new ChangeItem(ChangeStatus.InsufficientStorage, null, newValue, null, newValue.Name, description);
            }

            public static ChangeItem ReadOnly(IUntypedReadableProperty property, XElement newValue, string description) {
                return new ChangeItem(ChangeStatus.ReadOnlyProperty, property, newValue, null, property.Name, description);
            }
        }
    }
}
