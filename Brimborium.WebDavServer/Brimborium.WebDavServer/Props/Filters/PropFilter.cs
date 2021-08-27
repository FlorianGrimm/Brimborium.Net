using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Brimborium.WebDavServer.Model;

namespace Brimborium.WebDavServer.Props.Filters
{
    /// <summary>
    /// Filters the allowed properties by name
    /// </summary>
    public class PropFilter : IPropertyFilter
    {
        private readonly ISet<XName> _selectedProperties;

        private readonly HashSet<XName> _requestedProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropFilter"/> class.
        /// </summary>
        /// <param name="prop">The <see cref="prop"/> element containing the property names</param>
        public PropFilter(prop prop)
        {
            this._requestedProperties = new HashSet<XName>(prop.Any.Select(x => x.Name));
            this._selectedProperties = new HashSet<XName>();
        }

        /// <inheritdoc />
        public void Reset()
        {
            this._selectedProperties.Clear();
        }

        /// <inheritdoc />
        public bool IsAllowed(IProperty property)
        {
            return this._requestedProperties.Contains(property.Name);
        }

        /// <inheritdoc />
        public void NotifyOfSelection(IProperty property)
        {
            this._selectedProperties.Add(property.Name);
        }

        /// <inheritdoc />
        public IEnumerable<MissingProperty> GetMissingProperties()
        {
            var missingProps = new HashSet<XName>(this._requestedProperties);
            missingProps.ExceptWith(this._selectedProperties);
            return missingProps.Select(x => new MissingProperty(WebDavStatusCode.NotFound, x));
        }
    }
}
