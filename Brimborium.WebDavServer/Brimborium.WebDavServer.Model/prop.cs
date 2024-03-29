﻿using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Brimborium.WebDavServer.Model
{
    /// <summary>
    /// The WebDAV prop element
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Name created by xsd tool.")]
    //// ReSharper disable once InconsistentNaming
    public partial class prop
    {
        /// <summary>
        /// Gets or sets the language code
        /// </summary>
        [XmlAttribute("xml:lang", DataType = "language")]
        public string Language { get; set; }
    }
}
