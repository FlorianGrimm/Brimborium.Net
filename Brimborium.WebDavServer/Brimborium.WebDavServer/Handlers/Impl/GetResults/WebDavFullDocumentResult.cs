using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Props;
using Brimborium.WebDavServer.Props.Dead;
using Brimborium.WebDavServer.Props.Live;
using Brimborium.WebDavServer.Utils;

namespace Brimborium.WebDavServer.Handlers.Impl.GetResults
{
    internal class WebDavFullDocumentResult : WebDavResult
    {
        private readonly IDocument _document;

        private readonly bool _returnFile;

        public WebDavFullDocumentResult(IDocument document, bool returnFile)
            : base(WebDavStatusCode.OK)
        {
            this._document = document;
            this._returnFile = returnFile;
        }

        public override async Task ExecuteResultAsync(IWebDavResponse response, CancellationToken ct)
        {
            await base.ExecuteResultAsync(response, ct).ConfigureAwait(false);

            if (this._document.FileSystem.SupportsRangedRead) {
                response.Headers["Accept-Ranges"] = new[] { "bytes" };
            }

            var properties = await this._document.GetProperties(response.Dispatcher).ToListAsync(ct).ConfigureAwait(false);
            var etagProperty = properties.OfType<GetETagProperty>().FirstOrDefault();
            if (etagProperty != null)
            {
                var propValue = await etagProperty.GetValueAsync(ct).ConfigureAwait(false);
                response.Headers["ETag"] = new[] { propValue.ToString() };
            }

            if (!this._returnFile)
            {
                var lastModifiedProp = properties.OfType<LastModifiedProperty>().FirstOrDefault();
                if (lastModifiedProp != null)
                {
                    var propValue = await lastModifiedProp.GetValueAsync(ct).ConfigureAwait(false);
                    response.Headers["Last-Modified"] = new[] { propValue.ToString("R") };
                }

                return;
            }

            using (var stream = await this._document.OpenReadAsync(ct).ConfigureAwait(false))
            {
                using (var content = new StreamContent(stream))
                {
                    // I'm storing the headers in the content, because I'm too lazy to
                    // look up the header names and the formatting of its values.
                    await SetPropertiesToContentHeaderAsync(content, properties, ct).ConfigureAwait(false);

                    foreach (var header in content.Headers)
                    {
                        response.Headers.Add(header.Key, header.Value.ToArray());
                    }

                    // Use the CopyToAsync function of the stream itself, because
                    // we're able to pass the cancellation token. This is a workaround
                    // for issue dotnet/corefx#9071 and fixes FubarDevelopment/WebDavServer#47.
                    await stream.CopyToAsync(response.Body, 81920, ct)
                        .ConfigureAwait(false);
                }
            }
        }

        private async Task SetPropertiesToContentHeaderAsync(HttpContent content, IReadOnlyCollection<IUntypedReadableProperty> properties, CancellationToken ct)
        {
            var lastModifiedProp = properties.OfType<LastModifiedProperty>().FirstOrDefault();
            if (lastModifiedProp != null)
            {
                var propValue = await lastModifiedProp.GetValueAsync(ct).ConfigureAwait(false);
                content.Headers.LastModified = new DateTimeOffset(propValue);
            }

            var contentLanguageProp = properties.OfType<GetContentLanguageProperty>().FirstOrDefault();
            if (contentLanguageProp != null)
            {
                var propValue = await contentLanguageProp.TryGetValueAsync(ct).ConfigureAwait(false);
                if (propValue.Item1) {
                    content.Headers.ContentLanguage.Add(propValue.Item2);
                }
            }

            string contentType;
            var contentTypeProp = properties.OfType<GetContentTypeProperty>().FirstOrDefault();
            if (contentTypeProp != null)
            {
                contentType = await contentTypeProp.GetValueAsync(ct).ConfigureAwait(false);
            }
            else
            {
                contentType = MimeTypesMap.DefaultMimeType;
            }

            content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = this._document.Name,
                FileNameStar = this._document.Name,
            };

            content.Headers.ContentDisposition = contentDisposition;
        }
    }
}
