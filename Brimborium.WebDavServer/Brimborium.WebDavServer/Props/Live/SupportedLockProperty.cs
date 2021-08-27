using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Locking;
using Brimborium.WebDavServer.Model;
using Brimborium.WebDavServer.Props.Converters;

namespace Brimborium.WebDavServer.Props.Live
{
    /// <summary>
    /// The <c>supportedlock</c> property
    /// </summary>
    public class SupportedLockProperty : ITypedReadableProperty<supportedlock>, ILiveProperty
    {
        /// <summary>
        /// The XML name of the property
        /// </summary>
        public static readonly XName PropertyName = WebDavXml.Dav + "supportedlock";

        private static readonly XmlConverter<supportedlock> _converter = new XmlConverter<supportedlock>();

        private readonly ILockManager? _lockManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedLockProperty"/> class.
        /// </summary>
        /// <param name="entry">The file system entry this property is for</param>
        public SupportedLockProperty(IEntry entry)
        {
            this._lockManager = entry.FileSystem.LockManager;
            this.Cost = 0;
            this.Name = PropertyName;
        }

        /// <inheritdoc />
        public XName Name { get; }

        /// <inheritdoc />
        public string? Language { get; } = null;

        /// <inheritdoc />
        public IReadOnlyCollection<XName> AlternativeNames { get; } = new[] { WebDavXml.Dav + "contentlength" };

        /// <inheritdoc />
        public int Cost { get; }

        /// <inheritdoc />
        public Task<bool> IsValidAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public async Task<XElement> GetXmlValueAsync(CancellationToken ct)
        {
            return _converter.ToElement(this.Name, await this.GetValueAsync(ct).ConfigureAwait(false));
        }

        /// <inheritdoc />
        public Task<supportedlock> GetValueAsync(CancellationToken ct)
        {
            var result = new supportedlock();

            if (this._lockManager != null)
            {
                result.lockentry = new[]
                {
                    new supportedlockLockentry()
                    {
                        locktype = new locktype()
                        {
                            Item = new object(),
                        },
                        lockscope = new lockscope()
                        {
                            ItemElementName = ItemChoiceType.shared,
                            Item = new object(),
                        },
                    },
                    new supportedlockLockentry()
                    {
                        locktype = new locktype()
                        {
                            Item = new object(),
                        },
                        lockscope = new lockscope()
                        {
                            ItemElementName = ItemChoiceType.exclusive,
                            Item = new object(),
                        },
                    },
                };
            }

            return Task.FromResult(result);
        }
    }
}
