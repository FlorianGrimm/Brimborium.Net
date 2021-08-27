using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Model;

namespace Brimborium.WebDavServer.Handlers.Impl.GetResults
{
    internal class WebDavCollectionResult : WebDavResult
    {
        private readonly ICollection _collection;

        public WebDavCollectionResult(ICollection collection)
            : base(WebDavStatusCode.OK)
        {
            this._collection = collection;
        }

        public override async Task ExecuteResultAsync(IWebDavResponse response, CancellationToken ct)
        {
            await base.ExecuteResultAsync(response, ct).ConfigureAwait(false);
            response.Headers["Last-Modified"] = new[] { this._collection.LastWriteTimeUtc.ToString("R") };
        }
    }
}
