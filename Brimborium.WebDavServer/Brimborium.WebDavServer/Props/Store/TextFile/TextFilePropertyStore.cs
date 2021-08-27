using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Model.Headers;
using Brimborium.WebDavServer.Props.Dead;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Polly;

namespace Brimborium.WebDavServer.Props.Store.TextFile {
    /// <summary>
    /// A property store that stores the properties in a JSON file
    /// </summary>
    public class TextFilePropertyStore : PropertyStoreBase, IFileSystemPropertyStore {
        private static readonly XName _EtagKey = GetETagProperty.PropertyName;

        private readonly Policy<string> _FileReadPolicy;

        private readonly Policy _FileWritePolicy;

        private readonly ILogger<TextFilePropertyStore> _Logger;

        private readonly TextFilePropertyStoreOptions _Options;

        private readonly bool _StoreInRootOnly;

        private readonly string _StoreEntryName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextFilePropertyStore"/> class.
        /// </summary>
        /// <param name="options">The options for the text file property store</param>
        /// <param name="deadPropertyFactory">The factory for the dead properties</param>
        /// <param name="rootFolder">The root folder where the properties will be stored</param>
        /// <param name="storeInRootOnly">Store all properties in the same JSON text file</param>
        /// <param name="storeEntryName">The name of the JSON text file</param>
        /// <param name="logger">The logger for the property store</param>
        public TextFilePropertyStore(
            TextFilePropertyStoreOptions options,
            IDeadPropertyFactory deadPropertyFactory,
            string rootFolder,
            bool storeInRootOnly,
            string storeEntryName,
            ILogger<TextFilePropertyStore> logger)
            : base(deadPropertyFactory) {
            this._Logger = logger;
            this._Options = options;
            this._StoreInRootOnly = storeInRootOnly;
            this._StoreEntryName = storeEntryName;
            this.RootPath = rootFolder;
            var rnd = new Random();
            this._FileReadPolicy = Policy<string>
                .Handle<IOException>()
                .WaitAndRetry(100, n => TimeSpan.FromMilliseconds(100 + rnd.Next(-10, 10)));
            this._FileWritePolicy = Policy
                .Handle<IOException>()
                .WaitAndRetry(100, n => TimeSpan.FromMilliseconds(100 + rnd.Next(-10, 10)));
        }

        /// <inheritdoc />
        public override int Cost => this._Options.EstimatedCost;

        /// <summary>
        /// Gets or sets the root folder where the JSON file with the properties gets stored.
        /// </summary>
        public string RootPath { get; set; }

        /// <inheritdoc />
        public bool IgnoreEntry(IEntry entry) {
            return entry is IDocument && entry.Name == this._StoreEntryName;
        }

        /// <inheritdoc />
        public override Task<IReadOnlyCollection<XElement>> GetAsync(IEntry entry, CancellationToken cancellationToken) {
            if (this._Logger.IsEnabled(LogLevel.Trace)) {
                this._Logger.LogTrace($"Get properties for {entry.Path}");
            }

            var storeData = this.Load(entry, false, cancellationToken);
            IReadOnlyCollection<XElement> result;
            if (storeData.Entries.TryGetValue(GetEntryKey(entry), out var info)) {
                result = info.Attributes
                    .Where(x => x.Key != GetETagProperty.PropertyName)
                    .Select(x => x.Value)
                    .ToList();
            } else {
                result = new XElement[0];
            }

            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public override Task SetAsync(IEntry entry, IEnumerable<XElement> elements, CancellationToken cancellationToken) {
            if (this._Logger.IsEnabled(LogLevel.Trace)) {
                this._Logger.LogTrace($"Set properties for {entry.Path}");
            }

            var info = this.GetInfo(entry, cancellationToken) ?? new EntryInfo();
            foreach (var element in elements) {
                if (element.Name == GetETagProperty.PropertyName) {
                    this._Logger.LogWarning("The ETag property must not be set using the property store.");
                    continue;
                }

                info.Attributes[element.Name] = element;
            }

            this.UpdateInfo(entry, info, cancellationToken);
            return Task.FromResult(0);
        }

        /// <inheritdoc />
        public override Task<IReadOnlyCollection<bool>> RemoveAsync(IEntry entry, IEnumerable<XName> keys, CancellationToken cancellationToken) {
            if (this._Logger.IsEnabled(LogLevel.Trace)) {
                this._Logger.LogTrace($"Remove properties for {entry.Path}");
            }

            var info = this.GetInfo(entry, cancellationToken) ?? new EntryInfo();
            var result = new List<bool>();
            foreach (var key in keys) {
                if (key == GetETagProperty.PropertyName) {
                    this._Logger.LogWarning("The ETag property must not be set using the property store.");
                    result.Add(false);
                } else {
                    result.Add(info.Attributes.Remove(key));
                }
            }

            this.UpdateInfo(entry, info, cancellationToken);
            return Task.FromResult<IReadOnlyCollection<bool>>(result);
        }

        /// <inheritdoc />
        public override Task RemoveAsync(IEntry entry, CancellationToken cancellationToken) {
            var fileName = this.GetFileNameFor(entry);
            if (!File.Exists(fileName)) {
                return Task.FromResult(0);
            }

            var storeData = this.Load(entry, false, cancellationToken);
            var entryKey = GetEntryKey(entry);
            if (storeData.Entries.Remove(entryKey)) {
                this.Save(entry, storeData, cancellationToken);
            }

            return Task.FromResult(0);
        }

        /// <inheritdoc />
        protected override Task<EntityTag> GetDeadETagAsync(IEntry entry, CancellationToken cancellationToken) {
            var storeData = this.Load(entry, false, cancellationToken);
            var entryKey = GetEntryKey(entry);
            if (!storeData.Entries.TryGetValue(entryKey, out var info)) {
                info = new EntryInfo();
                storeData.Entries.Add(entryKey, info);
            }

            if (!info.Attributes.TryGetValue(_EtagKey, out var etagElement)) {
                var etag = new EntityTag(false);
                etagElement = etag.ToXml();
                info.Attributes[_EtagKey] = etagElement;

                this.Save(entry, storeData, cancellationToken);

                return Task.FromResult(etag);
            }

            return Task.FromResult(EntityTag.FromXml(etagElement));
        }

        /// <inheritdoc />
        protected override Task<EntityTag> UpdateDeadETagAsync(IEntry entry, CancellationToken cancellationToken) {
            var storeData = this.Load(entry, false, cancellationToken);
            var entryKey = GetEntryKey(entry);
            if (!storeData.Entries.TryGetValue(entryKey, out var info)) {
                info = new EntryInfo();
                storeData.Entries.Add(entryKey, info);
            }

            var etag = new EntityTag(false);
            var etagElement = etag.ToXml();
            info.Attributes[_EtagKey] = etagElement;

            this.Save(entry, storeData, cancellationToken);

            return Task.FromResult(etag);
        }

        private static string GetEntryKey(IEntry entry) {
            if (entry is ICollection) {
                return ".";
            }

            return entry.Name.ToLower();
        }

        private void UpdateInfo(IEntry entry, EntryInfo info, CancellationToken cancellationToken) {
            var storeData = this.Load(entry, true, cancellationToken);
            var entryKey = GetEntryKey(entry);
            storeData.Entries[entryKey] = info;
            this.Save(entry, storeData, cancellationToken);
        }

        private EntryInfo GetInfo(IEntry entry, CancellationToken cancellationToken) {
            var storeData = this.Load(entry, false, cancellationToken);

            if (!storeData.Entries.TryGetValue(GetEntryKey(entry), out var info)) {
                info = new EntryInfo();
            }

            return info;
        }

        private void Save(IEntry entry, StoreData data, CancellationToken cancellationToken) {
            this.Save(this.GetFileNameFor(entry), data, cancellationToken);
        }

        private StoreData Load(IEntry entry, bool useCache, CancellationToken cancellationToken) {
            return this.Load(this.GetFileNameFor(entry), useCache, cancellationToken);
        }

        private void Save(string fileName, StoreData data, CancellationToken cancellationToken) {
            try {
                this._FileWritePolicy.Execute(ct => File.WriteAllText(fileName, JsonConvert.SerializeObject(data)), cancellationToken);
            } catch (Exception) {
                // Ignore all exceptions for directories that cannot be modified
            }
        }

        private StoreData Load(string fileName, bool useCache, CancellationToken cancellationToken) {
            if (!File.Exists(fileName)) {
                return new StoreData();
            }

            if (!useCache) {
                var result = JsonConvert.DeserializeObject<StoreData>(
                    this._FileReadPolicy.Execute(ct => File.ReadAllText(fileName), cancellationToken))!;
                return result;
            }

            return JsonConvert.DeserializeObject<StoreData>(
                this._FileReadPolicy.Execute(ct => File.ReadAllText(fileName), cancellationToken))!;
        }

        private string GetFileNameFor(IEntry entry) {
            string result;
            if (this._StoreInRootOnly) {
                result = Path.Combine(this.RootPath, this._StoreEntryName);
            } else {
                var path = this.GetFileSystemPath(entry);
                var isCollection = entry is ICollection;
                if (isCollection) {
                    result = Path.Combine(path, this._StoreEntryName);
                } else {
                    var directoryName = Path.GetDirectoryName(path);
                    Debug.Assert(directoryName != null, "directoryName != null");
                    result = Path.Combine(directoryName, this._StoreEntryName);
                }
            }

            if (this._Logger.IsEnabled(LogLevel.Trace)) {
                this._Logger.LogTrace($"Property store file name for {entry.Path} is {result}");
            }

            return result;
        }

        private string GetFileSystemPath(IEntry entry) {
            var names = new List<string>();

            while (entry.Parent != null) {
                if (!string.IsNullOrEmpty(entry.Name)) {
                    names.Add(entry.Name);
                }

                entry = entry.Parent;
            }

            names.Reverse();
            var result = Path.Combine(this.RootPath, Path.Combine(names.ToArray()));

            if (this._Logger.IsEnabled(LogLevel.Trace)) {
                this._Logger.LogTrace($"File system path for {entry.Path} is {result}");
            }

            return result;
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class StoreData {
            public IDictionary<string, EntryInfo> Entries { get; } = new Dictionary<string, EntryInfo>();
        }

        private class EntryInfo {
            public IDictionary<XName, XElement> Attributes { get; } = new Dictionary<XName, XElement>();
        }
    }
}
