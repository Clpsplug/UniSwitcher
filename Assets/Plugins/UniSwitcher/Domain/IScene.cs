namespace UniSwitcher.Domain
{
    /// <summary>
    /// Interface for scene object
    /// </summary>
    /// <remarks>This object should hold a relative path (starting from Assets/) to the current Unity scene.</remarks>
    public interface IScene
    {
        /// <summary>
        /// Internal usage only - used to match against what Unity returns.
        /// </summary>
        /// <returns></returns>
        string GetRawValue();
    }
}