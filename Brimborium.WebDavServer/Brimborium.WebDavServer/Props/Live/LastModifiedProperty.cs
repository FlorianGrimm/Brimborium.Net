using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Props.Generic;

namespace Brimborium.WebDavServer.Props.Live
{
    /// <summary>
    /// The <c>getlastmodified</c> property
    /// </summary>
    public class LastModifiedProperty : GenericDateTimeRfc1123Property, ILiveProperty
    {
        /// <summary>
        /// The XML name of the property
        /// </summary>
        public static readonly XName PropertyName = WebDavXml.Dav + "getlastmodified";

        /// <summary>
        /// Initializes a new instance of the <see cref="LastModifiedProperty"/> class.
        /// </summary>
        /// <param name="propValue">The initial property value</param>
        /// <param name="setValueAsyncFunc">The delegate to set the value asynchronously</param>
        public LastModifiedProperty(DateTime propValue, SetPropertyValueAsyncDelegate<DateTime> setValueAsyncFunc)
            : base(PropertyName, 0, ct => Task.FromResult(propValue), setValueAsyncFunc, WebDavXml.Dav + "lastmodified")
        {
        }

        /// <inheritdoc />
        public async Task<bool> IsValidAsync(CancellationToken cancellationToken)
        {
            return this.Converter.IsValidValue(await this.GetValueAsync(cancellationToken).ConfigureAwait(false));
        }
    }
}
