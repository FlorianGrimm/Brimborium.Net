using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Handlers;
using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Model.Headers;
using Brimborium.WebDavServer.Props;
using Brimborium.WebDavServer.Props.Dead;
using Brimborium.WebDavServer.Props.Live;
using Brimborium.WebDavServer.Props.Store;

namespace Brimborium.WebDavServer.Dispatchers {
    /// <summary>
    /// The default WebDAV class 2 implementation
    /// </summary>
    public class WebDavDispatcherClass2 : IWebDavClass2 {
        private readonly ILockHandler? _lockHandler;

        private readonly IUnlockHandler? _unlockHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavDispatcherClass2"/> class.
        /// </summary>
        /// <param name="handlers">The WebDAV class 2 handlers</param>
        /// <param name="context">The WebDAV context</param>
        public WebDavDispatcherClass2(IEnumerable<IClass2Handler> handlers, IWebDavContext context) {
            var httpMethods = new HashSet<string>();

            foreach (var handler in handlers) {
                var handlerFound = false;

                if (handler is ILockHandler lockHandler) {
                    this._lockHandler = lockHandler;
                    handlerFound = true;
                }

                if (handler is IUnlockHandler unlockHandler) {
                    this._unlockHandler = unlockHandler;
                    handlerFound = true;
                }

                if (!handlerFound) {
                    throw new NotSupportedException();
                }

                foreach (var httpMethod in handler.HttpMethods) {
                    httpMethods.Add(httpMethod);
                }
            }

            this.HttpMethods = httpMethods.ToList();
            this.WebDavContext = context;

            this.OptionsResponseHeaders = new Dictionary<string, IEnumerable<string>>() {
                ["Allow"] = HttpMethods,
            };

            this.DefaultResponseHeaders = new Dictionary<string, IEnumerable<string>>() {
                ["DAV"] = new[] { "2" },
            };
        }

        /// <inheritdoc />
        public IEnumerable<string> HttpMethods { get; }

        /// <inheritdoc />
        public IWebDavContext WebDavContext { get; }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, IEnumerable<string>> OptionsResponseHeaders { get; }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, IEnumerable<string>> DefaultResponseHeaders { get; }

        /// <inheritdoc />
        public Task<IWebDavResult> LockAsync(string path, lockinfo info, CancellationToken cancellationToken) {
            if (this._lockHandler == null) {
                throw new NotSupportedException();
            }

            return this._lockHandler.LockAsync(path, info, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IWebDavResult> RefreshLockAsync(string path, IfHeader ifHeader, TimeoutHeader? timeoutHeader, CancellationToken cancellationToken) {
            if (this._lockHandler == null) {
                throw new NotSupportedException();
            }

            return this._lockHandler.RefreshLockAsync(path, ifHeader, timeoutHeader, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IWebDavResult> UnlockAsync(string path, LockTokenHeader stateToken, CancellationToken cancellationToken) {
            if (this._unlockHandler == null) {
                throw new NotSupportedException();
            }

            return this._unlockHandler.UnlockAsync(path, stateToken, cancellationToken);
        }

        /// <inheritdoc />
        public IEnumerable<IUntypedReadableProperty> GetProperties(IEntry entry) {
            yield return new LockDiscoveryProperty(entry);
            yield return new SupportedLockProperty(entry);
        }

        /// <inheritdoc />
        public bool TryCreateDeadProperty(IPropertyStore store, IEntry entry, XName name, [MaybeNullWhen(false)] out IDeadProperty deadProperty) {
            deadProperty = default;
            return false;
        }
    }
}
