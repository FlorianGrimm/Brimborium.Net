﻿using System;
using System.Diagnostics.CodeAnalysis;

using Brimborium.WebDavServer.Utils;

namespace Brimborium.WebDavServer.Model.Headers {
    /// <summary>
    /// The parser for a <c>Coded-URL</c>
    /// </summary>
    public static class CodedUrlParser {
        /// <summary>
        /// Parse the <c>Coded-URL</c> from a <paramref name="source"/> <see langword="string"/>
        /// </summary>
        /// <param name="source">The <see langword="string"/> to parse the <c>Coded-URL</c> from</param>
        /// <returns>The parsed <c>Coded-URL</c></returns>
        public static Uri Parse(string source) {
            var src = new StringSource(source);
            if (!TryParse(src, out var stateToken)) {
                throw new FormatException("No Coded-URL found");
            }

            if (!src.Empty) {
                throw new FormatException("Unknown content after Coded-URL");
            }

            return stateToken;
        }

        /// <summary>
        /// Tries to parse the <c>Coded-URL</c> from a <paramref name="source"/> <see langword="string"/>
        /// </summary>
        /// <param name="source">The <see langword="string"/> to parse the <c>Coded-URL</c> from</param>
        /// <param name="codedUrl">The parsed <c>Coded-URL</c></param>
        /// <returns><see langword="true"/> when the <c>Coded-URL</c> could be parsed successfully</returns>
        public static bool TryParse(string source,[MaybeNullWhen(false)] out Uri codedUrl) {
            var src = new StringSource(source);
            if (!TryParse(src, out codedUrl)) {
                return false;
            }

            return src.Empty;
        }

        /// <summary>
        /// Tries to parse the <c>Coded-URL</c> from a <see cref="StringSource"/>
        /// </summary>
        /// <param name="source">The <see cref="StringSource"/> to parse the <c>Coded-URL</c> from</param>
        /// <param name="codedUrl">The parsed <c>Coded-URL</c></param>
        /// <returns><see langword="true"/> when the <c>Coded-URL</c> could be parsed successfully</returns>
        internal static bool TryParse(StringSource source, [MaybeNullWhen(false)] out Uri codedUrl) {
            if (!source.AdvanceIf("<")) {
                codedUrl = null;
                return false;
            }

            // Coded-URL found
            var codedUrlText = source.GetUntil('>');
            if (codedUrlText == null) {
                throw new ArgumentException($"{source.Remaining} is not a valid Coded-URL (not ending with '>')", nameof(source));
            }

            source.Advance(1);
            codedUrl = new Uri(codedUrlText, UriKind.RelativeOrAbsolute);
            return true;
        }
    }
}
