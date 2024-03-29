﻿using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.Props.Dead;

using Microsoft.Extensions.Logging;

namespace Brimborium.WebDavServer.Props.Store.InMemory
{
    /// <summary>
    /// The factory for the <see cref="InMemoryPropertyStore"/>
    /// </summary>
    public class InMemoryPropertyStoreFactory : IPropertyStoreFactory
    {
        private readonly ILogger<InMemoryPropertyStore> _logger;

        private readonly IDeadPropertyFactory _deadPropertyFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryPropertyStoreFactory"/> class.
        /// </summary>
        /// <param name="logger">The logger for the property store factory</param>
        /// <param name="deadPropertyFactory">The factory for dead properties</param>
        public InMemoryPropertyStoreFactory(ILogger<InMemoryPropertyStore> logger, IDeadPropertyFactory deadPropertyFactory)
        {
            this._logger = logger;
            this._deadPropertyFactory = deadPropertyFactory;
        }

        /// <inheritdoc />
        public IPropertyStore Create(IFileSystem fileSystem)
        {
            return new InMemoryPropertyStore(this._deadPropertyFactory, this._logger);
        }
    }
}
