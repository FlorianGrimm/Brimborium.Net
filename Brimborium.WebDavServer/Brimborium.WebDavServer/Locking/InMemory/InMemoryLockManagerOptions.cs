namespace Brimborium.WebDavServer.Locking.InMemory
{
    /// <summary>
    /// Options for the <see cref="InMemoryLockManager"/>
    /// </summary>
    public class InMemoryLockManagerOptions : ILockManagerOptions
    {
        /// <inheritdoc />
        public ILockTimeRounding Rounding { get; set; } = new DefaultLockTimeRounding(DefaultLockTimeRoundingMode.OneSecond);
    }
}
