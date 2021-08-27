﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Props.Dead;
using Brimborium.WebDavServer.Props.Store;

namespace Brimborium.WebDavServer.Props
{
    /// <summary>
    /// The default implementation of a <see cref="IEntryPropertyInitializer"/>
    /// </summary>
    public class DefaultEntryPropertyInitializer : IEntryPropertyInitializer
    {
        /// <inheritdoc />
        public virtual async Task CreatePropertiesAsync(IDocument document, IPropertyStore propertyStore, IWebDavContext context, CancellationToken cancellationToken)
        {
            if (context.RequestHeaders.Headers.TryGetValue("Content-Type", out var contentTypeValues))
            {
                var contentType = contentTypeValues.FirstOrDefault();
                if (!string.IsNullOrEmpty(contentType))
                {
                    var contentTypeProperty = new GetContentTypeProperty(document, propertyStore);
                    await contentTypeProperty.SetValueAsync(contentType, cancellationToken).ConfigureAwait(false);
                }
            }

            await this.CreateGenericPropertiesAsync(document, propertyStore, context, cancellationToken);
        }

        /// <inheritdoc />
        public virtual Task CreatePropertiesAsync(ICollection collection, IPropertyStore propertyStore, IWebDavContext context, CancellationToken cancellationToken)
        {
            return this.CreateGenericPropertiesAsync(collection, propertyStore, context, cancellationToken);
        }

        /// <summary>
        /// Create generic property
        /// </summary>
        /// <param name="entry">The entry to create the properties for</param>
        /// <param name="propertyStore">The property store</param>
        /// <param name="context">The PUT/MKCOL request context</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The task</returns>
        protected virtual Task CreateGenericPropertiesAsync(
            IEntry entry,
            IPropertyStore propertyStore,
            IWebDavContext context,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
