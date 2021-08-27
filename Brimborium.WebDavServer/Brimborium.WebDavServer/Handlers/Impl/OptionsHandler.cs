using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Model;

namespace Brimborium.WebDavServer.Handlers.Impl
{
    /// <summary>
    /// Implementation of the <see cref="IOptionsHandler"/> interface.
    /// </summary>
    public class OptionsHandler : IOptionsHandler
    {
        private readonly IFileSystem _rootFileSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsHandler"/> class.
        /// </summary>
        /// <param name="rootFileSystem">The root file system</param>
        public OptionsHandler(IFileSystem rootFileSystem)
        {
            this._rootFileSystem = rootFileSystem;
        }

        /// <inheritdoc />
        public IEnumerable<string> HttpMethods { get; } = new[] { "OPTIONS" };

        /// <inheritdoc />
        public async Task<IWebDavResult> OptionsAsync(string path, CancellationToken cancellationToken)
        {
            var selectionResult = await this._rootFileSystem.SelectAsync(path, cancellationToken).ConfigureAwait(false);
            var targetFileSystem = selectionResult.TargetFileSystem;
            return new WebDavOptionsResult(targetFileSystem);
        }

        private class WebDavOptionsResult : WebDavResult
        {
            private readonly IFileSystem _targetFileSystem;

            public WebDavOptionsResult(IFileSystem targetFileSystem)
                : base(WebDavStatusCode.OK)
            {
                this._targetFileSystem = targetFileSystem;
            }

            public override Task ExecuteResultAsync(IWebDavResponse response, CancellationToken ct)
            {
                IImmutableDictionary<string, IEnumerable<string>> headers = ImmutableDictionary<string, IEnumerable<string>>.Empty;

                foreach (var webDavClass in response.Dispatcher.SupportedClasses) {
                    headers = this.AddHeaderValues(headers, webDavClass.OptionsResponseHeaders);
                }

                if (this._targetFileSystem.SupportsRangedRead) {
                    this.Headers["Accept-Ranges"] = new[] { "bytes" };
                }

                foreach (var header in headers) {
                    this.Headers[header.Key] = header.Value;
                }

                return base.ExecuteResultAsync(response, ct);
            }
        }
    }
}
