namespace Brimborium.WebDavServer.Locking
{
    /// <summary>
    /// The options for the <see cref="ILockManager"/>
    /// </summary>
    public interface ILockManagerOptions
    {
        /// <summary>
        /// Gets or sets the time rounding implementation
        /// </summary>
        ILockTimeRounding Rounding { get; set; }
    }
}
