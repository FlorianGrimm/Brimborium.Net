﻿using System.Collections.Generic;
using System.Xml.Linq;

namespace Brimborium.WebDavServer.Props
{
    /// <summary>
    /// The base property interface
    /// </summary>
    public interface IProperty
    {
        /// <summary>
        /// Gets the XML name of the property
        /// </summary>
        XName Name { get; }

        /// <summary>
        /// Gets the language of this property value
        /// </summary>
        string? Language { get; }

        /// <summary>
        /// Gets the alternative XML names
        /// </summary>
        IReadOnlyCollection<XName> AlternativeNames { get; }
    }
}
