using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Brimborium.WebDavServer.AspNetCore {
    /// <summary>
    /// The implementation of the <see cref="IWebDavResponse"/>
    /// </summary>
    /// <remarks>
    /// This class wraps a <see cref="HttpResponse"/> to be accessible by the WebDAV serves <see cref="IWebDavResult"/>.
    /// </remarks>
    public class WebDavResponse : IWebDavResponse {
        private readonly HttpResponse _response;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDavResponse"/> class.
        /// </summary>
        /// <param name="dispatcher">The WebDAV HTTP method dispatcher</param>
        /// <param name="response">The ASP.NET Core HTTP response</param>
        public WebDavResponse(IWebDavDispatcher dispatcher, HttpResponse response) {
            this._response = response;
            this.Dispatcher = dispatcher;
            this.Headers = new HeadersDictionary(this._response.Headers);
        }

        /// <inheritdoc />
        public IWebDavDispatcher Dispatcher { get; }

        /// <inheritdoc />
        public IDictionary<string, string[]> Headers { get; }

        /// <inheritdoc />
        public string ContentType {
            get { return this._response.ContentType; }
            set { this._response.ContentType = value; }
        }

        /// <inheritdoc />
        public Stream Body => this._response.Body;

        private class HeadersDictionary : IDictionary<string, string[]> {
            private readonly IHeaderDictionary _headers;

            public HeadersDictionary(IHeaderDictionary headers) {
                this._headers = headers;
            }

            public int Count => this._headers.Count;

            public bool IsReadOnly => this._headers.IsReadOnly;

            public ICollection<string> Keys => this._headers.Keys;

            public ICollection<string[]> Values => this._headers.Values.Select(x => x.ToArray()).ToList();

            public string[] this[string key] {
                get { return this._headers[key].ToArray(); }
                set { this._headers[key] = new StringValues(value); }
            }

            public IEnumerator<KeyValuePair<string, string[]>> GetEnumerator() {
                return this._headers.Select(x => new KeyValuePair<string, string[]>(x.Key, x.Value.ToArray())).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return this.GetEnumerator();
            }

            void ICollection<KeyValuePair<string, string[]>>.Add(KeyValuePair<string, string[]> item) {
                this._headers[item.Key] = new StringValues(item.Value);
            }

            public void Clear() {
                this._headers.Clear();
            }

            bool ICollection<KeyValuePair<string, string[]>>.Contains(KeyValuePair<string, string[]> item) {
                var values = this._headers[item.Key].ToArray();
                if (item.Value.Length != values.Length) {
                    return false;
                }

                for (var i = 0; i != values.Length; ++i) {
                    if (item.Value[i] != values[i]) {
                        return false;
                    }
                }

                return true;
            }

            void ICollection<KeyValuePair<string, string[]>>.CopyTo(KeyValuePair<string, string[]>[] array, int arrayIndex) {
                foreach (KeyValuePair<string, StringValues> header in this._headers) {
                    array[arrayIndex++] = new KeyValuePair<string, string[]>(header.Key, header.Value.ToArray());
                }
            }

            bool ICollection<KeyValuePair<string, string[]>>.Remove(KeyValuePair<string, string[]> item) {
                return this.Remove(item.Key);
            }

            public void Add(string key, string[] value) {
                this._headers.Add(key, new StringValues(value));
            }

            public bool ContainsKey(string key) {
                return this._headers.ContainsKey(key);
            }

            public bool Remove(string key) {
                return this._headers.Remove(key);
            }

            public bool TryGetValue(string key, [MaybeNullWhen(false)] out string[] value) {
                StringValues values;
                if (this._headers.TryGetValue(key, out values)) {
                    value = values.ToArray();
                    return true;
                }

                value = null;
                return false;
            }
        }
    }
}
