﻿using System;

using Brimborium.WebDavServer.Props.Converters;

namespace Brimborium.WebDavServer.Model.Headers
{
    /// <summary>
    /// Class that represents the HTTP <c>If-Modified-Since</c> header
    /// </summary>
    public class IfModifiedSinceHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IfModifiedSinceHeader"/> class.
        /// </summary>
        /// <param name="lastWriteTimeUtc">The last time the defination was changed</param>
        public IfModifiedSinceHeader(DateTime lastWriteTimeUtc)
        {
            this.LastWriteTimeUtc = lastWriteTimeUtc;
        }

        /// <summary>
        /// Gets the last time the definition was changed
        /// </summary>
        public DateTime LastWriteTimeUtc { get; }

        /// <summary>
        /// Parses the header string to get a new instance of the <see cref="IfModifiedSinceHeader"/> class
        /// </summary>
        /// <param name="s">The header string to parse</param>
        /// <returns>The new instance of the <see cref="IfModifiedSinceHeader"/> class</returns>
        public static IfModifiedSinceHeader Parse(string s)
        {
            return new IfModifiedSinceHeader(DateTimeRfc1123Converter.Parse(s));
        }

        /// <summary>
        /// Returns a value that indicates whether the <paramref name="lastWriteTimeUtc"/> is past the value in the <c>If-Modified-Since</c> header
        /// </summary>
        /// <param name="lastWriteTimeUtc">The last write time of the entry to compare with</param>
        /// <returns><see langword="true"/> when the <paramref name="lastWriteTimeUtc"/> is past the value in the <c>If-Modified-Since</c> header</returns>
        public bool IsMatch(DateTime lastWriteTimeUtc)
        {
            return lastWriteTimeUtc > this.LastWriteTimeUtc;
        }
    }
}
