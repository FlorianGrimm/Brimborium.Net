﻿using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Model;

namespace Brimborium.WebDavServer.Utils
{
    internal static class RequestHeaderExtensions
    {
        public static async Task ValidateAsync(this IWebDavRequestHeaders headers, IEntry entry, CancellationToken cancellationToken)
        {
            if (headers.IfMatch != null || headers.IfNoneMatch != null)
            {
                // Validate against ETag
                var etag = await entry.GetEntityTagAsync(cancellationToken).ConfigureAwait(false);
                if (headers.IfMatch != null && !headers.IfMatch.IsMatch(etag)) {
                    throw new WebDavException(WebDavStatusCode.PreconditionFailed);
                }

                if (headers.IfNoneMatch != null && !headers.IfNoneMatch.IsMatch(etag)) {
                    throw new WebDavException(WebDavStatusCode.NotModified);
                }
            }

            if (headers.IfModifiedSince != null || headers.IfUnmodifiedSince != null)
            {
                // Validate against last modification time
                var lastWriteTimeUtc = entry.LastWriteTimeUtc;
                if (headers.IfUnmodifiedSince != null && !headers.IfUnmodifiedSince.IsMatch(lastWriteTimeUtc)) {
                    throw new WebDavException(WebDavStatusCode.PreconditionFailed);
                }

                if (headers.IfModifiedSince != null && !headers.IfModifiedSince.IsMatch(lastWriteTimeUtc)) {
                    throw new WebDavException(WebDavStatusCode.NotModified);
                }
            }
        }
    }
}
