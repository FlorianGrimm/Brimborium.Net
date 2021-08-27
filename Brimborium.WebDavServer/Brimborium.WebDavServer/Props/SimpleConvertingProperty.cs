using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.Props.Converters;

namespace Brimborium.WebDavServer.Props
{
    /// <summary>
    /// Simple converting property
    /// </summary>
    /// <typeparam name="T">The type to be converted from or to</typeparam>
    public abstract class SimpleConvertingProperty<T> : SimpleTypedProperty<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleConvertingProperty{T}"/> class.
        /// </summary>
        /// <param name="name">The property name</param>
        /// <param name="language">The language for the property value</param>
        /// <param name="cost">The cost to get the properties value</param>
        /// <param name="converter">The converter to copy the value to/from an <see cref="XElement"/></param>
        /// <param name="alternativeNames">The alternative names</param>
        protected SimpleConvertingProperty(XName name, string? language, int cost, IPropertyConverter<T> converter, params XName[] alternativeNames)
            : base(name, language, cost, alternativeNames)
        {
            this.Converter = converter;
        }

        /// <summary>
        /// Gets the converter to be used to copy to/from an <see cref="XElement"/>
        /// </summary>
        protected IPropertyConverter<T> Converter { get; }

        /// <inheritdoc />
        public override async Task<XElement> GetXmlValueAsync(CancellationToken ct)
        {
            var result = await this.GetValueAsync(ct).ConfigureAwait(false);
            var element = this.Converter.ToElement(this.Name, result);
            if (!string.IsNullOrEmpty(this.Language)) {
                element.SetAttributeValue(XNamespace.Xml + "lang", this.Language);
            }

            return element;
        }

        /// <inheritdoc />
        public override Task SetXmlValueAsync(XElement element, CancellationToken ct)
        {
            this.Language = element.Attribute(XNamespace.Xml + "lang")?.Value;
            return this.SetValueAsync(this.Converter.FromElement(element), ct);
        }
    }
}
