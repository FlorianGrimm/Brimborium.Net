using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Model.Headers;
using Brimborium.WebDavServer.Props;
using Brimborium.WebDavServer.Props.Dead;
using Brimborium.WebDavServer.Props.Live;
using Brimborium.WebDavServer.Utils;

namespace Brimborium.WebDavServer.Handlers.Impl.GetResults
{
    internal class WebDavPartialDocumentResult : WebDavResult
    {
        private readonly IDocument _document;

        private readonly bool _returnFile;

        private readonly IReadOnlyCollection<NormalizedRangeItem> _rangeItems;

        public WebDavPartialDocumentResult(IDocument document, bool returnFile, IReadOnlyCollection<NormalizedRangeItem> rangeItems)
            : base(WebDavStatusCode.PartialContent)
        {
            this._document = document;
            this._returnFile = returnFile;
            this._rangeItems = rangeItems;
        }

        public override async Task ExecuteResultAsync(IWebDavResponse response, CancellationToken ct)
        {
            await base.ExecuteResultAsync(response, ct).ConfigureAwait(false);

            response.Headers["Accept-Ranges"] = new[] { "bytes" };

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

            var views = new List<StreamView>();
            try
            {
                foreach (var rangeItem in this._rangeItems)
                {
                    var baseStream = await this._document.OpenReadAsync(ct).ConfigureAwait(false);
                    var streamView = await StreamView
                        .CreateAsync(baseStream, rangeItem.From, rangeItem.Length, ct)
                        .ConfigureAwait(false);
                    views.Add(streamView);
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

                if (this._rangeItems.Count == 1)
                {
                    // No multipart content
                    var rangeItem = this._rangeItems.Single();
                    var streamView = views.Single();
                    using (var streamContent = new StreamContent(streamView))
                    {
                        streamContent.Headers.ContentRange = new ContentRangeHeaderValue(
                            rangeItem.From,
                            rangeItem.To,
                            this._document.Length);
                        streamContent.Headers.ContentLength = rangeItem.Length;

                        streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

                        await SetPropertiesToContentHeaderAsync(streamContent, properties, ct)
                            .ConfigureAwait(false);

                        foreach (var header in streamContent.Headers)
                        {
                            response.Headers.Add(header.Key, header.Value.ToArray());
                        }

                        // Use the CopyToAsync function of the stream itself, because
                        // we're able to pass the cancellation token. This is a workaround
                        // for issue dotnet/corefx#9071 and fixes FubarDevelopment/WebDavServer#47.
                        await streamView.CopyToAsync(response.Body, 81920, ct)
                            .ConfigureAwait(false);
                    }
                }
                else
                {
                    // Multipart content
                    using (var multipart = new MultipartContent("byteranges"))
                    {
                        var index = 0;
                        foreach (var rangeItem in this._rangeItems)
                        {
                            var streamView = views[index++];
                            var partContent = new StreamContent(streamView);
                            partContent.Headers.ContentRange = new ContentRangeHeaderValue(
                                rangeItem.From,
                                rangeItem.To,
                                this._document.Length);
                            partContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                            partContent.Headers.ContentLength = rangeItem.Length;
                            multipart.Add(partContent);
                        }

                        await SetPropertiesToContentHeaderAsync(multipart, properties, ct)
                            .ConfigureAwait(false);

                        foreach (var header in multipart.Headers)
                        {
                            response.Headers.Add(header.Key, header.Value.ToArray());
                        }

                        // TODO: Workaround for issue dotnet/corefx#9071
                        await multipart.CopyToAsync(response.Body).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                foreach (var streamView in views)
                {
                    streamView.Dispose();
                }
            }
        }

        private async Task SetPropertiesToContentHeaderAsync(
            HttpContent content,
            IReadOnlyCollection<IUntypedReadableProperty> properties,
            CancellationToken ct)
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
        }
    }
}
