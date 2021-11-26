namespace UniSwitcher.Domain
{
    /// <summary>
    /// Implement this along with <see cref="IScene"/> if you use Unity Analytics.
    /// </summary>
    public interface IReportable
    {
        /// <summary>
        /// If this returns true, DO NOT SEND ANALYTICS REPORT.
        /// </summary>
        /// <returns></returns>
        bool DoNotReport();
    }
}