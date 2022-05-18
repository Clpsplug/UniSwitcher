namespace UniSwitcher.Domain
{
    /// <summary>
    /// Basic <see cref="IScene"/> implementation.
    /// You should extend this class to add static members to avoid typing scene paths.
    /// </summary>
    public abstract class BaseScene : IScene
    {
        private readonly string _rawValue;
        
        protected BaseScene(string rawValue)
        {
            _rawValue = rawValue;
        }
        
        public string GetRawValue()
        {
            return _rawValue;
        }
    }
}