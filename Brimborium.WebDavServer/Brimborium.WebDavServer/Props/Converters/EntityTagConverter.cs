using System.Xml.Linq;

using Brimborium.WebDavServer.Model.Headers;

namespace Brimborium.WebDavServer.Props.Converters
{
    /// <summary>
    /// Property converter for an <see cref="EntityTag"/>
    /// </summary>
    public class EntityTagConverter : IPropertyConverter<EntityTag>
    {
        /// <inheritdoc />
        public EntityTag FromElement(XElement element)
        {
            return EntityTag.FromXml(element);
        }

        /// <inheritdoc />
        public XElement ToElement(XName name, EntityTag value)
        {
            return value.ToXml();
        }

        /// <inheritdoc />
        public bool IsValidValue(EntityTag value)
        {
            return true;
        }
    }
}
