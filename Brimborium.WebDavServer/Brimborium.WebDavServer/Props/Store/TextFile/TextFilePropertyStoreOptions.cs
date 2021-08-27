namespace Brimborium.WebDavServer.Props.Store.TextFile {
    /// <summary>
    /// The options for the <see cref="TextFilePropertyStore"/>
    /// </summary>
    public class TextFilePropertyStoreOptions {
        public TextFilePropertyStoreOptions() {
            this.RootFolder = string.Empty;
        }
        /// <summary>
        /// Gets or sets the default estimated cost for querying the dead properties values
        /// </summary>
        public int EstimatedCost { get; set; } = 10;

        /// <summary>
        /// Gets or sets the root folder where the properties are stored
        /// </summary>
        public string RootFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the properties are stored in the same place as the
        /// file system that's accessible to the user.
        /// </summary>
        public bool StoreInTargetFileSystem { get; set; } = true;
    }
}
