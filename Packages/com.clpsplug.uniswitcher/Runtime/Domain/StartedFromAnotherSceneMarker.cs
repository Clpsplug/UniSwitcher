namespace UniSwitcher.Domain
{
    /// <summary>
    /// This is a 'Marker Class' which denotes that the scene was started via UniSwitcher.
    /// This is (probably) only useful when you need to test a scene in development
    /// where you have a scene that depends on data from another scene and
    /// you want to add the test-only initialization code without disturbing the intended
    /// load process.
    /// </summary>
    /// <remarks>
    /// You cannot instantiate this class.
    /// To use it, try injecting by <see cref="Zenject.InjectOptionalAttribute"/>
    /// and check if it's not null or check <see cref="MarkerExtension.HasStartedByAnotherScene"/>.
    /// </remarks>
    public class StartedFromAnotherSceneMarker
    {
        internal StartedFromAnotherSceneMarker()
        {
            /* no-op */
        }
    }

    /// <summary>
    /// Extension class for <see cref="StartedFromAnotherSceneMarker"/>
    /// </summary>
    public static class MarkerExtension
    {
        /// <summary>
        /// Returns true if the scene with this marker is started from another scene via UniSwitcher.
        /// If this is false, it either means:
        /// <list type="bullet">
        /// <item>the Play Mode started from that scene or</item>
        /// <item>the Scene change occurred outside of UniSwitcher.</item>
        /// </list>
        /// </summary>
        /// <param name="m">The marker itself - no need to pass an argument.</param>
        /// <returns>True if the scene is started from another marker. If false, you may need to take action.</returns>
        public static bool HasStartedByAnotherScene(this StartedFromAnotherSceneMarker m)
        {
            return m != null;
        }
    }
}