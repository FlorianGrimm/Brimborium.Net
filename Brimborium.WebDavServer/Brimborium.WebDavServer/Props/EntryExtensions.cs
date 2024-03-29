﻿using System;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Props.Live;

namespace Brimborium.WebDavServer.Props
{
    /// <summary>
    /// Extension methods for an <see cref="IEntry"/>
    /// </summary>
    public static class EntryExtensions
    {
        /// <summary>
        /// Gets the default resource type for the given <paramref name="entry"/>
        /// </summary>
        /// <param name="entry">The entry to get the resource type property for</param>
        /// <returns>The resource type property</returns>
        public static ILiveProperty GetResourceTypeProperty(this IEntry entry)
        {
            var coll = entry as ICollection;

            if (coll != null) {
                return ResourceTypeProperty.GetCollectionResourceType();
            }

            var doc = entry as IDocument;
            if (doc != null) {
                return ResourceTypeProperty.GetDocumentResourceType();
            }

            throw new NotSupportedException();
        }
    }
}
