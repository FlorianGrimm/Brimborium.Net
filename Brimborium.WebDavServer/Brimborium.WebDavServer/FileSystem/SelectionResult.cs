﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Brimborium.WebDavServer.FileSystem {
    /// <summary>
    /// The result of a <see cref="IFileSystem.SelectAsync"/> operation
    /// </summary>
    public class SelectionResult {
        private static readonly IReadOnlyCollection<string> _emptyCollection = new string[0];
        private readonly IDocument? _document;
        private readonly IReadOnlyCollection<string> _pathEntries;

        internal SelectionResult(SelectionResultType resultType, ICollection collection, IDocument? document, IReadOnlyCollection<string>? pathEntries) {
            this.ResultType = resultType;
            this.Collection = collection;
            this._document = document;
            this._pathEntries = pathEntries ?? _emptyCollection;
        }

        /// <summary>
        /// Gets the type of the result
        /// </summary>
        public SelectionResultType ResultType { get; }

        /// <summary>
        /// Gets a value indicating whether there was a missing path part?
        /// </summary>
        public bool IsMissing =>
            this.ResultType == SelectionResultType.MissingCollection ||
            this.ResultType == SelectionResultType.MissingDocumentOrCollection;

        /// <summary>
        /// Gets the collection of the search result.
        /// </summary>
        /// <remarks>
        /// When <see cref="ResultType"/> is <see cref="SelectionResultType.FoundCollection"/>, this is the found collection.
        /// When <see cref="ResultType"/> is <see cref="SelectionResultType.FoundDocument"/>, this is the parent collection.
        /// Otherwise, this is the last found collection.
        /// </remarks>
        public ICollection Collection { get; }

        /// <summary>
        /// Gets the found document
        /// </summary>
        /// <remarks>
        /// This property is only valid when <see cref="ResultType"/> is <see cref="SelectionResultType.FoundDocument"/>.
        /// </remarks>
        public IDocument? Document {
            get {
                if (this.ResultType != SelectionResultType.FoundDocument) {
                    throw new InvalidOperationException();
                }

                return this._document;
            }
        }

        /// <summary>
        /// Gets the collection of missing child elements
        /// </summary>
        /// <remarks>
        /// This is only valid, when <see cref="IsMissing"/> is <see langword="true"/>.
        /// </remarks>
        public IReadOnlyCollection<string> MissingNames {
            get {
                if (this.ResultType != SelectionResultType.MissingCollection && this.ResultType != SelectionResultType.MissingDocumentOrCollection) {
                    throw new InvalidOperationException();
                }

                return this._pathEntries;
            }
        }

        /// <summary>
        /// Gets the full root-relative path of the element that was searched
        /// </summary>
        public Uri FullPath {
            get {
                switch (this.ResultType) {
                    case SelectionResultType.FoundCollection:
                        Debug.Assert(this.Collection != null, "Collection != null");
                        return this.Collection.Path;
                    case SelectionResultType.FoundDocument:
                        Debug.Assert(this.Document != null, "Document != null");
                        return this.Document.Path;
                }

                var result = new StringBuilder();
                Debug.Assert(this.Collection != null, "Collection != null");
                result.Append(this.Collection.Path.OriginalString);
                Debug.Assert(this.MissingNames != null, "MissingNames != null");
                result.Append(string.Join("/", this.MissingNames.Select(n => n.UriEscape())));
                if (this.ResultType == SelectionResultType.MissingCollection) {
                    result.Append("/");
                }

                return new Uri(result.ToString(), UriKind.Relative);
            }
        }

        /// <summary>
        /// Gets the found target entry
        /// </summary>
        /// <remarks>
        /// This is only valid when <see cref="IsMissing"/> is <see langword="false"/>.
        /// </remarks>
        public IEntry TargetEntry {
            get {
                if (this.IsMissing) {
                    throw new InvalidOperationException();
                }

                if (this.ResultType == SelectionResultType.FoundDocument) {
                    Debug.Assert(this.Document != null, "Document != null");
                    return this.Document;
                }

                return this.Collection;
            }
        }

        /// <summary>
        /// Gets the file system of the found element or the last found collection
        /// </summary>
        public IFileSystem TargetFileSystem => ((IEntry?)this._document ?? this.Collection).FileSystem;

        /// <summary>
        /// Creates a selection result for a found document
        /// </summary>
        /// <param name="collection">The parent collection</param>
        /// <param name="document">The found document</param>
        /// <returns>The created selection result</returns>
        public static SelectionResult Create(ICollection collection, IDocument document) {
            if (collection == null) {
                throw new ArgumentNullException(nameof(collection));
            }

            if (document == null) {
                throw new ArgumentNullException(nameof(document));
            }

            return new SelectionResult(SelectionResultType.FoundDocument, collection, document, null);
        }

        /// <summary>
        /// Creates a selection result for a found collection
        /// </summary>
        /// <param name="collection">The found collection</param>
        /// <returns>The created selection result</returns>
        public static SelectionResult Create(ICollection collection) {
            if (collection == null) {
                throw new ArgumentNullException(nameof(collection));
            }

            return new SelectionResult(SelectionResultType.FoundCollection, collection, null, null);
        }

        /// <summary>
        /// Creates a selection for a missing document or collection
        /// </summary>
        /// <param name="collection">The last found collection</param>
        /// <param name="pathEntries">The missing path elements</param>
        /// <returns>The created selection result</returns>
        public static SelectionResult CreateMissingDocumentOrCollection(ICollection collection, IReadOnlyCollection<string> pathEntries) {
            if (collection == null) {
                throw new ArgumentNullException(nameof(collection));
            }

            if (pathEntries == null) {
                throw new ArgumentNullException(nameof(pathEntries));
            }

            return new SelectionResult(SelectionResultType.MissingDocumentOrCollection, collection, null, pathEntries);
        }

        /// <summary>
        /// Creates a selection for a missing collection
        /// </summary>
        /// <param name="collection">The last found collection</param>
        /// <param name="pathEntries">The missing path elements</param>
        /// <returns>The created selection result</returns>
        public static SelectionResult CreateMissingCollection(ICollection collection, IReadOnlyCollection<string> pathEntries) {
            if (collection == null) {
                throw new ArgumentNullException(nameof(collection));
            }

            if (pathEntries == null) {
                throw new ArgumentNullException(nameof(pathEntries));
            }

            return new SelectionResult(SelectionResultType.MissingCollection, collection, null, pathEntries);
        }
    }
}
