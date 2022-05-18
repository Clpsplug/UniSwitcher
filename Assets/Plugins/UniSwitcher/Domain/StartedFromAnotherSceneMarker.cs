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

    public static class MarkerExtension
    {
        public static bool HasStartedByAnotherScene(this StartedFromAnotherSceneMarker m)
        {
            return m != null;
        }
    }
}