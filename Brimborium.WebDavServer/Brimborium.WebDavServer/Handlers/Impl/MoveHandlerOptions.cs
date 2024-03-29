﻿namespace Brimborium.WebDavServer.Handlers.Impl
{
    /// <summary>
    /// The options for the <see cref="MoveHandler"/>
    /// </summary>
    public class MoveHandlerOptions
    {
        /// <summary>
        /// Gets or sets the mode that determines the method used to move files
        /// </summary>
        public RecursiveProcessingMode Mode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the destination of a move operation is overwriteable when
        /// the client doesn't specify the <see cref="Model.Headers.OverwriteHeader"/>.
        /// </summary>
        public bool OverwriteAsDefault { get; set; } = true;
    }
}
